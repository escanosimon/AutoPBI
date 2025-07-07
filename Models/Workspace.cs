using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoPBI.Models
{
    public class Workspace : ObservableObject
    {
        private string? _id;
        private string? _name;
        private bool? _isSelected;
        private bool? _isAllReportsSelected;
        private ObservableCollection<Report> _reports = [];
        
        public Workspace(string? id, string? name)
        {
            Id = id;
            Name = name;
            IsSelected = false;
            IsAllReportsSelected = false;
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