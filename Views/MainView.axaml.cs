using System;
using AutoPBI.ViewModels;
using Avalonia.Controls;
using Avalonia.Input;

namespace AutoPBI;

public partial class MainView : Window
{
    public MainView()
    {
        InitializeComponent();
    }

    private void BtnLogIn_OnClick(object sender, PointerPressedEventArgs e)
    {
        Console.Error.WriteLine("First...");
        ((MainViewModel) DataContext!).Login();
    }
}