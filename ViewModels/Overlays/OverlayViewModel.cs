using System;
using AutoPBI.ViewModels.Popups;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoPBI.ViewModels.Overlays;

public partial class OverlayViewModel : ViewModelBase
{
    [ObservableProperty] private bool _isOpen;
    [ObservableProperty] private PopupViewModel _popupViewModel;
    [ObservableProperty] private IRelayCommand? _command;
    [ObservableProperty] private string? _buttonText;

    public OverlayViewModel(PopupViewModel popupViewModel, IRelayCommand? command = null, string? buttonText = null)
    {
        _popupViewModel = popupViewModel;
        PopupViewModel = popupViewModel;
        if (command != null)
        {
            Command = command;
        }
        if (buttonText != null)
        {
            ButtonText = buttonText;
        }
    }

    [RelayCommand]
    public void Execute()
    {
        Command?.Execute(null);
        Close();
    }

    [RelayCommand]
    public void Close()
    {
        IsOpen = false;
    }
}