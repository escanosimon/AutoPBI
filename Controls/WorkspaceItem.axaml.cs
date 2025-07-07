using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AutoPBI.Controls;

public class WorkspaceItem : TemplatedControl
{
    public static readonly StyledProperty<string> IdProperty = AvaloniaProperty.Register<WorkspaceItem, string>(
        nameof(Id));

    public string Id
    {
        get => GetValue(IdProperty);
        set => SetValue(IdProperty, value);
    }
    
    public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<WorkspaceItem, string>(
        nameof(Text), "Unnamed");

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
}