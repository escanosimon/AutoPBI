using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AutoPBI.Controls;

public class WorkspaceItem : TemplatedControl
{
    public static readonly StyledProperty<string> WorkspaceIdProperty = AvaloniaProperty.Register<WorkspaceItem, string>(
        nameof(WorkspaceId));

    public string WorkspaceId
    {
        get => GetValue(WorkspaceIdProperty);
        set => SetValue(WorkspaceIdProperty, value);
    }
    
    public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<WorkspaceItem, string>(
        nameof(Text), "Unnamed");

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
}