using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AutoPBI.Controls;
using AutoPBI.Models;
using AutoPBI.Services;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoPBI.ViewModels.Popups;

public partial class ClonePopupViewModel : PopupViewModel
{
    [ObservableProperty] private bool _isCloning;
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
        var options = new FolderPickerOpenOptions
        {
            Title = "Select destination folder",
            AllowMultiple = false
        };

        var destinationFolder = await MainViewModel.DialogService.OpenFolderDialogAsync(options);
        if (destinationFolder == null) return;
        
        IsCloning = true;
        
        foreach (var report in MainViewModel.SelectedReports)
        {
            if (!IsCloning) return;
            report.Status = Report.StatusType.Loading;
        }
        
        Close();
    }
    
    [RelayCommand]
    private void Close()
    {
        IsVisible = false;
        
        if (!IsCloning) return;
        Console.Error.WriteLine("Clone stopped...");
        IsCloning =  false;
        foreach (var report in MainViewModel.SelectedReports)
        {
            report.Status = Report.StatusType.Selectable;
        }
    }
}