using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using NeoSmart.SecureStore;

namespace AutoPBI.Services;

public class SecureStorageService
{
    public static (string Username, string Password)? LoadSavedCredentials()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appFolderPath = Path.Combine(appDataPath, "AutoPBI");
        var secretsFile = Path.Combine(appFolderPath, "secrets.json");
        var keyFile = Path.Combine(appFolderPath, "key.dat");

        if (!File.Exists(secretsFile) || !File.Exists(keyFile)) return null;
        using var sman = SecretsManager.LoadStore(secretsFile);
        sman.LoadKeyFromFile(keyFile);
        try
        {
            var username = sman.Get("username");
            var password = sman.Get("password");
            return (username, password);
        }
        catch (KeyNotFoundException)
        {
            // Keys not found, return null
            return null;
        }
    }
    
    public static void SaveCredentials(string username, string password)
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appFolderPath = Path.Combine(appDataPath, "AutoPBI");
        Directory.CreateDirectory(appFolderPath);
        
        var secretsFile = Path.Combine(appFolderPath, "secrets.json");
        Console.WriteLine(secretsFile);
        var keyFile = Path.Combine(appFolderPath, "key.dat");
        Console.WriteLine(keyFile);

        using var sman = File.Exists(secretsFile) ? SecretsManager.LoadStore(secretsFile) : SecretsManager.CreateStore();
        if (!File.Exists(keyFile))
        {
            sman.GenerateKey();
            sman.ExportKey(keyFile);
        }
        else
        {
            sman.LoadKeyFromFile(keyFile);
        }

        sman.Set("username", username);
        sman.Set("password", password);
        sman.SaveStore(secretsFile);
    }

    public static void ClearSavedCredentials()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appFolderPath = Path.Combine(appDataPath, "AutoPBI");
        var secretsFile = Path.Combine(appFolderPath, "secrets.json");
        var keyFile = Path.Combine(appFolderPath, "key.dat");
        if (File.Exists(secretsFile)) File.Delete(secretsFile);
        if (File.Exists(keyFile)) File.Delete(keyFile);
    }
}