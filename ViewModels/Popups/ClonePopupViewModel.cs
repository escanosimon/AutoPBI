using System.Collections.ObjectModel;
using AutoPBI.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoPBI.ViewModels.Popups;

public partial class ClonePopupViewModel(MainViewModel mainViewModel) : PopupViewModel(mainViewModel)
{
    [ObservableProperty] 
    private ObservableCollection<Workspace> _workspaces = [];

    // Design-time constructor
    public ClonePopupViewModel() : this(new MainViewModel())
    {
        // This is for design-time only
    }

    [RelayCommand]
    private void Close()
    {
        IsVisible = false;
    }
    
   

    [RelayCommand]
    private void FetchReports()
    {
        MainViewModel.FetchReportsCommand.Execute(null);
    }
}