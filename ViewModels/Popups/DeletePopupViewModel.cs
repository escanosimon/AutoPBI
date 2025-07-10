using System;
using System.Threading.Tasks;
using AutoPBI.Models;
using AutoPBI.Services;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoPBI.ViewModels.Popups;

public partial class DeletePopupViewModel : PopupViewModel
{
    [ObservableProperty] private bool _isDeleting;
    
    public DeletePopupViewModel(MainViewModel mainViewModel) : base(mainViewModel)
    {
        MainViewModel = mainViewModel;
    }

    public DeletePopupViewModel() : base(new MainViewModel()) {}

    [RelayCommand]
    private async void Delete()
    {
        IsDeleting = true;
        
        foreach (var report in MainViewModel.SelectedReports)
        {
            if (!IsDeleting) return;
            
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
        }

        MainViewModel.FetchReportsCommand.Execute(null);
    }
    
    [RelayCommand]
    private void Close()
    {
        IsVisible = false;
        
        if (!IsDeleting) return;
        Console.Error.WriteLine("Delete stopped...");
        IsDeleting =  false;
        foreach (var report in MainViewModel.SelectedReports)
        {
            report.Status = Report.StatusType.Selectable;
        }
    }
}