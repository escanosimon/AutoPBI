using System.Windows.Input;
using AutoPBI.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AutoPBI.Controls;

public class StatusIcon : TemplatedControl
{
    public enum StatusType
    {
        Showable,
        Selectable,
        Success,
        Loading,
        Warning,
        Error
    }
    
    public static readonly StyledProperty<object?> TooltipProperty = AvaloniaProperty.Register<StatusIcon, object?>(
        nameof(Tooltip), false);

    public object? Tooltip
    {
        get => GetValue(TooltipProperty);
        set => SetValue(TooltipProperty, value);
    }
    
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
    
    public static readonly StyledProperty<StatusType> StatusProperty = AvaloniaProperty.Register<StatusIcon, StatusType>(
        nameof(Status));

    public StatusType Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public static readonly StyledProperty<object?> IconUnicodeProperty = AvaloniaProperty.Register<StatusIcon, object?>(
        nameof(IconUnicode));

    public object? IconUnicode
    {
        get => GetValue(IconUnicodeProperty);
        set => SetValue(IconUnicodeProperty, value);
    }

    public static readonly StyledProperty<bool> IsSelectedProperty = AvaloniaProperty.Register<StatusIcon, bool>(
        nameof(IsSelected));

    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    public static readonly StyledProperty<bool> IsShownProperty = AvaloniaProperty.Register<StatusIcon, bool>(
        nameof(IsShown));

    public bool IsShown
    {
        get => GetValue(IsShownProperty);
        set => SetValue(IsShownProperty, value);
    }
}