using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Threading.Tasks;
using AutoPBI.Models;
using AutoPBI.Services;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NeoSmart.SecureStore;

namespace AutoPBI.ViewModels.Popups;

public partial class LoginPopupViewModel : PopupViewModel
{
    [ObservableProperty] private string? _usernameText = "";
    [ObservableProperty] private string? _passwordText = "";
    [ObservableProperty] private bool _isRememberMeChecked;
    [ObservableProperty] private bool _isPasswordShown;
    [ObservableProperty] private object? _eyeIconUnicode = "\uf070" ;
    [ObservableProperty] private char _passwordChar = '*' ;
    
    public LoginPopupViewModel(MainViewModel mainViewModel) : base(mainViewModel)
    {
        MainViewModel = mainViewModel;
    }

    public LoginPopupViewModel() : base(new MainViewModel()) {}

    [RelayCommand]
    private void ToggleIsPasswordShown()
    {
        IsPasswordShown = !IsPasswordShown;
        EyeIconUnicode = IsPasswordShown ? "\uf06e" :  "\uf070";
        PasswordChar = IsPasswordShown ? '\0' : '*';
    }

    [RelayCommand]
    private async Task Login()
    {
        if (string.IsNullOrWhiteSpace(UsernameText) || string.IsNullOrWhiteSpace(PasswordText))
        {
            MainViewModel.WarningCommand.Execute(("Cannot log in.", "Please fill out the fields."));
            return;
        }
        IsProcessing = true;

        try
        {
            await MainViewModel.Login(UsernameText!, PasswordText!);
        }
        catch (Exception)
        {
            IsProcessing = false;
            return;
        }
        
        if (IsRememberMeChecked)
        {
            try
            {
                SecureStorageService.SaveCredentials(MainViewModel.User.UserName!, MainViewModel.User.Password!);
            }
            catch (Exception e)
            {
                MainViewModel.ErrorCommand.Execute(("Cannot save credentials.", e.Message));
            }
        }
        else
        {
            try
            {
                SecureStorageService.ClearSavedCredentials();
            }
            catch (Exception e)
            {
                MainViewModel.ErrorCommand.Execute(("Cannot clear saved credentials.", e.Message));   
            }
        }
        
        Close();
        await MainViewModel.FetchWorkspacesCommand.ExecuteAsync(null);
    }
    
    [RelayCommand]
    private void RememberMe()
    {
        IsRememberMeChecked = !IsRememberMeChecked;
    }
}