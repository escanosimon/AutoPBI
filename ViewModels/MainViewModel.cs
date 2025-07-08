using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using AutoPBI.Models;
using AutoPBI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AutoPBI.Views;



namespace AutoPBI.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<Workspace> _workspaces = [];
    [ObservableProperty] private ObservableCollection<Workspace> _selectedWorkspaces = [];
    [ObservableProperty] private ObservableCollection<Report> _selectedReports = [];
    
    [ObservableProperty] private bool _downloadModalIsOpen = false;
    
    [ObservableProperty] private string _email = "";
    [ObservableProperty] private string _password = "";

    
    // [ObservableProperty] private string _username = "russell.palma@amcsgroup.com";
    // [ObservableProperty] private string _password = "E519C9pDs!";
    // pakidelete lang boss, we even


    public ICommand LoginWindowCommand { get; }

    public MainViewModel()
    {
        _ps = new PsRunner();
        LoginWindowCommand = new RelayCommand(ShowLoginWindow);
    }

    private void ShowLoginWindow()
    {
        var loginWindow = new LoginPBI(this);
        loginWindow.Show();
    }

    private PsRunner _ps;

    [RelayCommand]
    public void Login()
    {
        var result = _ps.Execute(
            $"$password = '{Password}' | ConvertTo-SecureString -asPlainText -Force",
            $"$username = '{Email}'",
            "$credential = New-Object -TypeName System.Management.Automation.PSCredential -argumentlist $username, $password",
            "Connect-PowerBIServiceAccount -Credential $credential"
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
        var results = _ps.Execute("Get-PowerBIWorkspace -All");

        foreach (var obj in results.Objects)
        {
            var workspace = new Workspace(obj.Properties["Id"].Value.ToString(),
                obj.Properties["Name"].Value.ToString());
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
            var result = _ps.Execute($"Get-PowerBIReport -WorkspaceId '{workspace.Id}'");

            foreach (var obj in result.Objects)
            {
                try
                {
                    var report = new Report(    
                        obj.Properties["Id"].Value.ToString(),
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
        report.Workspace.CheckSelectedReports();
        
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