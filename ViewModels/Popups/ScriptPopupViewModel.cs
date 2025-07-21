using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Avalonia.Platform.Storage;
using AvaloniaEdit.Document;
using CliWrap;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoPBI.ViewModels.Popups;

public partial class ScriptPopupViewModel : PopupViewModel
{
    [ObservableProperty] private string _tabularEditorPath;
    
    [ObservableProperty] private bool _isScriptShown;
    [ObservableProperty] private bool _importButtonVisibility;
    [ObservableProperty] private bool _editScriptVisibility;
    [ObservableProperty] private string? _selectedScriptPath;
    [ObservableProperty] private TextDocument _scriptContents;
    
    public ScriptPopupViewModel(MainViewModel mainViewModel) : base(mainViewModel)
    {
        MainViewModel = mainViewModel;

        TabularEditorPath = Path.Combine(MainViewModel.ToolsFolder, "TabularEditor.2.26.0/TabularEditor.exe");
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

        SelectedScriptPath = (await MainViewModel.DialogService.OpenFileDialogAsync(options)).FirstOrDefault();
        if (SelectedScriptPath == null) return;
        ScriptContents = new TextDocument(await File.ReadAllTextAsync(SelectedScriptPath));
        UpdateVisibilities();
    }

    [RelayCommand]
    private async void Script()
    {
        if (SelectedScriptPath == null)
        {
            MainViewModel.WarningCommand.Execute(("Cannot proceed with script!", "Please import a C# script to apply."));
            return;
        }
        await File.WriteAllTextAsync(SelectedScriptPath, ScriptContents.Text);
        
        IsProcessing = true;
        ShowReports();
        
        var successes = 0;
        var warnings = 0;
        var errors = 0;

        foreach (var workspace in MainViewModel.Workspaces)
        {
            foreach (var report in workspace.SelectedReports)
            {
                if (!IsProcessing) return;
            
                report.Loading();

                var sbOutput = new StringBuilder();
                try
                {
                    await Cli.Wrap(TabularEditorPath)
                        .WithArguments(args => args
                            .Add($"Data Source=powerbi://api.powerbi.com/v1.0/myorg/{report.Workspace!.Name};User ID={MainViewModel.User.UserName};Password={MainViewModel.User.Password}")
                            .Add($"{report.DatasetId}")
                            .Add("-S")
                            .Add($"{SelectedScriptPath}")
                            .Add("-D")
                            .Add("-W")
                            .Add("-E")
                        )
                        .WithStandardOutputPipe(PipeTarget.ToStringBuilder(sbOutput))
                        .ExecuteAsync();
                }
                catch (Exception e)
                {
                    var result = sbOutput.ToString();
                    var lines = result.Split([Environment.NewLine], StringSplitOptions.None);
                    foreach (var line in lines)
                    {
                        if (line.Contains("Error", StringComparison.OrdinalIgnoreCase))
                        {
                            report.Error(line);
                        }
                    }
                    errors++;
                    continue;
                }
                report.Success("Successfully applied script to dataset");
                successes++;
            }
        }
        
        ToastCommand(successes, warnings, errors).Execute(("Scripting finished!", $"{successes} successful, {warnings} warnings, {errors} errors."));
    }

    [RelayCommand]
    private void OpenScriptRepository()
    {
        const string scriptRepository = "https://github.com/PowerBI-tips/TabularEditor-Scripts/";
        try
        {
            if (OperatingSystem.IsWindows())
            {
                Process.Start(new ProcessStartInfo("cmd", $"/c start {scriptRepository}") { CreateNoWindow = true });
            }
            else if (OperatingSystem.IsLinux())
            {
                Process.Start(new ProcessStartInfo("xdg-open", scriptRepository) { RedirectStandardOutput = true, UseShellExecute = true });
            }
            else if (OperatingSystem.IsMacOS())
            {
                Process.Start(new ProcessStartInfo("open", scriptRepository) { RedirectStandardOutput = true, UseShellExecute = true });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred when trying to open the URL: {ex.Message}");
        }
    }
    
    private void UpdateVisibilities()
    {
        ImportButtonVisibility = IsScriptShown && (SelectedScriptPath == null);
        EditScriptVisibility = IsScriptShown && (SelectedScriptPath != null);
    }

    public override void Close(Action? whileProcessingAction = null)
    {
        base.Close(() =>
        {
            SelectedScriptPath = null;
            ScriptContents = new TextDocument();
        });
    }
}