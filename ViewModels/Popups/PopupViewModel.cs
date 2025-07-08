using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoPBI.ViewModels.Popups;

public abstract partial class PopupViewModel: ViewModelBase
{
    [ObservableProperty] private bool _isVisible;
    [ObservableProperty] private MainViewModel _mainViewModel;

    protected PopupViewModel(MainViewModel mainViewModel)
    {
        IsVisible = false;
        MainViewModel = mainViewModel;
    }
}