using System.Collections.ObjectModel;
using System.Linq;
using AutoPBI.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoPBI.Models
{
    public partial class Workspace : ObservableObject
    {
        private string? _id;
        private string? _name;
        private bool? _isShown;
        private bool? _isSelected;
        private bool? _isAllReportsSelected;
        private bool? _isLoading;
        private bool? _isDroppedDown;
        private ObservableCollection<Report> _reports = [];
        private MainViewModel? _mainViewModel;

        public Workspace(string? id, MainViewModel? mainViewModel)
        {
            Id = id;
            Name = null;
            MainViewModel = mainViewModel;
            IsShown = false;
            IsSelected = false;
            IsLoading = true;
            IsAllReportsSelected = false;
        }

        [RelayCommand]
        public void Show()
        {
            MainViewModel!.ShowWorkspaceCommand.Execute(this);
        }

        [RelayCommand]
        public void Select()
        {
            MainViewModel!.SelectWorkspaceCommand.Execute(this);
        }

        public void CheckSelectedReports()
        {
            if (Reports.Any(report => !report.IsSelected))
            {
                IsAllReportsSelected = false;
                return;
            }

            IsAllReportsSelected = true;
        }
        
        public MainViewModel? MainViewModel
        {
            get => _mainViewModel;
            set => SetProperty(ref _mainViewModel, value);
        }
        
        public string? Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string? Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        
        public bool IsShown
        {
            get => (bool)_isShown!;
            set => SetProperty(ref _isShown, value);
        }
        
        public bool IsSelected
        {
            get => (bool)_isSelected!;
            set => SetProperty(ref _isSelected, value);
        }
        
        public bool IsAllReportsSelected
        {
            get => (bool)_isAllReportsSelected!;
            set => SetProperty(ref _isAllReportsSelected, value);
        }
        
        public bool IsLoading
        {
            get => (bool)_isLoading!;
            set => SetProperty(ref _isLoading, value);
        }
        
        public bool IsDroppedDown
        {
            get => (bool)_isDroppedDown!;
            set => SetProperty(ref _isDroppedDown, value);
        }

        public ObservableCollection<Report> Reports
        {
            get => _reports;
            set => SetProperty(ref _reports, value);
        }
    }
}