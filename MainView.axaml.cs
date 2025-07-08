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
        DataContext = new MainViewModel(); // Ensure ViewModel is set as DataContext
    }

    private async void BtnLogIn_OnClick(object sender, PointerPressedEventArgs e)
    {
        var loginView = new LoginView
        {
            DataContext = DataContext // Share the same ViewModel instance
        };
        await loginView.ShowDialog(this);
    }
}