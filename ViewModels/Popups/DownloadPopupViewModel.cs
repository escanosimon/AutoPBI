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
    [ObservableProperty] private bool _isDownloading;
    
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
        
        IsDownloading = true;
        
        foreach (var report in MainViewModel.SelectedReports)
        {
            if (!IsDownloading)
            {
                Close();
                return;
            }
            report.Status = Report.StatusType.Loading;
            var outputFile = $"{destinationFolder}/{report.Name}.pbix";
            var result = await MainViewModel.Ps.Execute(
                @$"if (Test-Path '{outputFile}') {{
                    Remove-Item '{outputFile}' -Force
                }}",
                $"Export-PowerBIReport -Id '{report.Id}' -OutFile  '{outputFile}'"
            );
            
            report.Status = result.Error.Count == 0 ? Report.StatusType.Success : Report.StatusType.Error;
        }
        
        Close();
    }
    
    [RelayCommand]
    private void Close()
    {
        IsVisible = false;
        
        if (!IsDownloading) return;
        Console.Error.WriteLine("Download stopped...");
        IsDownloading =  false;
        foreach (var report in MainViewModel.SelectedReports)
        {
            report.Status = Report.StatusType.Selectable;
        }
    }
}