using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AutoPBI.Controls;

public class ReportItem : TemplatedControl
{
    public static readonly StyledProperty<string> StatusProperty = AvaloniaProperty.Register<ReportItem, string>(
        nameof(Status));

    public string Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
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
    
    public static readonly StyledProperty<string> IdProperty = AvaloniaProperty.Register<ReportItem, string>(
        nameof(Id));

    public string Id
    {
        get => GetValue(IdProperty);
        set => SetValue(IdProperty, value);
    }
    
    public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<ReportItem, string>(
        nameof(Text), "Unnamed");

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly StyledProperty<bool> IsCheckedProperty = AvaloniaProperty.Register<ReportItem, bool>(
        nameof(IsChecked));

    public bool IsChecked
    {
        get => GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }
}