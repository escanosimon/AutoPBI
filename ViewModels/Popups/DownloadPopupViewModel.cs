using System;
using System.Threading.Tasks;
using AutoPBI.Services;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;

namespace AutoPBI.ViewModels.Popups;

public partial class DownloadPopupViewModel : PopupViewModel
{
    public DownloadPopupViewModel(MainViewModel mainViewModel, DialogService dialogService) : base(mainViewModel, dialogService)
    {
        MainViewModel = mainViewModel;
        DialogService = dialogService;
    }

    public DownloadPopupViewModel() : base(new MainViewModel(), new DialogService()) {}

    [RelayCommand]
    private void Close()
    {
        IsVisible = false;
    }

    [RelayCommand]
    private async void Download()
    {
        var options = new FolderPickerOpenOptions()
        {
            Title = "Select destination folder",
            AllowMultiple = false
        };

        var destinationFolder = await DialogService.OpenFolderDialogAsync(options);
        if (destinationFolder == null) return;

        foreach (var report in MainViewModel.SelectedReports)
        {
            var outputFile = $"{destinationFolder}/{report.Name}.pbix";
            var result = MainViewModel.Ps.Execute(
                @$"if (Test-Path '{outputFile}') {{
                    Remove-Item '{outputFile}' -Force
                }}",
                $"Export-PowerBIReport -Id '{report.Id}' -OutFile  '{outputFile}'"
            );
            
        }
    }
}