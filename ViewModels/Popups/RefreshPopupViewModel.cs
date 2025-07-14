using System;
using System.Management.Automation;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AutoPBI.Models;
using AutoPBI.Services;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.Rendering;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoPBI.ViewModels.Popups;

public partial class RefreshPopupViewModel : PopupViewModel
{
    public RefreshPopupViewModel(MainViewModel mainViewModel) : base(mainViewModel)
    {
        MainViewModel = mainViewModel;
    }

    public RefreshPopupViewModel() : base(new MainViewModel()) {}

    [RelayCommand]
    private async void Scan()
    {
        IsProcessing = true;
        
        foreach (var report in MainViewModel.SelectedReports)
        {
            if (!IsProcessing) return;
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
                    var obj = (PSObject) value[0];

                    switch (obj.Properties["status"].Value.ToString()!)
                    {
                        case "Failed":
                            report.Status = Report.StatusType.Error;
                            report.Message = obj.Properties["serviceExceptionJson"].Value.ToString();
                            break;
                        case "Unknown":
                            report.Status = Report.StatusType.Warning;
                            var messageBuilder = new StringBuilder();
        
                            foreach (var property in obj.Properties)
                            {
                                // Assuming each property has a Name and a Value
                                messageBuilder.AppendLine($"{property.Name}: {property.Value}");
                            }
                            report.Message = obj.ToString();
                            break;
                        default:
                            report.Status = Report.StatusType.Success;
                            report.Message = "No issues found.";
                            break;
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
                report.Status = Report.StatusType.Warning;
                report.Message = "Dataset is not refreshable";
            }
        }
        IsProcessing = false;
    }

    [RelayCommand]
    private async void Refresh()
    {
        IsProcessing = true;
        
        foreach (var report in MainViewModel.SelectedReports)
        {
            if (!IsProcessing) return;
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
            }
            else
            {
                report.Status = Report.StatusType.Warning;
                report.Message = "Dataset is not refreshable";
                continue;
            }
            report.Status = Report.StatusType.Success;
            report.Message = "Successfully refreshed";
        }

        IsProcessing = false;
    }

    [RelayCommand]
    private async void ReAuth()
    {
        foreach (var report in MainViewModel.SelectedReports)
        {
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

            var datasourceResult = await MainViewModel.PowerShellService.BuildCommand()
                .WithCommand($"Get-PowerBIDataSource -DatasetId {dataset.Id}")
                .ExecuteAsync();
            foreach (var datasourceObj in  datasourceResult.Objects)
            {
                var gatewayId = datasourceObj.Properties["GatewayId"].Value.ToString();
                Console.Error.WriteLine(gatewayId);
                CommandResult gatewayResult;
                try
                {
                    gatewayResult = await MainViewModel.PowerShellService.BuildCommand()
                        .WithCommand(
                            $"Invoke-PowerBIRestMethod -Url 'gateways/{gatewayId}' -Method Get | ConvertFrom-Json")
                        .ExecuteAsync();
                }
                catch (Exception e)
                {
                    report.Status = Report.StatusType.Error;
                    report.Message = e.Message;
                    continue;
                }

                foreach (var obj in gatewayResult.Objects)
                {
                    
                }
                // var gateway = gatewayResult.Objects[0];
                // foreach (var gatewayProperty in gateway.Properties)
                // {
                //     Console.WriteLine($"{gatewayProperty.Name}: {gatewayProperty.Value}");
                // }
            }
        }
    }
}