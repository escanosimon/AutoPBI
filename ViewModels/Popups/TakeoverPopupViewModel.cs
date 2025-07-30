using System;
using System.Linq;
using AutoPBI.Controls;
using AutoPBI.Models;
using CommunityToolkit.Mvvm.Input;

namespace AutoPBI.ViewModels.Popups;

public partial class TakeoverPopupViewModel : PopupViewModel
{
    public TakeoverPopupViewModel(MainViewModel mainViewModel) : base(mainViewModel)
    {
        MainViewModel = mainViewModel;
    }

    public TakeoverPopupViewModel() : base(new MainViewModel()) {}

    [RelayCommand]
    private async void Takeover()
    {
        IsProcessing = true;
        RestartCts();
        
        var successes = 0;
        var warnings = 0;
        var errors = 0;
        
        foreach (var workspace in MainViewModel.Workspaces.ToList())
        {
            foreach (var report in workspace.SelectedReports.ToList())
            {
                report.Loading();

                try
                {
                    var url =
                        $"https://api.powerbi.com/v1.0/myorg/groups/{workspace.Id}/reports/{report.Id}/Default.TakeOver";
                    await Psr.Wrap()
                        .WithArguments(args => args.Add("Invoke-PowerBIRestMethod"))
                        .WithArguments(args => args
                            .Add("-Url")
                            .Add(url)
                            .Add("-Method")
                            .Add("Post")
                        )
                        .WithStandardErrorPipe(Console.Error.WriteLine)
                        .ExecuteAsync(Cts.Token);
                }
                catch (OperationCanceledException)
                {
                    SetReportsSelectable();
                    MainViewModel.Toast(Toast.StatusType.Normal, "Takeover cancelled!", $"Last to takeover: {report.Name}");
                    return;
                }
                catch (Exception e)
                {
                    report.Error(e.Message);
                    errors++;
                    continue;
                }
                
                Dataset dataset;
                try
                {
                    if (report.DatasetId == null)
                    {
                        report.Error("Report has no linked dataset.");
                        errors++;
                        continue;
                    }
                    dataset = MainViewModel.Datasets[report.DatasetId];
                }
                catch (Exception)
                {
                    report.Error("You do not have permissions to the underlying dataset. Please contact the dataset owner to request access.");
                    errors++;
                    continue;
                }
                
                try
                {
                    var url = $"https://api.powerbi.com/v1.0/myorg/groups/{dataset.Workspace.Id}/datasets/{dataset.Id}/Default.TakeOver";
                    await Psr.Wrap()
                        .WithArguments(args => args.Add("Invoke-PowerBIRestMethod"))
                        .WithArguments(args => args
                            .Add("-Url")
                            .Add(url)
                            .Add("-Method")
                            .Add("Post")
                        )
                        .WithStandardErrorPipe(Console.Error.WriteLine)
                        .ExecuteAsync(Cts.Token);
                }
                catch (OperationCanceledException)
                {
                    SetReportsSelectable();
                    MainViewModel.Toast(Toast.StatusType.Normal, "Refreshing cancelled!", $"Last to refresh: {report.Name}");
                    return;
                }
                catch (Exception e)
                {
                    report.Error(e.Message);
                    errors++;
                    continue;
                }
                
                report.Success("Took over report successfully");
                successes++;
            }
        }

        ToastCommand(successes, warnings, errors).Execute(("Takeover finished!", $"{successes} Took over, {warnings} warnings, {errors} errors."));
        IsProcessing = false;
    }
}