using System;
using System.Diagnostics;
using System.Windows.Input;
using AutoPBI.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using TextCopy;

namespace AutoPBI.Controls;

public partial class ReportItem : UserControl
{
    public static readonly StyledProperty<Report> ReportProperty = AvaloniaProperty.Register<ReportItem, Report>(
        nameof(Report));

    public Report Report
    {
        get => GetValue(ReportProperty);
        set => SetValue(ReportProperty, value);
    }

    public static readonly StyledProperty<ICommand> CommandProperty = AvaloniaProperty.Register<ReportItem, ICommand>(
        nameof(Command));

    public ICommand Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public static readonly StyledProperty<object?> CommandParameterProperty = AvaloniaProperty.Register<ReportItem, object?>(
        nameof(CommandParameter));

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }
    
    public ReportItem()
    {
        InitializeComponent();
    }

    private void CopyReportId(object? sender, RoutedEventArgs e)
    {
        ClipboardService.SetTextAsync(Report.Id!);
    }
    
    private void CopyDatasetId(object? sender, RoutedEventArgs e)
    {
        ClipboardService.SetTextAsync(Report.DatasetId!);
    }

    private void OpenInPowerBiService(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Report.WebUrl))
            return;

        try
        {
            if (OperatingSystem.IsWindows())
            {
                Process.Start(new ProcessStartInfo("cmd", $"/c start {Report.WebUrl}") { CreateNoWindow = true });
            }
            else if (OperatingSystem.IsLinux())
            {
                Process.Start(new ProcessStartInfo("xdg-open", Report.WebUrl) { RedirectStandardOutput = true, UseShellExecute = true });
            }
            else if (OperatingSystem.IsMacOS())
            {
                Process.Start(new ProcessStartInfo("open", Report.WebUrl) { RedirectStandardOutput = true, UseShellExecute = true });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred when trying to open the URL: {ex.Message}");
        }
    }

    private void CopyMessage(object? sender, RoutedEventArgs e)
    {
        ClipboardService.SetTextAsync(Report.Message!.ToString()!);
    }
}