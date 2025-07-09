using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AutoPBI.Controls;
using AutoPBI.Models;
using AutoPBI.Services;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoPBI.ViewModels.Popups;

public partial class ClonePopupViewModel : PopupViewModel
{
    [ObservableProperty] private User _user = null!;
    [ObservableProperty] private bool _isLoggedIn;
    [ObservableProperty] private ObservableCollection<Report> _selectedReports = [];
    [ObservableProperty] private ObservableCollection<PopupViewModel> _popups = [];
    [ObservableProperty] private DialogService _dialogService = new();
    [ObservableProperty] private PsRunner _ps = new();
    
    [ObservableProperty] private bool _isCloning;
    [ObservableProperty] private bool _isCloneShown;
    [ObservableProperty] private ObservableCollection<Workspace> _cloneWorkspaces = [];
    [ObservableProperty] private ObservableCollection<Workspace> _cloneSelectedWorkspaces = [];
    [ObservableProperty] private ObservableCollection<Report> _cloneSelectedReports = [];
    
    [ObservableProperty]
    private bool _canClone;

    private readonly PsRunner _ps1;

    public ClonePopupViewModel(MainViewModel mainViewModel) : base(mainViewModel)
    {
        MainViewModel = mainViewModel;
        _ps1 = MainViewModel.Ps; // Make sure we're using the same PowerShell runner as MainViewModel
        _ = InitializeCloneWorkspaces();
        InitializeSelectedReports();
        
        // Test PowerShell connection
        _ = TestPowerShellConnection();
    }

    private async Task TestPowerShellConnection()
    {
        Console.WriteLine("Testing PowerShell connection...");
        var result = await _ps1.Execute("Get-PowerBIWorkspace");
        Console.WriteLine($"PowerShell test result: {(result.Error.Count == 0 ? "Success" : "Failed")}");
        if (result.Error.Count > 0)
        {
            Console.WriteLine($"PowerShell test errors: {string.Join("\n", result.Error)}");
        }
    }
    
    private void InitializeSelectedReports()
    {
        CloneSelectedReports.Clear();
        foreach (var report in MainViewModel.SelectedReports)
        {
            CloneSelectedReports.Add(report);
        }
        UpdateCanClone();
    }


    private void UpdateCanClone()
    {
        CanClone = CloneSelectedWorkspaces.Count > 0 && CloneSelectedReports.Count > 0;
    }


    public ClonePopupViewModel() : base(new MainViewModel()) {}

    private async Task InitializeCloneWorkspaces()
    {
        await MainViewModel.FetchWorkspaces();
        foreach (var workspace in MainViewModel.Workspaces)
        {
            var cloneWorkspace = new Workspace(workspace.Id, workspace.Name);
            // Create new Reports collection
            cloneWorkspace.Reports = new ObservableCollection<Report>();
            // Deep copy each report
            foreach (var report in workspace.Reports)
            {
                var clonedReport = new Report(report.Id, report.Name, report.WebUrl, report.DatasetId, cloneWorkspace);
                cloneWorkspace.Reports.Add(clonedReport);
            }
            CloneWorkspaces.Add(cloneWorkspace);
        }
    }

    


    [RelayCommand]
    private async Task ShowWorkspaces()
    {
        IsCloneShown = true;
        if (CloneWorkspaces.Count == 0)
        {
            await InitializeCloneWorkspaces();
        }
    }




    [RelayCommand]
    private void ShowReports()
    {
        IsCloneShown = false;
        // Refresh the selected reports when switching to reports view
        InitializeSelectedReports();
    }


   private async Task<bool> CopyReport(Report report, string targetWorkspaceId)
{
    try
    {
        Console.WriteLine($"=== Starting Copy Report Debug ===");
        Console.WriteLine($"Report ID: {report.Id}");
        Console.WriteLine($"Report Name: {report.Name}");
        Console.WriteLine($"Dataset ID: {report.DatasetId}");
        Console.WriteLine($"Source Workspace ID: {report.Workspace.Id}");
        Console.WriteLine($"Target Workspace ID: {targetWorkspaceId}");
        
        // Updated command using the documented syntax, including Name and TargetDatasetId
        var command = $"Copy-PowerBIReport -Name '{report.Name}' -Id '{report.Id}' -WorkspaceId '{report.Workspace.Id}' -TargetWorkspaceId '{targetWorkspaceId}' -TargetDatasetId '{report.DatasetId}'";
        Console.WriteLine($"Command to execute: {command}");
        
        var result = await MainViewModel.Ps.Execute(command);
        
        Console.WriteLine("=== PowerShell Response ===");
        Console.WriteLine($"Error Count: {result.Error.Count}");
        Console.WriteLine($"Error Messages: {string.Join("\n", result.Error)}");
        Console.WriteLine($"Output Count: {result.Output.Count}");
        Console.WriteLine($"Output Messages: {string.Join("\n", result.Output)}");
        
        if (result.Error.Count > 0)
        {
            // Try to get more detailed error information
            var errorResult = await MainViewModel.Ps.Execute("$Error[0] | Format-List -Force");
            Console.WriteLine("=== Detailed Error Information ===");
            Console.WriteLine(string.Join("\n", errorResult.Output));
        }
        
        return result.Error.Count == 0;
    }
    catch (Exception ex)
    {
        Console.WriteLine("=== Exception Thrown ===");
        Console.WriteLine($"Exception Type: {ex.GetType().Name}");
        Console.WriteLine($"Message: {ex.Message}");
        Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        return false;
    }
    finally
    {
        Console.WriteLine("=== End Copy Report Debug ===\n");
    }
}



    [RelayCommand]
    private async Task Clone()
    {
        Console.WriteLine($"=== Clone Operation Started ===");
        Console.WriteLine($"Selected Workspaces: {CloneSelectedWorkspaces.Count}");
        Console.WriteLine($"Selected Reports: {CloneSelectedReports.Count}");
        
        if (CloneSelectedWorkspaces.Count == 0 || CloneSelectedReports.Count == 0)
        {
            Console.WriteLine("No workspaces or reports selected, returning");
            return;
        }

        IsCloning = true;

        try
        {
            foreach (var targetWorkspace in CloneSelectedWorkspaces)
            {
                Console.WriteLine($"\nProcessing target workspace: {targetWorkspace.Name} ({targetWorkspace.Id})");
                
                foreach (var report in CloneSelectedReports)
                {
                    if (!IsCloning)
                    {
                        Console.WriteLine("Cloning cancelled");
                        return;
                    }
                    
                    Console.WriteLine($"\nCopying report: {report.Name} ({report.Id})");
                    report.Status = Report.StatusType.Loading;
                    var success = await CopyReport(report, targetWorkspace.Id);
                    report.Status = success ? Report.StatusType.Success : Report.StatusType.Error;
                    Console.WriteLine($"Copy operation {(success ? "succeeded" : "failed")}");
                }
            }
        }
        finally
        {
            Console.WriteLine("\n=== Clone Operation Completed ===");
            IsCloning = false;
            Close();
        }
    }

    
    public void test()
    {
        PsRunner ps2 = new PsRunner();
        // example-workspace
        // bcba8d13-d417-4608-b1c6-ec94d3650c1b
        //
        // example-report
        // 6a92e428-479e-4138-b242-ee27a00e27da
        //
        // Copy-PowerBIReport -Name "Copy Test" -Id "6a92e428-479e-4138-b242-ee27a00e27da" -TargetWorkspaceId "bcba8d13-d417-4608-b1c6-ec94d3650c1b"
        ps2.Execute("Copy-PowerBIReport -Name \"Copy Test\" -Id \"6a92e428-479e-4138-b242-ee27a00e27da\" -TargetWorkspaceId \"bcba8d13-d417-4608-b1c6-ec94d3650c1b\"");

    }
    
    // private async void FetchWorkspaces()
    // {
    //     var result = await Ps.Execute("Get-PowerBIWorkspace -All");
    //
    //     foreach (var workspace in result.Objects.Select(obj => new Workspace(obj.Properties["Id"].Value.ToString(),
    //                  obj.Properties["Name"].Value.ToString())))
    //     {
    //         Workspaces.Add(workspace);
    //     }
    // }
    //
    [RelayCommand]
    private void SelectWorkspace(Workspace workspace)
    {
        workspace.IsSelected = !workspace.IsSelected;

        if (workspace.IsSelected)
        {
            CloneSelectedWorkspaces.Add(workspace);
        }
        else
        {
            CloneSelectedWorkspaces.Remove(workspace);
        }
    
        UpdateCanClone();
    }




    
    

    
    [RelayCommand]
    private void Close()
    {
        IsVisible = false;
        
        // Clear selections when closing
        foreach (var workspace in CloneSelectedWorkspaces.ToList())
        {
            workspace.IsSelected = false;
            CloneSelectedWorkspaces.Remove(workspace);
        }
        
        // Reset all workspace selections
        foreach (var workspace in CloneWorkspaces)
        {
            workspace.IsSelected = false;
        }
        
        // Reset report statuses
        foreach (var report in CloneSelectedReports)
        {
            report.Status = Report.StatusType.Selectable;
        }
        
        if (!IsCloning) return;
        Console.Error.WriteLine("Clone stopped...");
        IsCloning = false;
    }



}