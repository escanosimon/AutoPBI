using System;
using AutoPBI.Models;
using AutoPBI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoPBI.ViewModels.Popups;

public abstract partial class PopupViewModel: ViewModelBase
{
    [ObservableProperty] private bool _isVisible;
    [ObservableProperty] private bool _isProcessing;
    [ObservableProperty] private MainViewModel _mainViewModel;

    protected PopupViewModel(MainViewModel mainViewModel)
    {
        IsVisible = false;
        MainViewModel = mainViewModel;
    }
    
    [RelayCommand]
    public virtual void Close(Action? whileProcessingAction = null)
    {
        IsVisible = false;
        foreach (var report in MainViewModel.SelectedReports)
        {
            report.Selectable();
        }
        
        if (!IsProcessing) return;
        whileProcessingAction?.Invoke();
        Console.Error.WriteLine("Process stopped...");
        IsProcessing =  false;
    }

    public IRelayCommand<object?> ToastCommand(int successes, int warnings, int errors)
    {
        return errors > 0
            ? (successes > 0 ? MainViewModel.WarningCommand : MainViewModel.ErrorCommand)
            : (warnings > 0 ? MainViewModel.WarningCommand : MainViewModel.SuccessCommand);
    }
}