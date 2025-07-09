using AutoPBI.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AutoPBI.Controls;

public class StatusIcon : TemplatedControl
{
    
    
    public static readonly StyledProperty<Report.StatusType> StatusProperty = AvaloniaProperty.Register<StatusIcon, Report.StatusType>(
        nameof(Status));

    public Report.StatusType Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }
    
    public static readonly StyledProperty<bool> IsCheckedProperty = AvaloniaProperty.Register<ReportItem, bool>(
        nameof(IsChecked));

    public bool IsChecked
    {
        get => GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }
}