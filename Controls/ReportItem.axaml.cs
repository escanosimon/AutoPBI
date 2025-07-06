using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AutoPBI.Controls;

public class ReportItem : TemplatedControl
{
    public static readonly StyledProperty<string> ReportIdProperty = AvaloniaProperty.Register<ReportItem, string>(
        nameof(ReportId));

    public string ReportId
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
    
    public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<ReportItem, string>(
        nameof(Text), "Unnamed");

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
    
    
}