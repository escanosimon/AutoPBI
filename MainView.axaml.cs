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

    private void BtnLogIn_OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        ((MainViewModel) DataContext!).Login();
    }
}