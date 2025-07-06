using System;
using System.Collections.ObjectModel;
using AutoPBI.Models;
using AutoPBI.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoPBI.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<Workspace> _workspaces;
    [ObservableProperty] private string _username = "simon.escano@amcsgroup.com";
    [ObservableProperty] private string _password = "Onacsenomis8_";

    private PsRunner _ps;
    
    public MainViewModel()
    {
        _workspaces = new ObservableCollection<Workspace>();
        _ps = new PsRunner();
        Login();
        FetchWorkspaces();
    }

    public void Login()
    {
        var result = _ps.Execute(
            $"$password = '{Password}' | ConvertTo-SecureString -asPlainText -Force",
            $"$username = '{Username}'",
            "$credential = New-Object -TypeName System.Management.Automation.PSCredential -argumentlist $username, $password",
            "$account = Connect-PowerBIServiceAccount -Credential $credential"
        );
    }

    private void FetchWorkspaces()
    {
        var exec2 = _ps.Execute("$workspaces = Get-PowerBIWorkspace -All");
        var results = _ps.Execute("$workspaces");

        foreach (var obj in results.Objects)
        {
            Workspaces.Add(new Workspace(obj.Properties["Id"].Value.ToString(), obj.Properties["Name"].Value.ToString()));
        }
    }
}