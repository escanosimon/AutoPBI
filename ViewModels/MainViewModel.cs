using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using AutoPBI.Models;
using AutoPBI.Services;
using AutoPBI.ViewModels.Popups;
using CliWrap;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoPBI.ViewModels;


public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private User _user = null!;
    [ObservableProperty] private bool _isLoggedIn;
    
    [ObservableProperty] private ObservableCollection<Workspace> _workspaces = [];
    [ObservableProperty] private ObservableCollection<Workspace> _selectedWorkspaces = [];
    [ObservableProperty] private ObservableCollection<Report> _selectedReports = [];
    [ObservableProperty] private ObservableCollection<PopupViewModel> _popups = [];
    
    [ObservableProperty] private PopupViewModel _downloadPopup;
    [ObservableProperty] private PopupViewModel _scriptPopup;
    [ObservableProperty] private PopupViewModel _clonePopup;
    [ObservableProperty] private PopupViewModel _scanPopup;
    [ObservableProperty] private PopupViewModel _publishPopup;
    
    [ObservableProperty] private DialogService _dialogService = new();
    [ObservableProperty] private PowerShellService _powerShellService = new();

    public MainViewModel()
    {
        DownloadPopup = AddPopup(new DownloadPopupViewModel(this));
        ScriptPopup = AddPopup(new ScriptPopupViewModel(this));
        ClonePopup = AddPopup(new ClonePopupViewModel(this));
        ScanPopup = AddPopup(new ScanPopupViewModel(this));
        PublishPopup = AddPopup(new PublishPopupViewModel(this));
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
    private async void Login()
    {
        var result = await PowerShellService
            .BuildCommand()
            .WithCommand("Login-PowerBI")
            .WithStandardErrorPipe(Console.Error.WriteLine)
            .ExecuteAsync();

        if (result.Error.Count != 0) return;
        foreach (var obj in result.Objects)
        {
            User = new User(
                obj.Properties["Environment"].Value.ToString(),
                obj.Properties["TenantId"].Value.ToString(),
                obj.Properties["UserName"].Value.ToString());
        }

        IsLoggedIn = true;
        FetchWorkspaces();
    }
    
    private async void FetchWorkspaces()
    {
        var result = await PowerShellService
            .BuildCommand()
            .WithCommand("Get-PowerBIWorkspace")
            .WithArguments(args => args
                .Add("-All")
            )
            .WithStandardErrorPipe(Console.Error.WriteLine)
            .ExecuteAsync();
        
        foreach (var workspace in result.Objects.Select(obj => new Workspace(obj.Properties["Id"].Value.ToString(),
                     obj.Properties["Name"].Value.ToString(), this)))
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
            Console.Error.WriteLine($"Workspace: " + workspace.Id);
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
    private async void FetchReports()
    {
        if (SelectedWorkspaces.Count == 0) return;
        foreach (var workspace in SelectedWorkspaces)
        {
            var result = await PowerShellService
                .BuildCommand()
                .WithCommand("Get-PowerBIReport")
                .WithArguments(args => args
                    .Add("-WorkspaceId")
                    .Add($"{workspace.Id}")
                )
                .WithStandardErrorPipe(Console.Error.WriteLine)
                .ExecuteAsync();

            workspace.Reports.Clear();
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
            workspace.CheckSelectedReports();
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
            Console.Error.WriteLine($"Report: " + report.DatasetId);
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