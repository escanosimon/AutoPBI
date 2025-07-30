using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoPBI.Controls;
using AutoPBI.Models;
using AutoPBI.Services;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoPBI.ViewModels.Popups;

public partial class DownloadPopupViewModel : PopupViewModel
{
    [ObservableProperty] private bool _skipExisting;
    
    public DownloadPopupViewModel(MainViewModel mainViewModel) : base(mainViewModel)
    {
        MainViewModel = mainViewModel;
    }

    public DownloadPopupViewModel() : base(new MainViewModel()) {}

    [RelayCommand]
    private void ToggleSkipExisting()
    {
        SkipExisting = !SkipExisting;
    }

    [RelayCommand]
    private async void Download()
    {
        var options = new FolderPickerOpenOptions
        {
            Title = "Select destination folder",
            AllowMultiple = false
        };

        var destinationFolder = await MainViewModel.DialogService.OpenFolderDialogAsync(options);
        if (destinationFolder == null)
        {
            MainViewModel.WarningCommand.Execute(("Cannot proceed with download!", "Destination folder is null."));
            return;
        }
        
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
                var outputFile = $"{destinationFolder}/{report.Name}.pbix";

                if (File.Exists(outputFile))
                {
                    if (SkipExisting)
                    {
                        Console.WriteLine($"Skipping {outputFile}");
                        report.Success("Skipped report");
                        successes++;
                        continue;
                    }
                    File.Delete(outputFile);
                    Console.WriteLine($"Replacing {outputFile}");
                }

                try
                {
                    await ExecuteDownload(report, outputFile);
                }
                catch (OperationCanceledException)
                {
                    SetReportsSelectable();
                    MainViewModel.Toast(Toast.StatusType.Normal, "Download cancelled!", $"Last to download: {report.Name}");
                    return;
                }
                catch (Exception e)
                {
                    report.Error(e.Message);
                    errors++;
                    continue;
                }

                report.Success("Successfully downloaded report");
                successes++;
            }
        }
        
        IsProcessing = false;
        ToastCommand(successes, warnings, errors).Execute(("Downloading finished!", $"{successes} successful, {warnings} warnings, {errors} errors."));
    }
}