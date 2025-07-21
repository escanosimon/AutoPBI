using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using AutoPBI.Controls;
using AutoPBI.Models;
using AutoPBI.Services;
using AutoPBI.ViewModels.Popups;
using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Styling;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NeoSmart.SecureStore;
using Path = System.IO.Path;

namespace AutoPBI.ViewModels;


public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private string _exePath;
    [ObservableProperty] private string _toolsFolder;
    
    [ObservableProperty] private DispatcherTimer _timer;

    [ObservableProperty] private User _user = null!;
    [ObservableProperty] private bool _isLoggedIn;
    [ObservableProperty] private bool _hasSavedLoginInfo;

    [ObservableProperty] private bool _isReloading = true;
    
    [ObservableProperty] private string _workspaceSearchText;
    [ObservableProperty] private string _reportSearchText;
    
    [ObservableProperty] private bool _isToasting;
    [ObservableProperty] private Toast.StatusType _toastStatus;
    [ObservableProperty] private string _toastTitle;
    [ObservableProperty] private string _toastDescription;
    
    [ObservableProperty] private ObservableCollection<Workspace> _workspaces = [];
    [ObservableProperty] private ObservableHashMap<string, Dataset> _datasets = [];
    [ObservableProperty] private ObservableCollection<Workspace> _shownWorkspaces = [];
    [ObservableProperty] private ObservableCollection<Workspace> _selectedWorkspaces = [];
    [ObservableProperty] private int _totalSelectedReports = 0;
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
        ExePath = Assembly.GetExecutingAssembly().Location;
        ToolsFolder = Path.Combine(Path.GetDirectoryName(ExePath)!, "Tools");

        try
        {
            var (username, password) = CheckSavedLogin();
            AutoLogin(username, password);
            HasSavedLoginInfo = true;
        }
        catch (Exception)
        {
            HasSavedLoginInfo = false;
        }

        InitializePopups();

        Timer = new DispatcherTimer();
        Timer.Tick += (s, e) =>
        {
            CloseToast();
        };
    }

    [RelayCommand]
    private void ToggleTheme()
    {
        if (Application.Current!.ActualThemeVariant == ThemeVariant.Light)
        {
            Application.Current.RequestedThemeVariant = ThemeVariant.Dark;
        }
        else
        {
            Application.Current.RequestedThemeVariant = ThemeVariant.Light;
        }
    }

    private async Task InitializeMicrosoftPowerBIMgmt()
    {
        try
        {
            await PowerShellService.BuildCommand().WithCommand($@"
            if (-not (Get-Module -ListAvailable -Name MicrosoftPowerBIMgmt)) {{
                Install-Module -Name MicrosoftPowerBIMgmt -Scope CurrentUser
                Import-Module MicrosoftPowerBIMgmt
            }}
            ").ExecuteAsync();
        }
        catch (Exception)
        {
            Console.Error.WriteLine("Failed to initialize Module.");
        }
    }

    private void InitializePopups()
    {
        DownloadPopup = AddPopup(new DownloadPopupViewModel(this));
        ScriptPopup = AddPopup(new ScriptPopupViewModel(this));
        ClonePopup = AddPopup(new ClonePopupViewModel(this));
        ScanPopup = AddPopup(new ScanPopupViewModel(this));
        PublishPopup = AddPopup(new PublishPopupViewModel(this));
        DeletePopup = AddPopup(new DeletePopupViewModel(this));
        LoginPopup = AddPopup(new LoginPopupViewModel(this));
    }

    partial void OnWorkspaceSearchTextChanged(string value)
    {
        foreach (var workspace in Workspaces)
        {
            if (string.IsNullOrEmpty(value))
            {
                workspace.IsSearched = true;
            }
            else
            {
                workspace.IsSearched = workspace.Name?.Contains(value, StringComparison.OrdinalIgnoreCase) ?? false;
            }
        }
        CheckShownWorkspaces();
    }
    
    partial void OnReportSearchTextChanged(string value)
    {
        foreach (var workspace in ShownWorkspaces)
        {
            foreach (var report in workspace.Reports)
            {
                if (string.IsNullOrEmpty(value))
                {
                    report.IsSearched = true;
                }
                else
                {
                    report.IsSearched = report.Name?.Contains(value, StringComparison.OrdinalIgnoreCase) ?? false;
                }
            }
            workspace.CheckSelectedReports();
        }
    }

    private (string username, string password) CheckSavedLogin()
    {
        var credentials = SecureStorageService.LoadSavedCredentials();
        if (!credentials.HasValue) throw new Exception("No saved credentials found.");
            
        var (username, password) = credentials.Value;
        return (username, password);
    }

    private async void AutoLogin(string username, string password)
    {
        await Login(username, password);
        await FetchWorkspaces();
    }

    public async Task Login(string username, string password)
    {
        PSObject loginResult;
        try
        {
            loginResult = (await PowerShellService
                .BuildCommand()
                .WithCommand($@"
                $password = '{password}' | ConvertTo-SecureString -asPlainText -Force;
                $username = '{username}';
                $credential = New-Object -TypeName System.Management.Automation.PSCredential -argumentlist $username, $password;
                Connect-PowerBIServiceAccount -Credential $credential
            ")
                .WithStandardErrorPipe(Console.Error.WriteLine)
                .ExecuteAsync()).Objects[0];
        }
        catch (Exception e)
        {
            Error(("Login failed!", e.Message));
            throw;
        }
        var accessTokenResult = (await PowerShellService
            .BuildCommand()
            .WithCommand("(Get-PowerBIAccessToken).Values[0]")
            .WithStandardErrorPipe(Console.Error.WriteLine)
            .ExecuteAsync()).Objects[0];

        User = new User(
            loginResult.Properties["Environment"].Value.ToString(),
            loginResult.Properties["TenantId"].Value.ToString(),
            username,
            password
        );

        User.AccessToken = (string)accessTokenResult.BaseObject;
        
        Success(("Login successful!", "Fetching workspaces..."));
        IsLoggedIn = true;
        IsReloading = false;
    }

    [RelayCommand]
    private async void Logout()
    {
        try
        {
            await PowerShellService.BuildCommand()
                .WithCommand("Disconnect-PowerBIServiceAccount")
                .ExecuteAsync();
            TotalSelectedReports = 0;
            Workspaces = [];
            Datasets = [];
            ShownWorkspaces = [];
            SelectedWorkspaces = [];
            User = null!;
            IsLoggedIn = false;
            SecureStorageService.ClearSavedCredentials();
            
            Success(("Success!", "Logged out successfully."));
        }
        catch (Exception)
        {
            Error(("Failed to log out!", "Something went wrong."));
        }
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
        IsReloading = true;
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

        IsReloading = false;
        Toast(Controls.Toast.StatusType.Normal, "Finished fetching workspaces!", "Select a workspace to show reports.");
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
                workspace.SelectedReports.Remove(report);
            }
            workspace.CheckSelectedReports();
        }
    }

    [RelayCommand]
    private void CheckShownWorkspaces()
    {
        var searchedWorkspaces = Workspaces.Where(w => w.IsSearched).ToList();
        IsAllWorkspacesShown = searchedWorkspaces.Count != 0 && searchedWorkspaces.All(w => w.IsSelected);
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
        foreach (var workspace in workspaces)
        {
            workspace.Reports.Clear();
            workspace.SelectedReports.Clear();
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
                var report = new Report(
                    this,
                    obj.Properties["Id"].Value?.ToString(),
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
    private void ShowAllWorkspaces()
    {
        IsAllWorkspacesShown = !IsAllWorkspacesShown;

        if (IsAllWorkspacesShown)
        {
            foreach (var workspace in Workspaces)
            {
                if (!workspace.IsSearched) continue;
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

    private void Toast(Toast.StatusType statusType, string title, string description)
    {
        ToastStatus = statusType;
        ToastTitle = title;
        ToastDescription = description;
        
        IsToasting = true;
        Timer.Stop();
        Timer.Interval = TimeSpan.FromSeconds(5);
        Timer.Start();
    }

    [RelayCommand]
    private void Success(object? parameter)
    {
        if (parameter is (string title, string description))
        {
            Toast(Controls.Toast.StatusType.Success, title, description);
        }
    }
    
    [RelayCommand]
    private void Error(object? parameter)
    {
        if (parameter is (string title, string description))
        {
            Toast(Controls.Toast.StatusType.Error, title, description);
        }
    }
    
    [RelayCommand]
    private void Warning(object? parameter)
    {
        if (parameter is (string title, string description))
        {
            Toast(Controls.Toast.StatusType.Warning, title, description);
        }
    }

    [RelayCommand]
    private void CloseToast()
    {
        IsToasting = false;
        ToastTitle = "";
        ToastDescription = "";
        Timer.Stop();
    }
}