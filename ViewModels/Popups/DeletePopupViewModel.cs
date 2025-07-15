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
            
            report.Loading();

            try
            {
                await MainViewModel.PowerShellService
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
            }
            catch (Exception e)
            {
                report.Error(e.Message);
                continue;
            }
            
            report.Success("Successfully deleted report");
            report.IsSelected = false;
        }

        MainViewModel.ReloadWorkspacesCommand.Execute(MainViewModel.ShownWorkspaces);
    }
}