# Popup System

AutoPBI uses a comprehensive popup system for modal dialogs and user interactions. This guide explains how to add new popups and understand the existing popup architecture.

## 🏗️ Popup Architecture

### **Base Popup Structure**
All popups inherit from `PopupViewModel` and follow a consistent pattern:

```csharp
// ViewModels/Popups/PopupViewModel.cs
public abstract partial class PopupViewModel: ViewModelBase
{
    [ObservableProperty] private bool _isOpen;
    [ObservableProperty] private bool _isProcessing;
    [ObservableProperty] private MainViewModel _mainViewModel;
    [ObservableProperty] private ObservableCollection<OverlayViewModel> _overlays = [];
    
    protected PopupViewModel(MainViewModel mainViewModel)
    {
        IsOpen = false;
        MainViewModel = mainViewModel;
    }
}
```

### **Popup Registration Pattern**
```csharp
// MainViewModel.cs
private void InitializePopups()
{
    DownloadPopup = AddPopup(new DownloadPopupViewModel(this));
    PublishPopup = AddPopup(new PublishPopupViewModel(this));
    DeletePopup = AddPopup(new DeletePopupViewModel(this));
    // ... other popups
}

private PopupViewModel AddPopup(PopupViewModel popup)
{
    Popups.Add(popup);
    return popup;
}
```

## 📁 File Structure

### **Required Files for New Popup**
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

## 🔧 Adding a New Popup

### **Step 1: Create ViewModel**
```csharp
// ViewModels/Popups/NewFeaturePopupViewModel.cs
using AutoPBI.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoPBI.ViewModels.Popups;

public partial class NewFeaturePopupViewModel : PopupViewModel
{
    [ObservableProperty] private string _inputText = "";
    [ObservableProperty] private bool _isOptionEnabled;
    
    public NewFeaturePopupViewModel(MainViewModel mainViewModel) : base(mainViewModel)
    {
        MainViewModel = mainViewModel;
    }

    [RelayCommand]
    private async void ExecuteFeature()
    {
        IsProcessing = true;
        RestartCts();
        
        try
        {
            // Your feature logic here
            await MainViewModel.Psr.Wrap()
                .WithArguments(args => args.Add("Your-PowerShell-Command"))
                .ExecuteAsync(Cts.Token);
                
            MainViewModel.Success(("Success!", "Feature completed successfully."));
        }
        catch (Exception e)
        {
            MainViewModel.Error(("Error!", e.Message));
        }
        finally
        {
            IsProcessing = false;
        }
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
            <StackPanel Grid.Row="1" Spacing="16" Margin="0 24">
                <TextBox Text="{Binding InputText}"
                         Watermark="Enter input text"/>
                
                <CheckBox Content="Enable option"
                          IsChecked="{Binding IsOptionEnabled}"/>
            </StackPanel>
            
            <!-- Actions -->
            <StackPanel Grid.Row="2" 
                        Orientation="Horizontal" 
                        HorizontalAlignment="Right"
                        Spacing="12">
                <Button Content="Cancel"
                        Command="{Binding ClosePopupCommand}"/>
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
    // ... existing popups
    NewFeaturePopup = AddPopup(new NewFeaturePopupViewModel(this));
}
```

### **Step 5: Add to MainView**
```xml
<!-- Views/MainView.axaml -->
<Grid Grid.Row="0" Grid.Column="0" Grid.ColumnSpan="2" Grid.RowSpan="2" ZIndex="1">
    <!-- ... existing popups -->
    <popups:NewFeaturePopupView DataContext="{Binding NewFeaturePopup}" />
</Grid>
```

## 🎨 Popup Styling Patterns

### **Common Popup Structure**
```xml
<Border Background="{DynamicResource Background}"
        CornerRadius="12"
        Padding="24"
        Width="500"
        Height="400">
    
    <Grid RowDefinitions="Auto, *, Auto">
        <!-- Header -->
        <StackPanel Grid.Row="0" Spacing="16">
            <TextBlock Text="Popup Title"
                      FontSize="20"
                      FontWeight="SemiBold"/>
            <TextBlock Text="Description"
                      Foreground="{DynamicResource ForegroundSecondary}"/>
        </StackPanel>
        
        <!-- Content -->
        <StackPanel Grid.Row="1" Spacing="16" Margin="0 24">
            <!-- Your content here -->
        </StackPanel>
        
        <!-- Actions -->
        <StackPanel Grid.Row="2" 
                    Orientation="Horizontal" 
                    HorizontalAlignment="Right"
                    Spacing="12">
            <Button Content="Cancel"/>
            <Button Content="Confirm"/>
        </StackPanel>
    </Grid>
</Border>
```

### **Status Indicators**
```xml
<!-- Loading State -->
<controls:StatusIcon Status="Loading" 
                    IsVisible="{Binding IsProcessing}"/>

<!-- Success State -->
<controls:StatusIcon Status="Success" 
                    IsVisible="{Binding IsSuccess}"/>

<!-- Error State -->
<controls:StatusIcon Status="Error" 
                    IsVisible="{Binding IsError}"/>
```

## 🔄 Popup Lifecycle

### **Opening a Popup**
```csharp
// In MainViewModel or other ViewModel
[RelayCommand]
private void OpenNewFeaturePopup()
{
    NewFeaturePopup.IsOpen = true;
}
```

### **Closing a Popup**
```csharp
// In PopupViewModel
[RelayCommand]
public virtual void Close(Action? whileProcessingAction = null)
{
    IsOpen = false;
    Cts.Cancel();
    SetReportsSelectable();
    
    if (!IsProcessing) return;
    whileProcessingAction?.Invoke();
    IsProcessing = false;
}
```

### **Processing State Management**
```csharp
[RelayCommand]
private async void ExecuteFeature()
{
    IsProcessing = true;
    RestartCts();
    
    try
    {
        // Your async operation
        await SomeAsyncOperation();
        MainViewModel.Success(("Success!", "Operation completed."));
    }
    catch (Exception e)
    {
        MainViewModel.Error(("Error!", e.Message));
    }
    finally
    {
        IsProcessing = false;
    }
}
```

## 📊 Data Binding Patterns

### **Two-Way Binding**
```xml
<TextBox Text="{Binding InputText}"/>
<CheckBox IsChecked="{Binding IsOptionEnabled}"/>
```

### **Command Binding**
```xml
<Button Command="{Binding ExecuteFeatureCommand}"
        IsEnabled="{Binding !IsProcessing}"/>
```

### **Visibility Binding**
```xml
<Border IsVisible="{Binding IsOpen}">
    <!-- Popup content -->
</Border>
```

## 🎯 Common Popup Types

### **Confirmation Popup**
```csharp
public partial class ConfirmationPopupViewModel : PopupViewModel
{
    [ObservableProperty] private string _message = "";
    [ObservableProperty] private string _confirmText = "Confirm";
    
    [RelayCommand]
    private void Confirm()
    {
        // Execute confirmation logic
        Close();
    }
}
```

### **Progress Popup**
```csharp
public partial class ProgressPopupViewModel : PopupViewModel
{
    [ObservableProperty] private int _progress = 0;
    [ObservableProperty] private string _statusMessage = "";
    
    public async Task UpdateProgress(int progress, string message)
    {
        Progress = progress;
        StatusMessage = message;
        await Task.Delay(100); // UI update delay
    }
}
```

### **Settings Popup**
```csharp
public partial class SettingsPopupViewModel : PopupViewModel
{
    [ObservableProperty] private bool _autoSave = true;
    [ObservableProperty] private string _defaultPath = "";
    
    [RelayCommand]
    private void SaveSettings()
    {
        // Save settings logic
        MainViewModel.Success(("Settings saved!", "Your preferences have been updated."));
        Close();
    }
}
```

## 🔧 Best Practices

### **1. Always Inherit from PopupViewModel**
```csharp
public partial class YourPopupViewModel : PopupViewModel
{
    // Your implementation
}
```

### **2. Use ObservableProperty for UI Properties**
```csharp
[ObservableProperty] private string _inputText = "";
[ObservableProperty] private bool _isProcessing;
```

### **3. Handle Cancellation**
```csharp
RestartCts(); // Reset cancellation token
// Use Cts.Token in async operations
```

### **4. Provide User Feedback**
```csharp
MainViewModel.Success(("Success!", "Operation completed."));
MainViewModel.Error(("Error!", "Something went wrong."));
MainViewModel.Warning(("Warning!", "Please check your input."));
```

### **5. Clean Up Resources**
```csharp
public override void Close(Action? whileProcessingAction = null)
{
    IsOpen = false;
    Cts.Cancel();
    SetReportsSelectable();
    // Additional cleanup
}
```

---

**This popup system provides a consistent and maintainable way to add new features to AutoPBI. Follow these patterns to ensure your popups integrate seamlessly with the existing codebase.** 