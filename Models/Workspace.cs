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
        private bool? _isShownSearched;
        private bool? _isSelectedSearched;
        private bool? _isSelected;
        private bool? _isAllReportsSelected;
        private bool? _isLoading;
        
        private bool? _isDroppedDown;
        private object? _dropDownIconUnicode;
        
        private ObservableCollection<Report> _reports = [];
        private ObservableCollection<Report> _selectedReports = [];
        private MainViewModel? _mainViewModel;

        public Workspace(string? id, MainViewModel? mainViewModel)
        {
            Id = id;
            Name = null;
            MainViewModel = mainViewModel;
            IsShown = false;
            IsShownSearched = true;
            IsSelected = false;
            IsLoading = true;
            
            IsDroppedDown = true;
            DropDownIconUnicode = "\uf078";
            
            IsAllReportsSelected = false;
        }
        
        [RelayCommand]
        private void SelectAllReports()
        {
            IsAllReportsSelected = !IsAllReportsSelected;

            if (IsAllReportsSelected)
            {
                foreach (var report in Reports)
                {
                    if (!report.IsSearched) continue;
                    if (report.IsSelected) continue;
                    report.IsSelected = true;
                    SelectedReports.Add(report);
                    MainViewModel!.TotalSelectedReports++;
                }
            }
            else
            {
                foreach (var report in Reports)
                {
                    if (!report.IsSelected) continue;
                    report.IsSelected = false;
                    SelectedReports.Remove(report);
                    MainViewModel!.TotalSelectedReports--;
                }
            }
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

        [RelayCommand]
        public void DropDown()
        {
            IsDroppedDown = !IsDroppedDown;
            DropDownIconUnicode = IsDroppedDown ? "\uf078" : "\uf054";
        }

        public void CheckSelectedReports()
        {
            var searchedReports = Reports.Where(r => r.IsSearched).ToList();
            IsAllReportsSelected = searchedReports.Count != 0 && searchedReports.All(r => r.IsSelected);
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
        
        public bool IsShownSearched
        {
            get => (bool)_isShownSearched!;
            set => SetProperty(ref _isShownSearched, value);
        }
        
        public bool IsSelectedSearched
        {
            get => (bool)_isSelectedSearched!;
            set => SetProperty(ref _isSelectedSearched, value);
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
        
        public object? DropDownIconUnicode
        {
            get => _dropDownIconUnicode!;
            set => SetProperty(ref _dropDownIconUnicode, value);
        }

        public ObservableCollection<Report> Reports
        {
            get => _reports;
            set => SetProperty(ref _reports, value);
        }
        
        public ObservableCollection<Report> SelectedReports
        {
            get => _selectedReports;
            set => SetProperty(ref _selectedReports, value);
        }
    }
}