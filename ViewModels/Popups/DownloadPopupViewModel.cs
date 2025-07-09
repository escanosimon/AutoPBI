using CommunityToolkit.Mvvm.Input;

namespace AutoPBI.ViewModels.Popups;

public partial class DownloadPopupViewModel(MainViewModel mainViewModel) : PopupViewModel(mainViewModel)
{
    // Design-time constructor
    public DownloadPopupViewModel() : this(new MainViewModel())
    {
        // This is for design-time only
    }

    [RelayCommand]
    private void Close()
    {
        IsVisible = false;
    }
}