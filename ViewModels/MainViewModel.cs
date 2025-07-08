using System;
using System.Collections.ObjectModel;
using System.Linq;
using AutoPBI.Models;
using AutoPBI.Services;
using AutoPBI.ViewModels.Popups;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoPBI.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<Workspace> _workspaces = [];
    [ObservableProperty] private ObservableCollection<Workspace> _selectedWorkspaces = [];
    [ObservableProperty] private ObservableCollection<Report> _selectedReports = [];
    [ObservableProperty] private ObservableCollection<PopupViewModel> _popups = [];
    [ObservableProperty] private PopupViewModel _downloadPopup;
    
    [ObservableProperty] private DialogService _dialogService = new();
    [ObservableProperty] private PsRunner _ps = new();

    public MainViewModel()
    {
        DownloadPopup = AddPopup(new DownloadPopupViewModel(this));
    }

    private PopupViewModel AddPopup(PopupViewModel popup)
    {
        Popups.Add(popup);
        return popup;
    }

    [RelayCommand]
    private void OpenPopup(PopupViewModel selectedPopup)
    {
        foreach (var popup in Popups)
        {
            popup.IsVisible = popup == selectedPopup;
        }
    }
    
    [RelayCommand]
    private void Login()
    {
        var result = Ps.Execute(
            "Connect-PowerBIServiceAccount"
        );
        if (result.Error.Count > 0)
        {
            Console.Error.WriteLine(result.Error.ToString());
        }
        else
        {
            FetchWorkspaces();
        }
    }
    
    private void FetchWorkspaces()
    {
        var result = Ps.Execute("Get-PowerBIWorkspace -All");

        foreach (var workspace in result.Objects.Select(obj => new Workspace(obj.Properties["Id"].Value.ToString(),
                     obj.Properties["Name"].Value.ToString())))
        {
            Workspaces.Add(workspace);
        }
    }

    [RelayCommand]
    private void SelectWorkspace(Workspace workspace)
    {
        workspace.IsSelected = !workspace.IsSelected;

        if (workspace.IsSelected)
        {
            SelectedWorkspaces.Add(workspace);
        }
        else
        {
            SelectedWorkspaces.Remove(workspace);
            foreach (var report in workspace.Reports)
            {
                report.IsSelected = false;
                SelectedReports.Remove(report);
            }
        }
    }
    
    [RelayCommand]
    private void FetchReports()
    {
        if (SelectedWorkspaces.Count == 0) return;
        foreach (var workspace in SelectedWorkspaces)
        {
            var result = Ps.Execute($"Get-PowerBIReport -WorkspaceId '{workspace.Id}'");

            foreach (var obj in result.Objects)
            {
                try
                {
                    var report = new Report(obj.Properties["Id"].Value.ToString(),
                        obj.Properties["Name"].Value.ToString(),
                        obj.Properties["WebUrl"].Value.ToString(),
                        obj.Properties["DatasetId"].Value.ToString(),
                        workspace
                        );
                    workspace.Reports.Add(report);
                }
                catch (Exception)
                {
                    Console.Error.WriteLine("--------ERROR--------");
                    foreach (var prop in obj.Properties)
                    {
                        Console.Error.WriteLine($"{prop.Name}: {prop.Value}");
                    }
                }
            }
        }
    }
    
    [RelayCommand]
    private void SelectReport(Report report)
    {
        report.IsSelected = !report.IsSelected;
        report.Workspace?.CheckSelectedReports();
        
        if (report.IsSelected)
        {
            SelectedReports.Add(report);
        }
        else
        {
            SelectedReports.Remove(report);
        }
    }

    [RelayCommand]
    private void SelectAllReports(Workspace workspace)
    {
        workspace.IsAllReportsSelected = !workspace.IsAllReportsSelected;

        if (workspace.IsAllReportsSelected)
        {
            foreach (var report in workspace.Reports)
            {
                if (report.IsSelected) continue;
                report.IsSelected = true;
                SelectedReports.Add(report);
            }
        }
        else
        {
            foreach (var report in workspace.Reports)
            {
                if (!report.IsSelected) continue;
                report.IsSelected = false;
                SelectedReports.Remove(report);
            }
        }
    }
}