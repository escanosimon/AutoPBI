using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AutoPBI.Controls;
using AutoPBI.Models;
using AutoPBI.Services;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using AvaloniaEdit.Document;
using CliWrap;
using CliWrap.Buffered;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using CommunityToolkit.Mvvm.Input;

namespace AutoPBI.ViewModels.Popups;

public partial class ScriptPopupViewModel : PopupViewModel
{
    [ObservableProperty] private bool _isScripting;
    [ObservableProperty] private bool _isScriptShown;
    [ObservableProperty] private bool _importButtonVisibility;
    [ObservableProperty] private bool _editScriptVisibility;
    [ObservableProperty] private string? _selectedScriptPath;
    [ObservableProperty] private TextDocument _scriptContents;
    
    public ScriptPopupViewModel(MainViewModel mainViewModel) : base(mainViewModel)
    {
        MainViewModel = mainViewModel;
    }

    public ScriptPopupViewModel() : base(new MainViewModel()) {}

    [RelayCommand]
    private void ShowScript()
    {
        IsScriptShown = true;
        UpdateVisibilities();
    }

    [RelayCommand]
    private void ShowReports()
    {
        IsScriptShown = false;
        UpdateVisibilities();
    }

    [RelayCommand]
    private async void ImportScript()
    {
        var options = new FilePickerOpenOptions
        {
            Title = "Select C# File",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("C# Files")
                {
                    Patterns = new[] { "*.cs" }
                }
            }
        };

        SelectedScriptPath = await MainViewModel.DialogService.OpenFileDialogAsync(options);
        if (SelectedScriptPath == null) return;
        ScriptContents = new TextDocument(await File.ReadAllTextAsync(SelectedScriptPath));
        UpdateVisibilities();
    }

    [RelayCommand]
    private async void Script()
    {
        if (SelectedScriptPath == null)
        {
            Close();
            return;
        };
        await File.WriteAllTextAsync(SelectedScriptPath, ScriptContents.Text);
        
        IsScripting = true;
        ShowReports();
        
        foreach (var report in MainViewModel.SelectedReports)
        {
            if (!IsScripting) return;
            
            report.Status = Report.StatusType.Loading;
            
            var username = "simon.escano@amcsgroup.com";
            var password = "Onacsenomis8_";
            var tabularEditorPath = "C:/Users/simon.escano/Downloads/TabularEditor.2.26.0/TabularEditor.exe";

            try
            {
                await Cli.Wrap(tabularEditorPath)
                    .WithArguments(args => args
                        .Add(
                            $"Data Source=powerbi://api.powerbi.com/v1.0/myorg/{report.Workspace!.Name};User ID={username};Password={password}")
                        .Add($"{report.DatasetId}")
                        .Add("-S")
                        .Add($"{SelectedScriptPath}")
                        .Add("-D")
                    )
                    .WithStandardOutputPipe(PipeTarget.ToDelegate(Console.WriteLine))
                    .ExecuteAsync();
            }
            catch (Exception e)
            {
                report.Status = Report.StatusType.Error;
                continue;
            }
            report.Status = Report.StatusType.Success;
        }
        
        Close();
    }
    
    [RelayCommand]
    private void Close()
    {
        IsVisible = false;
        
        if (!IsScripting) return;
        Console.Error.WriteLine("Script stopped...");
        IsScripting =  false;
        foreach (var report in MainViewModel.SelectedReports)
        {
            report.Status = Report.StatusType.Selectable;
        }
    }

    private void UpdateVisibilities()
    {
        ImportButtonVisibility = IsScriptShown && (SelectedScriptPath == null);
        EditScriptVisibility = IsScriptShown && (SelectedScriptPath != null);
    }
}