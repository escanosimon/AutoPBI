using System;
using System.Collections.ObjectModel;
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
    [ObservableProperty] private bool _isWorkspacesShown;
    
    public ClonePopupViewModel(MainViewModel mainViewModel) : base(mainViewModel)
    {
        MainViewModel = mainViewModel;
    }

    public ClonePopupViewModel() : base(new MainViewModel()) {}

    [RelayCommand]
    private void ShowWorkspaces()
    {
        IsWorkspacesShown = true;
    }

    [RelayCommand]
    private void ShowReports()
    {
        IsWorkspacesShown = false;
    }

    [RelayCommand]
    private async void Clone()
    {
        if (MainViewModel.SelectedWorkspaces.Count == 0) return;
        
        IsProcessing = true;
        ShowReports();
        
        foreach (var report in MainViewModel.SelectedReports)
        {
            var errors = 0;
            var successes = 0;
            foreach (var workspace in MainViewModel.SelectedWorkspaces)
            {
                if (!IsProcessing) return;
                report.Loading();

                try
                {
                    var result = await MainViewModel.PowerShellService.BuildCommand()
                        .WithCommand("Copy-PowerBIReport")
                        .WithArguments(args => args
                            .Add("-Name")
                            .Add($"{report.Name} - Copy")
                            .Add("-Id")
                            .Add($"{report.Id}")
                            .Add("-TargetWorkspaceId")
                            .Add($"{workspace.Id}")
                            .Add("-TargetDatasetId")
                            .Add($"{report.DatasetId}")
                        )
                        .WithStandardErrorPipe(Console.Error.WriteLine)
                        .ExecuteAsync();
                }
                catch (Exception e)
                {
                    report.Error(e.Message);
                    errors++;
                    continue;
                }

                report.Success($"Successfully cloned report to {workspace.Name}");
                successes++;
            }

            if (errors > 0)
            {
                if (successes > 0)
                {
                    report.Warning("Report failed to clone to some selected workspaces.");
                }
                else
                {
                    report.Error("Report failed to clone to any selected workspaces.");
                }
            }
            else
            {
                report.Success("Successfully cloned report to selected workspaces.");
            }
        }
    }
}