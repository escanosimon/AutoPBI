using System.Windows.Input;
using AutoPBI.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using TextCopy;

namespace AutoPBI.Controls;

public partial class WorkspaceItem : UserControl
{
    public static readonly StyledProperty<StatusIcon.StatusType> StatusProperty = AvaloniaProperty.Register<WorkspaceItem, StatusIcon.StatusType>(
        nameof(Status), StatusIcon.StatusType.Selectable);

    public StatusIcon.StatusType Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }
    
    public static readonly StyledProperty<bool> IsShownProperty = AvaloniaProperty.Register<WorkspaceItem, bool>(
        nameof(IsShown));

    public bool IsShown
    {
        get => GetValue(IsShownProperty);
        set => SetValue(IsShownProperty, value);
    }
    
    public static readonly StyledProperty<bool> IsSelectedProperty = AvaloniaProperty.Register<WorkspaceItem, bool>(
        nameof(IsSelected));

    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }
    
    public static readonly StyledProperty<Workspace> WorkspaceProperty = AvaloniaProperty.Register<WorkspaceItem, Workspace>(
        nameof(Workspace));

    public Workspace Workspace
    {
        get => GetValue(WorkspaceProperty);
        set => SetValue(WorkspaceProperty, value);
    }

    public static readonly StyledProperty<ICommand> CommandProperty = AvaloniaProperty.Register<WorkspaceItem, ICommand>(
        nameof(Command));

    public ICommand Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public static readonly StyledProperty<object?> CommandParameterProperty = AvaloniaProperty.Register<WorkspaceItem, object?>(
        nameof(CommandParameter));

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }
    
    public WorkspaceItem()
    {
        InitializeComponent();
    }

    private void CopyWorkspaceId(object? sender, RoutedEventArgs e)
    {
        ClipboardService.SetText(Workspace.Id!);
    }
}