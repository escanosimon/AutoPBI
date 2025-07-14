using System;
using AutoPBI.Models;
using CommunityToolkit.Mvvm.Input;

namespace AutoPBI.ViewModels.Popups;

public partial class DeletePopupViewModel : PopupViewModel
{
    public DeletePopupViewModel(MainViewModel mainViewModel) : base(mainViewModel)
    {
        MainViewModel = mainViewModel;
    }

    public DeletePopupViewModel() : base(new MainViewModel()) {}

    [RelayCommand]
    private async void Delete()
    {
        IsProcessing = true;
        
        foreach (var report in MainViewModel.SelectedReports)
        {
            if (!IsProcessing) return;
            
            report.Status = Report.StatusType.Loading;

            var result = await MainViewModel.PowerShellService
                .BuildCommand()
                .WithCommand("Remove-PowerBIReport")
                .WithArguments(args => args
                    .Add("-Id")
                    .Add($"{report.Id}")
                    .Add("-WorkspaceId")
                    .Add($"{report.Workspace!.Id}")
                )
                .WithStandardErrorPipe(Console.Error.WriteLine)
                .ExecuteAsync();
            
            report.Status = result.Error.Count == 0 ? Report.StatusType.Success : Report.StatusType.Error;
            report.IsSelected = false;
        }

        MainViewModel.SelectedReports.Clear();
        foreach (var workspace in MainViewModel.ShownWorkspaces)
        {
            workspace.Reports.Clear();
            MainViewModel.FetchReportsCommand.Execute(workspace);
        }
        
    }
}