using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using AutoPBI.Models;
using AutoPBI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoPBI.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<Workspace> _workspaces = [];
    [ObservableProperty] private string _username = "simon.escano@amcsgroup.com";
    [ObservableProperty] private string _password = "Onacsenomis8_";

    private PsRunner _ps;
    
    public MainViewModel()
    {
        _ps = new PsRunner();
    }

    [RelayCommand]
    public void Login()
    {
        var result = _ps.Execute(
            $"$password = '{Password}' | ConvertTo-SecureString -asPlainText -Force",
            $"$username = '{Username}'",
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
            FetchReports();
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
    
    private void FetchReports()
    {
        foreach (var workspace in Workspaces)
        {
            var result = _ps.Execute($"Get-PowerBIReport -WorkspaceId '{workspace.Id}'");

            foreach (var obj in result.Objects)
            {
                try
                {
                    var report = new Report(obj.Properties["Id"].Value.ToString(),
                        obj.Properties["Name"].Value.ToString(),
                        obj.Properties["WebUrl"].Value.ToString(),
                        obj.Properties["DatasetId"].Value.ToString());
                    workspace.Reports.Add(report);
                }
                catch (Exception e)
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
}