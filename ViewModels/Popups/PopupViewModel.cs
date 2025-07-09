using AutoPBI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoPBI.ViewModels.Popups;

public abstract partial class PopupViewModel: ViewModelBase
{
    [ObservableProperty] private bool _isVisible;
    [ObservableProperty] private MainViewModel _mainViewModel;
    [ObservableProperty] private IDialogService _dialogService = null!;

    protected PopupViewModel(MainViewModel mainViewModel)
    {
        IsVisible = true;
        MainViewModel = mainViewModel;
    }
    
}