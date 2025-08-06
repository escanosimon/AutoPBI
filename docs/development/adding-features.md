# Adding New Features

This guide provides a step-by-step approach for adding new features to AutoPBI. Follow these patterns to ensure your features integrate seamlessly with the existing codebase.

## 🎯 Feature Development Workflow

### **1. Planning Phase**
- Define the feature requirements
- Identify which existing patterns to follow
- Plan the UI/UX design
- Determine PowerShell integration needs

### **2. Implementation Phase**
- Create ViewModel (business logic)
- Create View (UI definition)
- Add to MainViewModel and MainView
- Implement PowerShell integration
- Add error handling and user feedback

### **3. Testing Phase**
- Test the feature functionality
- Verify UI responsiveness
- Test error scenarios
- Validate PowerShell commands

## 📁 File Structure for New Features

### **Popup-Based Features**
```
ViewModels/Popups/
├── NewFeaturePopupViewModel.cs    # Business logic
└── PopupViewModel.cs             # Base class

Views/Popups/
├── NewFeaturePopupView.axaml     # UI definition
└── NewFeaturePopupView.axaml.cs  # Code-behind

MainView.axaml                    # Add popup reference
MainViewModel.cs                  # Add popup property
```

### **Direct Features (No Popup)**
```
ViewModels/
├── NewFeatureViewModel.cs        # Business logic
└── ViewModelBase.cs             # Base class

Views/
├── NewFeatureView.axaml         # UI definition
└── NewFeatureView.axaml.cs      # Code-behind

MainView.axaml                    # Add view reference
MainViewModel.cs                  # Add view model property
```

## 🔧 Step-by-Step Implementation

### **Step 1: Create ViewModel**

```csharp
// ViewModels/Popups/NewFeaturePopupViewModel.cs
using System;
using System.Threading.Tasks;
using AutoPBI.ViewModels;
using AutoPBI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoPBI.ViewModels.Popups;

public partial class NewFeaturePopupViewModel : PopupViewModel
{
    [ObservableProperty] private string _inputText = "";
    [ObservableProperty] private bool _isOptionEnabled;
    [ObservableProperty] private string _statusMessage = "";
    
    public NewFeaturePopupViewModel(MainViewModel mainViewModel) : base(mainViewModel)
    {
        MainViewModel = mainViewModel;
    }

    [RelayCommand]
    private async void ExecuteFeature()
    {
        if (string.IsNullOrWhiteSpace(InputText))
        {
            MainViewModel.Warning(("Invalid input", "Please enter required information."));
            return;
        }

        IsProcessing = true;
        RestartCts();
        
        var successes = 0;
        var warnings = 0;
        var errors = 0;

        try
        {
            // Your feature logic here
            var result = await Psr.Wrap()
                .WithArguments(args => args.Add("Your-PowerShell-Command"))
                .WithArguments(args => args.Add("-Parameter").Add(InputText))
                .WithStandardErrorPipe(Console.Error.WriteLine)
                .ExecuteAsync(Cts.Token);

            if (result.ExitCode == 0)
            {
                successes++;
                StatusMessage = "Feature executed successfully";
            }
            else
            {
                errors++;
                StatusMessage = "Feature execution failed";
            }
        }
        catch (OperationCanceledException)
        {
            SetReportsSelectable();
            MainViewModel.Toast(Toast.StatusType.Normal, "Operation cancelled", "Feature execution was cancelled.");
            return;
        }
        catch (Exception e)
        {
            errors++;
            StatusMessage = $"Error: {e.Message}";
        }
        finally
        {
            IsProcessing = false;
        }

        // Show final status
        ToastCommand(successes, warnings, errors).Execute(
            ("Feature completed", $"{successes} successful, {warnings} warnings, {errors} errors."));
    }
    
    [RelayCommand]
    private void ClosePopup()
    {
        Close();
    }
}
```

### **Step 2: Create View**

```xml
<!-- Views/Popups/NewFeaturePopupView.axaml -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:controls="clr-namespace:AutoPBI.Controls"
             x:Class="AutoPBI.Views.Popups.NewFeaturePopupView">
    
    <Border IsVisible="{Binding IsOpen}"
            Background="{DynamicResource Background}"
            CornerRadius="12"
            Padding="24"
            Width="500"
            Height="400">
        
        <Grid RowDefinitions="Auto, *, Auto">
            <!-- Header -->
            <StackPanel Grid.Row="0" Spacing="16">
                <TextBlock Text="New Feature"
                          FontSize="20"
                          FontWeight="SemiBold"/>
                <TextBlock Text="Description of your new feature"
                          Foreground="{DynamicResource ForegroundSecondary}"/>
            </StackPanel>
            
            <!-- Content -->
            <ScrollViewer Grid.Row="1" Margin="0 24">
                <StackPanel Spacing="16">
                    <TextBox Text="{Binding InputText}"
                             Watermark="Enter required information"
                             IsEnabled="{Binding !IsProcessing}"/>
                    
                    <CheckBox Content="Enable option"
                              IsChecked="{Binding IsOptionEnabled}"
                              IsEnabled="{Binding !IsProcessing}"/>
                    
                    <TextBlock Text="{Binding StatusMessage}"
                              Foreground="{DynamicResource ForegroundSecondary}"
                              IsVisible="{Binding !IsProcessing}"/>
                    
                    <!-- Processing indicator -->
                    <StackPanel IsVisible="{Binding IsProcessing}"
                              Spacing="8"
                              HorizontalAlignment="Center">
                        <controls:StatusIcon Status="Loading"/>
                        <TextBlock Text="Processing..."
                                  HorizontalAlignment="Center"/>
                    </StackPanel>
                </StackPanel>
            </ScrollViewer>
            
            <!-- Actions -->
            <StackPanel Grid.Row="2" 
                        Orientation="Horizontal" 
                        HorizontalAlignment="Right"
                        Spacing="12">
                <Button Content="Cancel"
                        Command="{Binding ClosePopupCommand}"
                        IsEnabled="{Binding !IsProcessing}"/>
                <Button Content="Execute"
                        Command="{Binding ExecuteFeatureCommand}"
                        IsEnabled="{Binding !IsProcessing}"/>
            </StackPanel>
        </Grid>
    </Border>
</UserControl>
```

### **Step 3: Add Code-Behind**

```csharp
// Views/Popups/NewFeaturePopupView.axaml.cs
using Avalonia.Controls;

namespace AutoPBI.Views.Popups;

public partial class NewFeaturePopupView : UserControl
{
    public NewFeaturePopupView()
    {
        InitializeComponent();
    }
}
```

### **Step 4: Register in MainViewModel**

```csharp
// MainViewModel.cs
[ObservableProperty] private PopupViewModel _newFeaturePopup;

private void InitializePopups()
{
    DownloadPopup = AddPopup(new DownloadPopupViewModel(this));
    PublishPopup = AddPopup(new PublishPopupViewModel(this));
    // ... existing popups
    NewFeaturePopup = AddPopup(new NewFeaturePopupViewModel(this));
}

[RelayCommand]
private void OpenNewFeaturePopup()
{
    NewFeaturePopup.IsOpen = true;
}
```

### **Step 5: Add to MainView**

```xml
<!-- Views/MainView.axaml -->
<Grid Grid.Row="0" Grid.Column="0" Grid.ColumnSpan="2" Grid.RowSpan="2" ZIndex="1">
    <popups:DownloadPopupView DataContext="{Binding DownloadPopup}" />
    <popups:PublishPopupView DataContext="{Binding PublishPopup}" />
    <!-- ... existing popups -->
    <popups:NewFeaturePopupView DataContext="{Binding NewFeaturePopup}" />
</Grid>
```

### **Step 6: Add UI Button (Optional)**

```xml
<!-- Add button to trigger the popup -->
<Button Content="New Feature"
        Command="{Binding OpenNewFeaturePopupCommand}"/>
```

## 🔧 PowerShell Integration

### **Basic PowerShell Command**

```csharp
// Simple command execution
var result = await Psr.Wrap()
    .WithArguments(args => args.Add("Get-PowerBIWorkspace"))
    .ExecuteAsync(Cts.Token);
```

### **Command with Parameters**

```csharp
// Command with parameters
var result = await Psr.Wrap()
    .WithArguments(args => args
        .Add("Connect-PowerBIServiceAccount")
        .Add("-Credential")
        .Add(credential))
    .ExecuteAsync(Cts.Token);
```

### **Command with Error Handling**

```csharp
// Command with error handling
try
{
    var result = await Psr.Wrap()
        .WithArguments(args => args.Add("Your-Command"))
        .WithStandardErrorPipe(Console.Error.WriteLine)
        .ExecuteAsync(Cts.Token);
        
    if (result.ExitCode == 0)
    {
        // Success
        MainViewModel.Success(("Success!", "Command executed successfully."));
    }
    else
    {
        // Error
        MainViewModel.Error(("Error!", "Command failed."));
    }
}
catch (Exception e)
{
    MainViewModel.Error(("Error!", e.Message));
}
```

### **Complex PowerShell Script**

```csharp
// Multi-line PowerShell script
var script = @"
$workspaces = Get-PowerBIWorkspace
foreach ($workspace in $workspaces) {
    Write-Host $workspace.Name
}
";

var result = await Psr.Wrap()
    .WithArguments(args => args.Add(script))
    .ExecuteAsync(Cts.Token);
```

## 🎯 Common Feature Patterns

### **Bulk Operations Pattern**

```csharp
[RelayCommand]
private async void ExecuteBulkOperation()
{
    IsProcessing = true;
    RestartCts();
    
    var successes = 0;
    var warnings = 0;
    var errors = 0;

    foreach (var workspace in MainViewModel.Workspaces.ToList())
    {
        foreach (var report in workspace.SelectedReports.ToList())
        {
            report.Loading();
            
            try
            {
                // Your operation logic here
                await ExecuteOperation(report);
                report.Success("Operation completed successfully");
                successes++;
            }
            catch (OperationCanceledException)
            {
                SetReportsSelectable();
                MainViewModel.Toast(Toast.StatusType.Normal, "Operation cancelled", $"Last processed: {report.Name}");
                return;
            }
            catch (Exception e)
            {
                report.Error(e.Message);
                errors++;
            }
        }
    }
    
    IsProcessing = false;
    ToastCommand(successes, warnings, errors).Execute(
        ("Operation completed", $"{successes} successful, {warnings} warnings, {errors} errors."));
}
```

### **File/Folder Selection Pattern**

```csharp
[RelayCommand]
private async void SelectFiles()
{
    var options = new FilePickerOpenOptions
    {
        Title = "Select Files",
        AllowMultiple = true,
        FileTypeFilter = new[]
        {
            new FilePickerFileType("Power BI Files")
            {
                Patterns = ["*.pbix"]
            }
        }
    };

    var selectedFiles = await MainViewModel.DialogService.OpenFileDialogAsync(options);
    foreach (var file in selectedFiles)
    {
        // Process selected files
    }
}
```

### **Confirmation Pattern**

```csharp
[RelayCommand]
private void ConfirmOperation()
{
    var overlay = AddOverlay(new OverlayViewModel(this, ExecuteOperationCommand, "Confirm", "Are you sure you want to proceed?"));
    OpenOverlay(overlay);
}

[RelayCommand]
private async void ExecuteOperation()
{
    // Your operation logic here
    Close();
}
```

## 🔧 Error Handling Patterns

### **PowerShell Error Handling**

```csharp
try
{
    var result = await Psr.Wrap()
        .WithArguments(args => args.Add("Your-Command"))
        .WithStandardErrorPipe(Console.Error.WriteLine)
        .ExecuteAsync(Cts.Token);
        
    if (result.ExitCode != 0)
    {
        throw new Exception("Command failed with exit code: " + result.ExitCode);
    }
}
catch (CmdletInvocationException e)
{
    // PowerShell-specific error
    MainViewModel.Error(("PowerShell Error", e.Message));
}
catch (Exception e)
{
    // General error
    MainViewModel.Error(("Error", e.Message));
}
```

### **User Input Validation**

```csharp
[RelayCommand]
private async void ExecuteFeature()
{
    // Validate input
    if (string.IsNullOrWhiteSpace(InputText))
    {
        MainViewModel.Warning(("Invalid input", "Please enter required information."));
        return;
    }
    
    if (InputText.Length < 3)
    {
        MainViewModel.Warning(("Invalid input", "Input must be at least 3 characters."));
        return;
    }
    
    // Proceed with execution
    IsProcessing = true;
    // ... rest of the logic
}
```

### **Cancellation Handling**

```csharp
try
{
    await SomeAsyncOperation(Cts.Token);
}
catch (OperationCanceledException)
{
    SetReportsSelectable();
    MainViewModel.Toast(Toast.StatusType.Normal, "Operation cancelled", "The operation was cancelled.");
    return;
}
```

## 📊 Testing Your Feature

### **1. Unit Testing**
```csharp
// Test your ViewModel logic
[Test]
public void ExecuteFeature_WithValidInput_ShouldSucceed()
{
    // Arrange
    var viewModel = new NewFeaturePopupViewModel(mockMainViewModel);
    viewModel.InputText = "valid input";
    
    // Act
    viewModel.ExecuteFeatureCommand.Execute(null);
    
    // Assert
    // Verify expected behavior
}
```

### **2. Integration Testing**
```csharp
// Test PowerShell integration
[Test]
public async Task ExecuteFeature_ShouldCallPowerShellCommand()
{
    // Arrange
    var mockPsr = new Mock<Psr>();
    var viewModel = new NewFeaturePopupViewModel(mockMainViewModel);
    
    // Act
    await viewModel.ExecuteFeature();
    
    // Assert
    mockPsr.Verify(x => x.Wrap(), Times.Once);
}
```

### **3. UI Testing**
```xml
<!-- Test UI elements are properly bound -->
<TextBox Text="{Binding InputText}"/>
<Button Command="{Binding ExecuteFeatureCommand}"
        IsEnabled="{Binding !IsProcessing}"/>
```

## 🔧 Best Practices

### **1. Follow Existing Patterns**
- Use the same naming conventions
- Follow the same file structure
- Use consistent error handling

### **2. Handle Cancellation**
```csharp
RestartCts(); // Always reset cancellation token
// Use Cts.Token in async operations
```

### **3. Provide User Feedback**
```csharp
MainViewModel.Success(("Success!", "Operation completed."));
MainViewModel.Error(("Error!", "Something went wrong."));
MainViewModel.Warning(("Warning!", "Please check your input."));
```

### **4. Clean Up Resources**
```csharp
public override void Close(Action? whileProcessingAction = null)
{
    IsOpen = false;
    Cts.Cancel();
    SetReportsSelectable();
    // Additional cleanup
}
```

### **5. Use ObservableProperty**
```csharp
[ObservableProperty] private string _inputText = "";
[ObservableProperty] private bool _isProcessing;
```

### **6. Document Your Feature**
- Add comments to complex logic
- Update this documentation if needed
- Include usage examples

---

**Following these patterns ensures your new features integrate seamlessly with the existing AutoPBI codebase and maintain consistency with the application's architecture and user experience.** 