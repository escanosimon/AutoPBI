using System;
using System.Management.Automation;
using System.Threading.Tasks;
using AutoPBI.Models;
using AutoPBI.Services;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoPBI.ViewModels.Popups;

public partial class RefreshPopupViewModel : PopupViewModel
{
    [ObservableProperty] private bool _isRefreshing;
    
    public RefreshPopupViewModel(MainViewModel mainViewModel) : base(mainViewModel)
    {
        MainViewModel = mainViewModel;
    }

    public RefreshPopupViewModel() : base(new MainViewModel()) {}

    [RelayCommand]
    private async void Refresh()
    {
        IsRefreshing = true;
        
        foreach (var report in MainViewModel.SelectedReports)
        {
            if (!IsRefreshing) return;
            report.Status = Report.StatusType.Loading;
            Dataset dataset;
            try
            {
                dataset = MainViewModel.Datasets[report.DatasetId!];
            }
            catch (Exception e)
            {
                report.Status = Report.StatusType.Error;
                report.Message = e.Message;
                continue;
            }

            if (dataset.IsRefreshable)
            {
                PSObject refreshHistory;
                var apiUrl =
                    $"https://api.powerbi.com/v1.0/myorg/groups/{dataset.Workspace.Id}/datasets/{dataset.Id}/refreshes";
                try
                {
                    await MainViewModel.PowerShellService.BuildCommand()
                        .WithCommand("Invoke-PowerBIRestMethod")
                        .WithArguments(args => args
                            .Add("-Url")
                            .Add(apiUrl)
                            .Add("-Method")
                            .Add("Post")
                        )
                        .WithStandardErrorPipe(Console.Error.WriteLine)
                        .ExecuteAsync();
                }
                catch (Exception e)
                {
                    report.Status = Report.StatusType.Error;
                    report.Message = e.Message;
                    continue;
                }
                
                try
                {
                    var result = await MainViewModel.PowerShellService.BuildCommand()
                        .WithCommand("Invoke-PowerBIRestMethod")
                        .WithArguments(args => args
                            .Add("-Url")
                            .Add(apiUrl)
                            .Add("-Method")
                            .Add("Get")
                            .Add("-ErrorAction")
                            .Add("Stop")
                            .Add("|")
                            .Add("ConvertFrom-Json")
                        )
                        .WithStandardErrorPipe(Console.Error.WriteLine)
                        .ExecuteAsync();
                    refreshHistory = result.Objects[0];
                }
                catch (Exception e)
                {
                    report.Status = Report.StatusType.Error;
                    report.Message = e.Message;
                    continue;
                }
                
                var value = (Object[]) refreshHistory.Properties["value"].Value;
                var count = value.Length;

                if (count > 0)
                {
                    
                    report.Status = Report.StatusType.Success;
                    report.Message = "Dataset has more than 0 refreshes";
                    
                    var obj = (PSObject) value[0];
                    
                    if (obj.Properties["status"].Value.ToString()!.Equals("Failed"))
                    {
                        report.Status = Report.StatusType.Error;
                        report.Message = obj.Properties["serviceExceptionJson"].Value.ToString();
                    }
                    else
                    {
                        report.Status = Report.StatusType.Success;
                        report.Message = "Successfully refreshed";
                    }
                }
                else
                {
                    report.Status = Report.StatusType.Warning;
                    report.Message = "Dataset is refreshable, but no refresh history";
                }
            }
            else
            {
                report.Status = Report.StatusType.Error;
                report.Message = "Dataset is not refreshable";
            }
        }
    }
    
    [RelayCommand]
    private void Close()
    {
        IsVisible = false;
        
        if (!IsRefreshing) return;
        Console.Error.WriteLine("Refresh stopped...");
        IsRefreshing =  false;
        foreach (var report in MainViewModel.SelectedReports)
        {
            report.Status = Report.StatusType.Selectable;
        }
    }
}