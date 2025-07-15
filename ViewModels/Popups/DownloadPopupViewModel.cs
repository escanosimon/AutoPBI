using System;
using System.Threading.Tasks;
using AutoPBI.Models;
using AutoPBI.Services;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoPBI.ViewModels.Popups;

public partial class DownloadPopupViewModel : PopupViewModel
{
    public DownloadPopupViewModel(MainViewModel mainViewModel) : base(mainViewModel)
    {
        MainViewModel = mainViewModel;
    }

    public DownloadPopupViewModel() : base(new MainViewModel()) {}

    [RelayCommand]
    private async void Download()
    {
        var options = new FolderPickerOpenOptions
        {
            Title = "Select destination folder",
            AllowMultiple = false
        };

        var destinationFolder = await MainViewModel.DialogService.OpenFolderDialogAsync(options);
        if (destinationFolder == null) return;
        
        IsProcessing = true;
        
        foreach (var report in MainViewModel.SelectedReports)
        {
            if (!IsProcessing) return;
            report.Loading();
            var outputFile = $"{destinationFolder}/{report.Name}.pbix";

            try
            {
                await MainViewModel.PowerShellService
                    .BuildCommand()
                    .WithCommand($"if (Test-Path '{outputFile}') {{ Remove-Item '{outputFile}' -Force }}")
                    .ExecuteAsync();
                await MainViewModel.PowerShellService
                    .BuildCommand()
                    .WithCommand("Export-PowerBIReport")
                    .WithArguments(args => args
                        .Add("-Id")
                        .Add($"{report.Id}")
                        .Add("-OutFile")
                        .Add($"{outputFile}")
                    )
                    .WithStandardErrorPipe(Console.Error.WriteLine)
                    .ExecuteAsync();
            }
            catch (Exception e)
            {
                report.Error(e.Message);
                continue;
            }

            report.Success("Successfully downloaded report");
        }
    }
}