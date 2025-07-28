using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoPBI.ViewModels.Popups;

public partial class ClonePopupViewModel : PopupViewModel
{
    [ObservableProperty] private bool _isWorkspacesShown;
    
    public ClonePopupViewModel(MainViewModel mainViewModel) : base(mainViewModel)
    {
        MainViewModel = mainViewModel;
    }

    public ClonePopupViewModel() : base(new MainViewModel()) {}

    [RelayCommand]
    private void ShowWorkspaces()
    {
        IsWorkspacesShown = true;
    }

    [RelayCommand]
    private void ShowReports()
    {
        IsWorkspacesShown = false;
    }

    [RelayCommand]
    private async void Clone()
    {
        if (MainViewModel.SelectedWorkspaces.Count == 0)
        {
            MainViewModel.WarningCommand.Execute(("Cannot proceed with cloning.", "Please select at least one workspace"));
            return;
        }
        
        IsProcessing = true;
        ShowReports();
        
        var errors = 0;
        var warnings = 0;
        var successes = 0;

        foreach (var workspace in MainViewModel.Workspaces)
        {
            foreach (var report in workspace.SelectedReports)
            {
                foreach (var selectedWorkspace in MainViewModel.SelectedWorkspaces)
                {
                    if (!IsProcessing) return;
                    report.Loading();

                    if (report.Workspace!.Id == selectedWorkspace.Id)
                    {
                        report.Error("Cannot clone report to the same workspace.");
                        errors++;
                    }
                    else
                    {
                        try
                        {
                            var result = await MainViewModel.PowerShellService.BuildCommand()
                                .WithCommand("Copy-PowerBIReport")
                                .WithArguments(args => args
                                    .Add("-Name")
                                    .Add($"{report.Name} - Copy")
                                    .Add("-Id")
                                    .Add($"{report.Id}")
                                    .Add("-TargetWorkspaceId")
                                    .Add($"{selectedWorkspace.Id}")
                                    .Add("-TargetDatasetId")
                                    .Add($"{report.DatasetId}")
                                )
                                .WithStandardErrorPipe(Console.Error.WriteLine)
                                .ExecuteAsync();
                        }
                        catch (Exception e)
                        {
                            report.Error(e.Message);
                            errors++;
                            continue;
                        }

                        report.Success($"Successfully cloned report to {selectedWorkspace.Name}");
                        successes++;
                    }
                }

                if (errors > 0)
                {
                    if (successes > 0)
                    {
                        report.Warning("Report failed to clone to some target workspaces.");
                    }
                    else
                    {
                        report.Error("Report failed to clone to any target workspaces.");
                    }
                }
                else
                {
                    report.Success("Successfully cloned report to target workspaces.");
                }
            }
        }

        ToastCommand(successes, warnings, errors).Execute(("Cloning finished!", $"{successes} successful, {warnings} warnings, {errors} errors."));
    }

    public override void Close(Action? whileProcessingAction = null)
    {
        base.Close(() =>
        {
            MainViewModel.ReloadWorkspacesCommand.Execute(MainViewModel.SelectedWorkspaces);
        });
    }
}