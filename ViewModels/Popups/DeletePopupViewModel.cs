using System;
using System.Threading.Tasks;
using AutoPBI.Models;
using CommunityToolkit.Mvvm.Input;

namespace AutoPBI.ViewModels.Popups;

public partial class DeletePopupViewModel : PopupViewModel
{
    public DeletePopupViewModel(MainViewModel mainViewModel) : base(mainViewModel)
    {
        MainViewModel = mainViewModel;
    }

    public DeletePopupViewModel() : base(new MainViewModel()) {}

    [RelayCommand]
    private async void Delete()
    {
        IsProcessing = true;
        
        var successes = 0;
        var warnings = 0;
        var errors = 0;

        foreach (var workspace in MainViewModel.Workspaces)
        {
            foreach (var report in workspace.SelectedReports)
            {
                if (!IsProcessing) return;
            
                report.Loading();
            
                Dataset dataset;
                try
                {
                    dataset = MainViewModel.Datasets[report.DatasetId!];
                    if (dataset.Name == report.Name && dataset.Workspace.Id == report.Workspace!.Id)
                    {
                        await DeleteDataset(report);
                        report.Success("Successfully deleted report and underlying dataset.");
                        MainViewModel.TotalSelectedReports--;
                        successes++;
                    }
                    else
                    {
                        await DeleteReport(report);
                        report.Warning("Report successfully deleted but failed to delete underlying dataset with a different name/workspace.");
                        MainViewModel.TotalSelectedReports--;
                        warnings++;
                    }
                }
                catch (Exception)
                {
                    await DeleteReport(report);
                    report.Warning("Report successfully deleted but failed to delete underlying dataset (No dataset or permissions to dataset).");
                    MainViewModel.TotalSelectedReports--;
                    warnings++;
                }
                report.IsSelected = false;
            }
        }
        
        ToastCommand(successes, warnings, errors).Execute(("Deleting finished!", $"{successes} successful, {warnings} warnings, {errors} errors."));
    }
    
    private async Task DeleteDataset(Report report)
    {
        var apiUrl = $"https://api.powerbi.com/v1.0/myorg/datasets/{report.DatasetId}";
        await MainViewModel.PowerShellService.BuildCommand()
            .WithCommand("Invoke-PowerBIRestMethod")
            .WithArguments(args => args
                .Add("-Url")
                .Add(apiUrl)
                .Add("-Method")
                .Add("Delete")
                .Add("-ErrorAction")
                .Add("Stop"))
            .WithStandardErrorPipe(Console.Error.WriteLine)
            .ExecuteAsync();
    }

    private async Task DeleteReport(Report report)
    {
        await MainViewModel.PowerShellService
            .BuildCommand()
            .WithCommand("Remove-PowerBIReport")
            .WithArguments(args => args
                .Add("-Id")
                .Add($"{report.Id}")
                .Add("-WorkspaceId")
                .Add($"{report.Workspace!.Id}")
            )
            .WithStandardErrorPipe(Console.Error.WriteLine)
            .ExecuteAsync();
    }

    public override void Close(Action? action = null)
    {
        base.Close(() =>
        {
            MainViewModel.ReloadWorkspacesCommand.Execute(MainViewModel.ShownWorkspaces);
        });
    }
}