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
        private bool? _isSelected;
        private bool? _isAllReportsSelected;
        private ObservableCollection<Report> _reports = [];
        private MainViewModel? _mainViewModel;
        
        public Workspace(string? id, string? name, MainViewModel mainViewModel)
        {
            Id = id;
            Name = name;
            IsSelected = false;
            IsAllReportsSelected = false;
            MainViewModel = mainViewModel;
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

        public ObservableCollection<Report> Reports
        {
            get => _reports;
            set => SetProperty(ref _reports, value);
        }
    }
}