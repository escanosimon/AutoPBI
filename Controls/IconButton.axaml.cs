using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace AutoPBI.Controls;

public class IconButton : TemplatedControl
{
    public static readonly StyledProperty<ICommand> CommandProperty = AvaloniaProperty.Register<IconButton, ICommand>(
        nameof(Command));

    public ICommand Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public static readonly StyledProperty<object?> CommandParameterProperty = AvaloniaProperty.Register<IconButton, object?>(
        nameof(CommandParameter));

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }
    
    public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<IconButton, string>(
        nameof(Text));

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly StyledProperty<string> PathProperty = AvaloniaProperty.Register<IconButton, string>(
        nameof(Path));

    public string Path
    {
        get => GetValue(PathProperty);
        set => SetValue(PathProperty, value);
    }
}