using System;
using System.Management.Automation;
using System.Threading.Tasks;
using AutoPBI.Models;
using AutoPBI.Services;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoPBI.ViewModels.Popups;

public partial class LoginPopupViewModel : PopupViewModel
{
    [ObservableProperty] private string? _usernameText = "";
    [ObservableProperty] private string? _passwordText = "";
    [ObservableProperty] private bool _isRememberMeChecked;
    
    public LoginPopupViewModel(MainViewModel mainViewModel) : base(mainViewModel)
    {
        MainViewModel = mainViewModel;
    }

    public LoginPopupViewModel() : base(new MainViewModel()) {}

    [RelayCommand]
    private async void Login()
    {
        if (string.IsNullOrWhiteSpace(UsernameText) || string.IsNullOrWhiteSpace(PasswordText))
        {
            MainViewModel.WarningCommand.Execute(("Cannot log in.", "Please fill out the fields."));
            return;
        }
        
        IsProcessing = true;
        PSObject loginResult;
        try
        {
            loginResult = (await MainViewModel.PowerShellService
                .BuildCommand()
                .WithCommand($@"
                $password = '{PasswordText}' | ConvertTo-SecureString -asPlainText -Force;
                $username = '{UsernameText}';
                $credential = New-Object -TypeName System.Management.Automation.PSCredential -argumentlist $username, $password;
                Connect-PowerBIServiceAccount -Credential $credential
            ")
                .WithStandardErrorPipe(Console.Error.WriteLine)
                .ExecuteAsync()).Objects[0];
        }
        catch (Exception e)
        {
            MainViewModel.ErrorCommand.Execute(("Login failed!", e.Message));
            IsProcessing = false;
            return;
        }
        var accessTokenResult = (await MainViewModel.PowerShellService
            .BuildCommand()
            .WithCommand("(Get-PowerBIAccessToken).Values[0]")
            .WithStandardErrorPipe(Console.Error.WriteLine)
            .ExecuteAsync()).Objects[0];

        MainViewModel.User = new User(
            loginResult.Properties["Environment"].Value.ToString(),
            loginResult.Properties["TenantId"].Value.ToString(),
            UsernameText,
            PasswordText
        );

        MainViewModel.User.AccessToken = (string)accessTokenResult.BaseObject;
        
        MainViewModel.SuccessCommand.Execute(("Login successful!", "Fetching workspaces..."));
        MainViewModel.IsLoggedIn = true;
        MainViewModel.IsReloading = false;
        Close();
        await MainViewModel.FetchWorkspacesCommand.ExecuteAsync(null);
    }
    
    [RelayCommand]
    private void RememberMe()
    {
        IsRememberMeChecked = !IsRememberMeChecked;

        if (IsRememberMeChecked)
        {
            Console.Write("Remembering...");
        }
    }
}