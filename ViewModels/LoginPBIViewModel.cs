using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Security;
using ReactiveUI;
using Avalonia.Threading;
using Avalonia.Controls;
using System.Reactive;
using System.Reactive.Linq;
using AutoPBI.Services;
using Avalonia.ReactiveUI;

namespace AutoPBI.ViewModels
{
    public class LoginPBIViewModel : ReactiveObject
    {
        private readonly Window _window;
        private readonly MainViewModel _mainViewModel;
    
        private string _email = "";
        public string Email
        {
            get => _email;
            set => this.RaiseAndSetIfChanged(ref _email, value);
        }

        private string _password = "";  
        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        public ReactiveCommand<Unit, Unit> SubmitCommand { get; }

        public LoginPBIViewModel(Window window, MainViewModel mainViewModel)
        {
            _window = window;
            _mainViewModel = mainViewModel;
        
            var canExecute = this.WhenAnyValue(
                x => x.Email,
                x => x.Password,
                (email, password) => !string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password)
            );

            SubmitCommand = ReactiveCommand.CreateFromTask(
                ExecuteSubmitAsync,
                canExecute,
                AvaloniaScheduler.Instance
            );      
        }


        private async Task ExecuteSubmitAsync()
        {
            await Dispatcher.UIThread.InvokeAsync(() => 
            {
                _mainViewModel.Email = Email;
                _mainViewModel.Password = Password;
                _mainViewModel.Login();
                _window?.Close();
            });
        }
    }

}