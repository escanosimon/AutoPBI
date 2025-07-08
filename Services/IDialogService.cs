using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace AutoPBI.Services;

public interface IDialogService
{
    Task<string?> OpenFolderDialogAsync(FolderPickerOpenOptions? options = null);
    Task<string?> OpenFileDialogAsync(FilePickerOpenOptions? options = null);
}