using System;
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

namespace AutoPBI.ViewModels.Popups;

public partial class PublishPopupViewModel : PopupViewModel
{
    [ObservableProperty] private bool _isPublishing;
    [ObservableProperty] private bool _isWorkspacesShown;
    [ObservableProperty] private bool _importButtonVisibility;
    [ObservableProperty] private bool _selectedPbixFilesVisibility;
    [ObservableProperty] private ObservableCollection<string> _selectedPbixFilePaths = [];
    
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
    private void ShowPbixFiles()
    {
        IsWorkspacesShown = false;
        UpdateVisibilities();
    }

    [RelayCommand]
    private async void ImportPbixFiles()
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
        var enumerable = selectedFiles as string[] ?? selectedFiles.ToArray();
        if (!enumerable!.Any())
        {
            SelectedPbixFilePaths.Clear();
            UpdateVisibilities();
            return;
        }

        // Ensure that selectedFiles consists of strings, or access the appropriate property
        SelectedPbixFilePaths = new ObservableCollection<string>(
            enumerable!.Where(file => file.EndsWith(".pbix", StringComparison.OrdinalIgnoreCase)));
        UpdateVisibilities();
    }

    [RelayCommand]
    private void SelectPublishWorkspace(Workspace? selectedWorkspace)
    {
        MainViewModel.SelectWorkspaceCommand?.Execute(selectedWorkspace);
    }

    [RelayCommand]
    private async void Publish()
    {
        if (SelectedPbixFilePaths.Count == 0)
        {
            Close();
            return;
        };
        
        IsPublishing = true;
        ShowPbixFiles();
        
        foreach (var report in MainViewModel.SelectedReports)
        {
            if (!IsPublishing) return;
            
            report.Status = Report.StatusType.Loading;
            
            var username = "simon.escano@amcsgroup.com";
            var password = "Onacsenomis8_";
            var tabularEditorPath = "C:/Users/simon.escano/Downloads/TabularEditor.2.26.0/TabularEditor.exe";

            try
            {
                await Cli.Wrap(tabularEditorPath)
                    .WithArguments(args => args
                        .Add(
                            $"Data Source=powerbi://api.powerbi.com/v1.0/myorg/{report.Workspace!.Name};User ID={username};Password={password}")
                        .Add($"{report.DatasetId}")
                        .Add("-S")
                        .Add($"{SelectedPbixFilePaths}")
                        .Add("-D")
                    )
                    .WithStandardOutputPipe(PipeTarget.ToDelegate(Console.WriteLine))
                    .ExecuteAsync();
            }
            catch (Exception e)
            {
                report.Status = Report.StatusType.Error;
                continue;
            }
            report.Status = Report.StatusType.Success;
        }
        
        Close();
    }
    
    [RelayCommand]
    private void Close()
    {
        IsVisible = false;
        
        if (!IsPublishing) return;
        Console.Error.WriteLine("Publish stopped...");
        IsPublishing =  false;
        foreach (var report in MainViewModel.SelectedReports)
        {
            report.Status = Report.StatusType.Selectable;
        }
    }

    private void UpdateVisibilities()
    {
        ImportButtonVisibility = !IsWorkspacesShown && (SelectedPbixFilePaths.Count == 0);
        SelectedPbixFilesVisibility = IsWorkspacesShown && (SelectedPbixFilePaths.Count != 0);
    }
}