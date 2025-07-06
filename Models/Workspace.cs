using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoPBI.Models
{
    public class Workspace : ObservableObject
    {
        private string? _id;
        private string? _name;
        private List<Report> _reports = [];

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

        public List<Report> Reports
        {
            get => _reports;
            set => SetProperty(ref _reports, value);
        }

        public Workspace(string? id, string? name)
        {
            Id = id;
            _name = name;
            _id = id;
            Name = name;
        }
    }
}