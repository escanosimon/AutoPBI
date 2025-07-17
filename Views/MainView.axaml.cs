using System;
using AutoPBI.ViewModels;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AutoPBI;

public partial class MainView : Window
{
    public MainView()
    {
        InitializeComponent();
    }

    private void DragWindow(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            BeginMoveDrag(e);
        }
    }

    private void OnSignOutButton_Click(object? sender, RoutedEventArgs e)
    {
        SignOutButton.Flyout!.Hide();
    }
}