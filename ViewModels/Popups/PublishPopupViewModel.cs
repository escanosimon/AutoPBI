using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using AutoPBI.Models;
using AutoPBI.ViewModels.Overlays;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommandResult = AutoPBI.Services.CommandResult;

namespace AutoPBI.ViewModels.Popups;

public partial class PublishPopupViewModel : PopupViewModel
{
    [ObservableProperty] private bool _isWorkspacesShown;
    [ObservableProperty] private bool _importButtonVisibility;
    [ObservableProperty] private bool _importedReportsVisibility;
    [ObservableProperty] private ObservableCollection<Report> _importedReports = [];
    [ObservableProperty] private OverlayViewModel _selectedWorkspacesOverlay;
    
    public PublishPopupViewModel(MainViewModel mainViewModel) : base(mainViewModel)
    {
        MainViewModel = mainViewModel;
        UpdateVisibilities();
        SelectedWorkspacesOverlay = AddOverlay(new OverlayViewModel(this, PublishCommand, "I'm sure, publish"));
    }

    public PublishPopupViewModel() : base(new MainViewModel()) {}
    
    public override void OpenOverlay(OverlayViewModel selectedOverlay)
    {
        if (ImportedReports.Count == 0 || MainViewModel.SelectedWorkspaces.Count == 0)
        {
            MainViewModel.WarningCommand.Execute(("Cannot publish!", "Select at least one workspace and one report to publish."));
            return;
        }
        base.OpenOverlay(selectedOverlay);
    }

    [RelayCommand]
    private void ShowWorkspaces()
    {
        IsWorkspacesShown = true;
        UpdateVisibilities();
    }

    [RelayCommand]
    private void ShowImportedReports()
    {
        IsWorkspacesShown = false;
        UpdateVisibilities();
    }

    [RelayCommand]
    private async void ImportReports()
    {
        var options = new FilePickerOpenOptions
        {
            Title = "Select Power BI Files",
            AllowMultiple = true,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("Power BI Files")
                {
                    Patterns = ["*.pbix"]
                }
            }
        };

        var selectedFiles = await MainViewModel.DialogService.OpenFileDialogAsync(options);
        foreach (var selectedFile in selectedFiles)
        {
            var report = new Report(MainViewModel, null, selectedFile, null, null, null);
            ImportedReports.Add(report);
            report.Selectable();
            report.IsSelected = true;
        }
        UpdateVisibilities();
    }

    [RelayCommand]
    private void SelectPublishWorkspace(Workspace? selectedWorkspace)
    {
        MainViewModel.ShowWorkspaceCommand?.Execute(selectedWorkspace);
    }

    [RelayCommand]
    private async void Publish()
    {
        IsProcessing = true;
        ShowImportedReports();
        
        var successes = 0;
        var warnings = 0;
        var errors = 0;
        
        foreach (var report in ImportedReports)
        {
            report.Loading();
            var path = report.Name!;
            var name = Path.GetFileNameWithoutExtension(path);
            List<string> reportErrors = [];
            var reportSuccesses = 0;
            
            foreach (var workspace in MainViewModel.SelectedWorkspaces)
            {
                if (!IsProcessing) return;
                
                try
                {
                    await ExecutePublish(path, name, workspace);
                }
                catch (Exception e)
                {
                    reportErrors.Add(e.Message);
                    continue;
                }
                reportSuccesses++;
            }

            if (reportErrors.Count > 0)
            {
                if (reportSuccesses > 0)
                {
                    report.Warning("Report was not published in some target workspaces.");
                    warnings++;
                }
                else
                {
                    report.Error("Report was not published to any target workspaces.");
                    errors++;
                }
            }
            else
            {
                report.Success("Successfully published to all target workspaces.");
                successes++;
            }
        }
        
        IsProcessing = false;
        ToastCommand(successes, warnings, errors).Execute(("Publishing finished!", $"{successes} successful, {warnings} warnings, {errors} errors."));
    }
    
    private void UpdateVisibilities()
    {
        ImportButtonVisibility = !IsWorkspacesShown && (ImportedReports.Count == 0);
        ImportedReportsVisibility = IsWorkspacesShown && (ImportedReports.Count != 0);
    }

    public override void Close(Action? whileProcessingAction = null)
    {
        ImportedReports.Clear();
        IsWorkspacesShown = false;
        UpdateVisibilities();
        base.Close(() =>
        {
            MainViewModel.ReloadWorkspacesCommand.Execute(MainViewModel.SelectedWorkspaces);
        });
    }
}