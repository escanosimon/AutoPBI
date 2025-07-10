using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoPBI.Controls;
using AutoPBI.Models;
using AutoPBI.Services;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using AvaloniaEdit.Document;
using CliWrap;
using CliWrap.Buffered;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using CommunityToolkit.Mvvm.Input;
using CommandResult = AutoPBI.Services.CommandResult;

namespace AutoPBI.ViewModels.Popups;

public partial class PublishPopupViewModel : PopupViewModel
{
    [ObservableProperty] private bool _isPublishing;
    [ObservableProperty] private bool _isWorkspacesShown;
    [ObservableProperty] private bool _importButtonVisibility;
    [ObservableProperty] private bool _importedReportsVisibility;
    [ObservableProperty] private ObservableCollection<Report> _importedReports = [];
    
    public PublishPopupViewModel(MainViewModel mainViewModel) : base(mainViewModel)
    {
        MainViewModel = mainViewModel;
        UpdateVisibilities();
    }

    public PublishPopupViewModel() : base(new MainViewModel()) {}

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
            var report = new Report(null, selectedFile, null, null, null);
            ImportedReports.Add(report);
            report.Status = Report.StatusType.Selectable;
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
        if (ImportedReports.Count == 0 || MainViewModel.SelectedWorkspaces.Count == 0) return;
        
        IsPublishing = true;
        ShowImportedReports();
        
        foreach (var report in ImportedReports)
        {
            report.Status = Report.StatusType.Loading;
            var path = report.Name!;
            var name = Path.GetFileNameWithoutExtension(path);
            List<string> reportErrors = [];
            var reportSuccesses = 0;
            foreach (var workspace in MainViewModel.SelectedWorkspaces)
            {
                if (!IsPublishing) return;
                
                CommandResult result;
                try
                {
                    result = await MainViewModel.PowerShellService
                        .BuildCommand()
                        .WithCommand("New-PowerBIReport")
                        .WithArguments(args => args
                            .Add("-Path")
                            .Add($"{path}")
                            .Add("-Name")
                            .Add($"{name}")
                            .Add("-WorkspaceId")
                            .Add($"{workspace.Id}")
                            .Add("-ConflictAction")
                            .Add("CreateOrOverwrite")
                        )
                        .WithStandardOutputPipe(Console.WriteLine)
                        .WithStandardErrorPipe(Console.Error.WriteLine)
                        .ExecuteAsync();
                }
                catch (Exception e)
                {
                    reportErrors.Add(e.Message);
                    Console.Error.WriteLine(e.Message);
                    continue;
                }
                if (result.Error.Count > 0)
                {
                    reportErrors = result.Error.ToList();
                }
                else
                {
                    reportSuccesses++;
                }
            }

            if (reportErrors.Count > 0)
            {
                report.Status = reportSuccesses > 0 ? Report.StatusType.Warning : Report.StatusType.Error;
            }
            else
            {
                report.Status =  Report.StatusType.Success;
            }
        }
    }
    
    [RelayCommand]
    private void Close()
    {
        IsVisible = false;
        
        if (!IsPublishing) return;
        Console.Error.WriteLine("Publish stopped...");
        IsPublishing =  false;
        foreach (var workspace in MainViewModel.SelectedWorkspaces)
        {
            MainViewModel.FetchReportsCommand.Execute(workspace);
        }
        foreach (var report in MainViewModel.SelectedReports)
        {
            report.Status = Report.StatusType.Selectable;
        }
    }

    private void UpdateVisibilities()
    {
        ImportButtonVisibility = !IsWorkspacesShown && (ImportedReports.Count == 0);
        ImportedReportsVisibility = IsWorkspacesShown && (ImportedReports.Count != 0);
    }
}