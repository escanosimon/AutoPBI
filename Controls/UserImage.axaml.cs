using AutoPBI.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AutoPBI.Controls;

public class UserImage : TemplatedControl
{
    public static readonly StyledProperty<IBrush> ColorProperty = AvaloniaProperty.Register<UserImage, IBrush>(
        nameof(Color));

    public IBrush Color
    {
        get => GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public static readonly StyledProperty<string[]> InitialsProperty = AvaloniaProperty.Register<UserImage, string[]>(
        nameof(Initials));

    public string[] Initials
    {
        get => GetValue(InitialsProperty);
        set => SetValue(InitialsProperty, value);
    }
}