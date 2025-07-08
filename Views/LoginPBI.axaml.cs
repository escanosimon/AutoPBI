using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AutoPBI.ViewModels;

namespace AutoPBI.Views
{
    public partial class LoginPBI : Window
    {
        public LoginPBI(MainViewModel mainViewModel)
        {
            InitializeComponent();
            DataContext = new LoginPBIViewModel(this, mainViewModel);
        }
    }
}