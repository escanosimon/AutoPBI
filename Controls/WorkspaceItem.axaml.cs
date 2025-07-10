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
    public static readonly StyledProperty<bool> IsCheckedProperty = AvaloniaProperty.Register<WorkspaceItem, bool>(
        nameof(IsChecked));

    public bool IsChecked
    {
        get => GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
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