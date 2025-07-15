using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AutoPBI.Models;
using AutoPBI.Services;
using AutoPBI.ViewModels.Popups;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoPBI.ViewModels;


public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private User _user = null!;
    [ObservableProperty] private bool _isLoggedIn;

    [ObservableProperty] private bool _isReloading = true;
    
    [ObservableProperty] private ObservableCollection<Workspace> _workspaces = [];
    [ObservableProperty] private ObservableHashMap<string, Dataset> _datasets = [];
    [ObservableProperty] private ObservableCollection<Workspace> _shownWorkspaces = [];
    [ObservableProperty] private ObservableCollection<Workspace> _selectedWorkspaces = [];
    [ObservableProperty] private ObservableCollection<Report> _selectedReports = [];
    [ObservableProperty] private ObservableCollection<PopupViewModel> _popups = [];

    [ObservableProperty] private bool _isAllWorkspacesShown;
    [ObservableProperty] private bool _isAllWorkspacesSelected;
    
    [ObservableProperty] private PopupViewModel _downloadPopup;
    [ObservableProperty] private PopupViewModel _scriptPopup;
    [ObservableProperty] private PopupViewModel _clonePopup;
    [ObservableProperty] private PopupViewModel _scanPopup;
    [ObservableProperty] private PopupViewModel _publishPopup;
    [ObservableProperty] private PopupViewModel _deletePopup;
    [ObservableProperty] private PopupViewModel _loginPopup;
    
    [ObservableProperty] private DialogService _dialogService = new();
    [ObservableProperty] private PowerShellService _powerShellService = new();

    public MainViewModel()
    {
        DownloadPopup = AddPopup(new DownloadPopupViewModel(this));
        ScriptPopup = AddPopup(new ScriptPopupViewModel(this));
        ClonePopup = AddPopup(new ClonePopupViewModel(this));
        ScanPopup = AddPopup(new ScanPopupViewModel(this));
        PublishPopup = AddPopup(new PublishPopupViewModel(this));
        DeletePopup = AddPopup(new DeletePopupViewModel(this));
        LoginPopup = AddPopup(new LoginPopupViewModel(this));
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
    private async Task FetchWorkspaces()
    {
        var result = await PowerShellService
            .BuildCommand()
            .WithCommand("Get-PowerBIWorkspace")
            .WithArguments(args => args
                .Add("-All")
            )
            .WithStandardErrorPipe(Console.Error.WriteLine)
            .ExecuteAsync();

        foreach (var obj in result.Objects)
        {
            var workspace = new Workspace(obj.Properties["Id"].Value.ToString(), this);
            Workspaces.Add(workspace);
            await FetchReports(workspace);
            await FetchDatasets(workspace);
            workspace.Name = obj.Properties["Name"].Value.ToString();
            workspace.IsLoading = false;
        }
    }

    [RelayCommand]
    private void ShowWorkspace(Workspace workspace)
    {
        workspace.IsShown = !workspace.IsShown;
        CheckShownWorkspaces();

        if (workspace.IsShown)
        {
            ShownWorkspaces.Add(workspace);
        }
        else
        {
            ShownWorkspaces.Remove(workspace);
            foreach (var report in workspace.Reports)
            {
                report.IsSelected = false;
                SelectedReports.Remove(report);
            }
        }
    }

    [RelayCommand]
    private void CheckShownWorkspaces()
    {
        if (Workspaces.Any(workspace => !workspace.IsShown))
        {
            IsAllWorkspacesShown = false;
            return;
        }

        IsAllWorkspacesShown = true;
    }
    
    [RelayCommand]
    private void CheckSelectedWorkspaces()
    {
        if (Workspaces.Any(workspace => !workspace.IsSelected))
        {
            IsAllWorkspacesSelected = false;
            return;
        }

        IsAllWorkspacesSelected = true;
    }
    
    [RelayCommand]
    private void SelectWorkspace(Workspace workspace)
    {
        workspace.IsSelected = !workspace.IsSelected;
        CheckSelectedWorkspaces();

        if (workspace.IsSelected)
        {
            SelectedWorkspaces.Add(workspace);
        }
        else
        {
            SelectedWorkspaces.Remove(workspace);
        }
    }

    [RelayCommand]
    private async void ReloadWorkspaces(ObservableCollection<Workspace> workspaces)
    {
        IsReloading = true;
        SelectedReports.Clear();
        foreach (var workspace in workspaces)
        {
            workspace.Reports.Clear();
            await FetchReports(workspace);
            await FetchDatasets(workspace);
        }
        IsReloading = false;
    }

    [RelayCommand]
    private async Task FetchReports(Workspace workspace)
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
        foreach (var obj in result.Objects)
        {
            try
            {
                var report = new Report(obj.Properties["Id"].Value?.ToString(),
                    obj.Properties["Name"].Value?.ToString(),
                    obj.Properties["WebUrl"].Value?.ToString(),
                    obj.Properties["DatasetId"].Value?.ToString(),
                    workspace
                );
                workspace.Reports.Add(report);
            }
            catch (Exception)
            {
                Console.Error.WriteLine("Error report: " + obj.Properties["Name"].Value);
            }
        }
        workspace.CheckSelectedReports();
    }

    [RelayCommand]
    private async Task FetchDatasets(Workspace workspace)
    {
        var result = await PowerShellService.BuildCommand()
            .WithCommand("Get-PowerBIDataset")
            .WithArguments(args => args
                .Add("-WorkspaceId")
                .Add($"{workspace.Id}")
            )
            .WithStandardErrorPipe(Console.WriteLine)
            .ExecuteAsync();
        foreach (var obj in result.Objects)
        {
            try
            {
                Datasets[obj.Properties["Id"].Value.ToString()!] = new Dataset(
                    obj.Properties["Id"].Value.ToString()!,
                    obj.Properties["Name"].Value.ToString()!,
                    obj.Properties["webUrl"].Value.ToString()!,
                    (bool)obj.Properties["IsRefreshable"].Value,
                    obj.Properties["CreatedDate"].Value.ToString()!,
                    workspace, (string )obj.Properties["ConfiguredBy"].Value);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"{obj.Properties["Name"].Value}: {e.Message}");
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

    [RelayCommand]
    private void ShowAllWorkspaces()
    {
        IsAllWorkspacesShown = !IsAllWorkspacesShown;

        if (IsAllWorkspacesShown)
        {
            foreach (var workspace in Workspaces)
            {
                if (workspace.IsShown) continue;
                workspace.IsShown = true;
                ShownWorkspaces.Add(workspace);
            }
        }
        else
        {
            foreach (var workspace in Workspaces)
            {
                if (!workspace.IsShown) continue;
                workspace.IsShown = false;
                ShownWorkspaces.Remove(workspace);
            }
        }
    }
    
    [RelayCommand]
    private void SelectAllWorkspaces()
    {
        IsAllWorkspacesSelected = !IsAllWorkspacesSelected;

        if (IsAllWorkspacesSelected)
        {
            foreach (var workspace in Workspaces)
            {
                if (workspace.IsSelected) continue;
                workspace.IsSelected = true;
                SelectedWorkspaces.Add(workspace);
            }
        }
        else
        {
            foreach (var workspace in Workspaces)
            {
                if (!workspace.IsSelected) continue;
                workspace.IsSelected = false;
                SelectedWorkspaces.Remove(workspace);
            }
        }
    }
}