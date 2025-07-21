using System.Windows.Input;
using AutoPBI.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AutoPBI.Controls;

public partial class WorkspaceWithReports : UserControl
{
    public static readonly StyledProperty<Workspace> WorkspaceProperty = AvaloniaProperty.Register<WorkspaceWithReports, Workspace>(
        nameof(Workspace));

    public Workspace Workspace
    {
        get => GetValue(WorkspaceProperty);
        set => SetValue(WorkspaceProperty, value);
    }

    public new static readonly StyledProperty<bool> IsEnabledProperty = AvaloniaProperty.Register<WorkspaceWithReports, bool>(
        nameof(IsEnabled));

    public new bool IsEnabled
    {
        get => GetValue(IsEnabledProperty);
        set => SetValue(IsEnabledProperty, value);
    }

    public WorkspaceWithReports()
    {
        InitializeComponent();
    }
}