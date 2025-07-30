using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using AutoPBI.Models;
using AutoPBI.Services;
using AutoPBI.ViewModels.Overlays;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoPBI.ViewModels.Popups;

public abstract partial class PopupViewModel: ViewModelBase
{
    [ObservableProperty] private Psr _psr = new();
    [ObservableProperty] private bool _isOpen;
    [ObservableProperty] private bool _isProcessing;
    [ObservableProperty] private MainViewModel _mainViewModel;
    [ObservableProperty] private ObservableCollection<OverlayViewModel> _overlays = [];
    [ObservableProperty] private OverlayViewModel _reportsSummaryOverlay;

    protected PopupViewModel(MainViewModel mainViewModel)
    {
        IsOpen = false;
        MainViewModel = mainViewModel;
        ReportsSummaryOverlay = AddOverlay(new OverlayViewModel(this));
    }

    public OverlayViewModel AddOverlay(OverlayViewModel overlay)
    {
        Overlays.Add(overlay);
        return overlay;
    }
    
    [RelayCommand]
    public virtual void OpenOverlay(OverlayViewModel selectedOverlay)
    {
        foreach (var overlay in Overlays)
        {
            overlay.IsOpen = overlay == selectedOverlay;
        }
    }
    
    [RelayCommand]
    public virtual void Close(Action? whileProcessingAction = null)
    {
        IsOpen = false;

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
        await Psr
            .Wrap()
            .WithArguments(args => args.Add("Export-PowerBIReport"))
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
        await Psr
            .Wrap()
            .WithArguments(args => args.Add("New-PowerBIReport"))
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