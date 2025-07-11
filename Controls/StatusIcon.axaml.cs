using System.Windows.Input;
using AutoPBI.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AutoPBI.Controls;

public class StatusIcon : TemplatedControl
{
    public static readonly StyledProperty<ICommand> CommandProperty = AvaloniaProperty.Register<StatusIcon, ICommand>(
        nameof(Command));

    public ICommand Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public static readonly StyledProperty<object?> CommandParameterProperty = AvaloniaProperty.Register<StatusIcon, object?>(
        nameof(CommandParameter));

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }
    
    public static readonly StyledProperty<Report.StatusType> StatusProperty = AvaloniaProperty.Register<StatusIcon, Report.StatusType>(
        nameof(Status));

    public Report.StatusType Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }
    
    public static readonly StyledProperty<bool> IsCheckedProperty = AvaloniaProperty.Register<ReportItem, bool>(
        nameof(IsChecked));

    public bool IsChecked
    {
        get => GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }
}