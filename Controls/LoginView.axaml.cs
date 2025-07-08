using System;
using AutoPBI.ViewModels;
using Avalonia.Controls;
using Avalonia.Input;

namespace AutoPBI;

public partial class LoginView : Window
{
    public LoginView()
    {
        InitializeComponent();
    }

    private void LoginButton_OnClick(object sender, PointerPressedEventArgs e)
    {
        ((MainViewModel)DataContext!).Login();
        Close();
    }
}