using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoPBI.Controls;
using AutoPBI.Models;
using AutoPBI.ViewModels.Overlays;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoPBI.ViewModels.Popups;

public partial class DeletePopupViewModel : PopupViewModel
{
    [ObservableProperty] private OverlayViewModel _confirmationOverlay;
    
    public DeletePopupViewModel(MainViewModel mainViewModel) : base(mainViewModel)
    {
        MainViewModel = mainViewModel;
        ConfirmationOverlay = AddOverlay(new OverlayViewModel(this, DeleteCommand, "Yes, delete", "This action will attempt to delete the report and its underlying dataset (if they share the same name and workspace). Do you wish to continue?"));
    }

    public DeletePopupViewModel() : base(new MainViewModel()) {}

    [RelayCommand]
    private async void Delete()
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
                catch (OperationCanceledException)
                {
                    SetReportsSelectable();
                    MainViewModel.Toast(Toast.StatusType.Normal, "Delete cancelled!", $"Last to delete: {report.Name}");
                    return;
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
        RestartCts();
        var apiUrl = $"https://api.powerbi.com/v1.0/myorg/datasets/{report.DatasetId}";
        await Psr.Wrap()
            .WithArguments(args => args.Add("Invoke-PowerBIRestMethod"))
            .WithArguments(args => args
                .Add("-Url")
                .Add(apiUrl)
                .Add("-Method")
                .Add("Delete")
                .Add("-ErrorAction")
                .Add("Stop"))
            .WithStandardErrorPipe(Console.Error.WriteLine)
            .ExecuteAsync(Cts.Token);
    }

    private async Task DeleteReport(Report report)
    {
        RestartCts();
        await Psr
            .Wrap()
            .WithArguments(args => args.Add("Remove-PowerBIReport"))
            .WithArguments(args => args
                .Add("-Id")
                .Add($"{report.Id}")
                .Add("-WorkspaceId")
                .Add($"{report.Workspace!.Id}")
            )
            .WithStandardErrorPipe(Console.Error.WriteLine)
            .ExecuteAsync(Cts.Token);
    }

    public override void Close(Action? action = null)
    {
        base.Close(() =>
        {
            MainViewModel.ReloadWorkspacesCommand.Execute(MainViewModel.ShownWorkspaces);
        });
    }
}