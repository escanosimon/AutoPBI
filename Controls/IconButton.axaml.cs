using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AutoPBI.Controls;

public class IconButton : TemplatedControl
{
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