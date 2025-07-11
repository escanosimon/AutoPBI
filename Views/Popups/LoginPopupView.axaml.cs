using System.Runtime.InteropServices.JavaScript;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AutoPBI.Views.Popups;

public partial class LoginPopupView : UserControl
{
    public LoginPopupView()
    {
        InitializeComponent();
    }

    private void TextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(PasswordTextBox.Text) || string.IsNullOrEmpty(UsernameTextBox.Text))
        {
            LoginButton.Classes.Add("Disabled");
        }
        else
        {
            LoginButton.Classes.Remove("Disabled");
        }
    }
}