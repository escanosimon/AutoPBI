using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AutoPBI.Controls;

public class Toast : TemplatedControl
{
    public enum StatusType
    {
        Normal,
        Success,
        Warning,
        Error
    }
    
    public static readonly StyledProperty<StatusType> StatusProperty = AvaloniaProperty.Register<Toast, StatusType>(
        nameof(Status));

    public StatusType Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public static readonly StyledProperty<object?> IconUnicodeProperty = AvaloniaProperty.Register<Toast, object?>(
        nameof(IconUnicode));

    public object? IconUnicode
    {
        get => GetValue(IconUnicodeProperty);
        set => SetValue(IconUnicodeProperty, value);
    }

    public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<Toast, string>(
        nameof(Title));

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly StyledProperty<string> DescriptionProperty = AvaloniaProperty.Register<Toast, string>(
        nameof(Description));

    public string Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public static readonly StyledProperty<ICommand> CommandProperty = AvaloniaProperty.Register<Toast, ICommand>(
        nameof(Command));

    public ICommand Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
}