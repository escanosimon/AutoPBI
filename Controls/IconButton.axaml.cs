using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace AutoPBI.Controls;

public class IconButton : TemplatedControl
{
    static IconButton()
    {
        BackgroundProperty.Changed.AddClassHandler<IconButton>((control, args) =>
        {
            control.CoerceValue(HoverBackgroundProperty);
        });
        
        BorderBrushProperty.Changed.AddClassHandler<IconButton>((control, args) =>
        {
            control.CoerceValue(HoverBorderBrushProperty);
        });
    }
    private static IBrush? CoerceHoverBackground(AvaloniaObject instance, IBrush? value)
    {
        var iconButton = (IconButton)instance;
        return value ?? iconButton.Background;
    }
    
    private static IBrush? CoerceHoverBorderBrush(AvaloniaObject instance, IBrush? value)
    {
        var iconButton = (IconButton)instance;
        return value ?? iconButton.BorderBrush;
    }
    
    public static readonly StyledProperty<IBrush> HoverBackgroundProperty = AvaloniaProperty.Register<IconButton, IBrush>(
        nameof(HoverBackground), 
        defaultValue: null!,
        coerce: CoerceHoverBackground!);

    public IBrush HoverBackground
    {
        get => GetValue(HoverBackgroundProperty);
        set => SetValue(HoverBackgroundProperty, value);
    }

    public static readonly StyledProperty<IBrush> HoverBorderBrushProperty = AvaloniaProperty.Register<IconButton, IBrush>(
        nameof(HoverBorderBrush), 
        defaultValue: null!,
        coerce: CoerceHoverBorderBrush!);

    public IBrush HoverBorderBrush
    {
        get => GetValue(HoverBorderBrushProperty);
        set => SetValue(HoverBorderBrushProperty, value);
    }

    public static readonly StyledProperty<int?> IconSizeProperty = AvaloniaProperty.Register<IconButton, int?>(
        nameof(IconSize), 13);

    public int? IconSize
    {
        get => GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }
    
    public static readonly StyledProperty<object?> IconUnicodeProperty = AvaloniaProperty.Register<IconButton, object?>(
        nameof(IconUnicode), false);

    public object? IconUnicode
    {
        get => GetValue(IconUnicodeProperty);
        set => SetValue(IconUnicodeProperty, value);
    }
    
    public static readonly StyledProperty<object?> TextProperty = AvaloniaProperty.Register<IconButton, object?>(
        nameof(Text), false);

    public object? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly StyledProperty<object?> ExtraTextProperty = AvaloniaProperty.Register<IconButton, object?>(
        nameof(ExtraText), false);

    public object? ExtraText
    {
        get => GetValue(ExtraTextProperty);
        set => SetValue(ExtraTextProperty, value);
    }

    public static readonly StyledProperty<Thickness> IconMarginProperty = AvaloniaProperty.Register<IconButton, Thickness>(
        nameof(IconMargin), new Thickness(0, 0, 8, 0));

    public Thickness IconMargin
    {
        get => GetValue(IconMarginProperty);
        set => SetValue(IconMarginProperty, value);
    }

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

    public static readonly StyledProperty<EventHandler<RoutedEventArgs>?> ClickProperty = AvaloniaProperty.Register<IconButton, EventHandler<RoutedEventArgs>?>(
        nameof(Click));

    public EventHandler<RoutedEventArgs>? Click
    {
        get => GetValue(ClickProperty);
        set => SetValue(ClickProperty, value);
    }
}