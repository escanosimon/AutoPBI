using System;
using System.IO;
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
        
        var tempFolderPath = GetTempFolderPath();

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
                            var name = $"{report.Name} - Copy";
                            var outputFile = Path.Combine(tempFolderPath, $"{name}.pbix");
                            await ExecuteDownload(report, outputFile);
                            await ExecutePublish(outputFile, name, selectedWorkspace);
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

        DeleteTempFolder(tempFolderPath);
        ToastCommand(successes, warnings, errors).Execute(("Cloning finished!", $"{successes} successful, {warnings} warnings, {errors} errors."));
    }

    private static string GetTempFolderPath()
    {
        var tempFolderPath = Path.Combine(Path.GetTempPath(), $"PowerBiTemp_{Guid.NewGuid()}");
        if (!Directory.Exists(tempFolderPath))
        {
            Directory.CreateDirectory(tempFolderPath);
        }
        return tempFolderPath;
    }

    private static void DeleteTempFolder(string tempFolderPath)
    {
        if (!Directory.Exists(tempFolderPath)) return;
        try
        {
            Directory.Delete(tempFolderPath, true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to delete temporary folder: {ex.Message}");
        }
    }

    public override void Close(Action? whileProcessingAction = null)
    {
        base.Close(() =>
        {
            MainViewModel.ReloadWorkspacesCommand.Execute(MainViewModel.SelectedWorkspaces);
        });
    }
}