using System;
using System.IO;
using System.Threading.Tasks;
using AutoPBI.Models;
using AutoPBI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoPBI.ViewModels.Popups;

public abstract partial class PopupViewModel: ViewModelBase
{
    [ObservableProperty] private bool _isVisible;
    [ObservableProperty] private bool _isProcessing;
    [ObservableProperty] private MainViewModel _mainViewModel;

    protected PopupViewModel(MainViewModel mainViewModel)
    {
        IsVisible = false;
        MainViewModel = mainViewModel;
    }
    
    [RelayCommand]
    public virtual void Close(Action? whileProcessingAction = null)
    {
        IsVisible = false;

        foreach (var workspace in MainViewModel.Workspaces)
        {
            foreach (var report in workspace.SelectedReports)
            {
                report.Selectable();
                report.Message = false;
            }
        }
        
        if (!IsProcessing) return;
        whileProcessingAction?.Invoke();
        Console.Error.WriteLine("Process stopped...");
        IsProcessing =  false;
    }

    public IRelayCommand<object?> ToastCommand(int successes, int warnings, int errors)
    {
        return errors > 0
            ? (successes > 0 ? MainViewModel.WarningCommand : MainViewModel.ErrorCommand)
            : (warnings > 0 ? MainViewModel.WarningCommand : MainViewModel.SuccessCommand);
    }

    public async Task ExecuteDownload(Report report, string outputFile)
    {
        await MainViewModel.PowerShellService
            .BuildCommand()
            .WithCommand("Export-PowerBIReport")
            .WithArguments(args => args
                .Add("-Id")
                .Add($"{report.Id}")
                .Add("-OutFile")
                .Add($"{outputFile}")
            )
            .WithStandardErrorPipe(Console.Error.WriteLine)
            .ExecuteAsync();

        if (!Path.Exists(outputFile))
        {
            throw new FileNotFoundException("Failed to download report. Try downloading from Power BI Service.");
        }
    }

    public async Task ExecutePublish(string path, string name, Workspace workspace)
    {
        await MainViewModel.PowerShellService
            .BuildCommand()
            .WithCommand("New-PowerBIReport")
            .WithArguments(args => args
                .Add("-Path")
                .Add($"{path}")
                .Add("-Name")
                .Add($"{name}")
                .Add("-WorkspaceId")
                .Add($"{workspace.Id}")
                .Add("-ConflictAction")
                .Add("CreateOrOverwrite")
            )
            .WithStandardOutputPipe(Console.WriteLine)
            .WithStandardErrorPipe(Console.Error.WriteLine)
            .ExecuteAsync();
    }
}