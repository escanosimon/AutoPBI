using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

namespace AutoPBI.Services;

public class DialogService : IDialogService
{
    public async Task<string?> OpenFolderDialogAsync(FolderPickerOpenOptions? options = null)
    {
        var topLevel = TopLevel.GetTopLevel(Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop 
            ? desktop.MainWindow 
            : null);
            
        if (topLevel == null) return null;

        options ??= new FolderPickerOpenOptions
        {
            Title = "Select Folder",
            AllowMultiple = false
        };

        var result = await topLevel.StorageProvider.OpenFolderPickerAsync(options);
        return result?.FirstOrDefault()?.Path.LocalPath;
    }

    public async Task<IEnumerable<string>> OpenFileDialogAsync(FilePickerOpenOptions? options = null)
    {
        var topLevel = TopLevel.GetTopLevel(Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop 
            ? desktop.MainWindow 
            : null);
            
        if (topLevel == null) return null!;

        options ??= new FilePickerOpenOptions
        {
            Title = "Select File",
            AllowMultiple = false
        };

        
        var result = await topLevel.StorageProvider.OpenFilePickerAsync(options);
        return result?.Select(file => file.Path.LocalPath) ?? [];
    }
}