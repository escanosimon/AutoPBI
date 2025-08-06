# UI Components

AutoPBI uses a comprehensive set of custom controls and UI components built with Avalonia UI. This guide covers the custom controls, styling system, and UI patterns used throughout the application.

## 🎨 Custom Controls

### **Toast Control**
The Toast control provides user feedback for operations with different status types.

```csharp
// Controls/Toast.axaml.cs
public class Toast : TemplatedControl
{
    public enum StatusType
    {
        Normal,
        Success,
        Warning,
        Error
    }
    
    public static readonly StyledProperty<StatusType> StatusProperty;
    public static readonly StyledProperty<string> TitleProperty;
    public static readonly StyledProperty<string> DescriptionProperty;
    public static readonly StyledProperty<ICommand> CommandProperty;
}
```

**Usage:**
```xml
<controls:Toast 
    IsVisible="{Binding IsToasting}"
    Status="{Binding ToastStatus}"
    Title="{Binding ToastTitle}"
    Description="{Binding ToastDescription}"
    Command="{Binding CloseToastCommand}"
    VerticalAlignment="Bottom" 
    HorizontalAlignment="Right" />
```

### **StatusIcon Control**
Displays status indicators for operations with different states.

```csharp
// Controls/StatusIcon.axaml.cs
public class StatusIcon : TemplatedControl
{
    public enum Status
    {
        Loading,
        Success,
        Warning,
        Error
    }
    
    public static readonly StyledProperty<Status> StatusProperty;
    public static readonly StyledProperty<string> MessageProperty;
}
```

**Usage:**
```xml
<controls:StatusIcon 
    Status="Loading"
    Message="Processing..."
    IsVisible="{Binding IsProcessing}"/>
```

### **IconButton Control**
Custom button with icon support using FontAwesome.

```csharp
// Controls/IconButton.axaml.cs
public class IconButton : Button
{
    public static readonly StyledProperty<string> IconUnicodeProperty;
    public static readonly StyledProperty<double> IconSizeProperty;
    public static readonly StyledProperty<IBrush> IconBrushProperty;
}
```

**Usage:**
```xml
<controls:IconButton 
    IconUnicode="&#xf019;"
    IconSize="16"
    Command="{Binding DownloadCommand}"/>
```

### **UserImage Control**
Displays user avatars with initials and color generation.

```csharp
// Controls/UserImage.axaml.cs
public class UserImage : TemplatedControl
{
    public static readonly StyledProperty<string[]> InitialsProperty;
    public static readonly StyledProperty<IBrush> ColorProperty;
    public static readonly StyledProperty<double> SizeProperty;
}
```

**Usage:**
```xml
<controls:UserImage 
    Initials="{Binding User.Initials}"
    Color="{Binding User.Color}"
    Size="32"/>
```

### **WorkspaceWithReports Control**
Complex control for displaying workspaces with expandable reports.

```csharp
// Controls/WorkspaceWithReports.axaml.cs
public class WorkspaceWithReports : UserControl
{
    public static readonly StyledProperty<Workspace> WorkspaceProperty;
    public static readonly StyledProperty<bool> IsSelectedProperty;
    public static readonly StyledProperty<bool> IsExpandedProperty;
}
```

**Usage:**
```xml
<controls:WorkspaceWithReports 
    Workspace="{Binding}"
    IsSelected="{Binding IsSelected}"
    IsExpanded="{Binding IsExpanded}"/>
```

## 🎨 Styling System

### **AppDefaultStyles.axaml**
Global styles applied throughout the application.

```xml
<!-- Styles/AppDefaultStyles.axaml -->
<Styles xmlns="https://github.com/avaloniaui">
    
    <!-- TextBox Styling -->
    <Style Selector="TextBox">
        <Setter Property="FontFamily" Value="{DynamicResource InterRegular}"/>
        <Setter Property="FontSize" Value="14" />
        <Setter Property="BorderThickness" Value="1" />
        <Setter Property="BorderBrush" Value="{DynamicResource NeutralMuted}" />
        <Setter Property="Foreground" Value="{DynamicResource Foreground}" />
        <Setter Property="CornerRadius" Value="8" />
        <Setter Property="Padding" Value="12 6" />
        <Setter Property="Background" Value="Transparent" />
    </Style>
    
    <!-- Button Styling -->
    <Style Selector="Button">
        <Setter Property="FontFamily" Value="{DynamicResource InterRegular}"/>
        <Setter Property="Foreground" Value="{DynamicResource Foreground}"/>
        <Setter Property="FontSize" Value="14" />
        <Setter Property="BorderThickness" Value="0" />
        <Setter Property="Cursor" Value="Hand" />
    </Style>
    
    <!-- Custom Button Variants -->
    <Style Selector="Button.PopupBackground">
        <Setter Property="Background" Value="{DynamicResource ForegroundSemiTransparent}" />
        <Setter Property="CornerRadius" Value="0" />
        <Setter Property="VerticalAlignment" Value="Stretch" />
        <Setter Property="HorizontalAlignment" Value="Stretch" />
        <Setter Property="Cursor" Value="Arrow" />
    </Style>
    
    <!-- Disabled State -->
    <Style Selector="Button.Disabled, TextBox.Disabled">
        <Setter Property="Opacity" Value="0.5"/>
        <Setter Property="IsHitTestVisible" Value="False"/>
    </Style>
</Styles>
```

### **Color System**
Dynamic resource colors for consistent theming.

```xml
<!-- App.axaml -->
<Application.Resources>
    <!-- Light Theme Colors -->
    <SolidColorBrush x:Key="Background" Color="#FFFFFF"/>
    <SolidColorBrush x:Key="Foreground" Color="#1A1A1A"/>
    <SolidColorBrush x:Key="ForegroundSecondary" Color="#666666"/>
    <SolidColorBrush x:Key="NeutralMuted" Color="#E5E5E5"/>
    <SolidColorBrush x:Key="ForegroundSemiTransparent" Color="#80000000"/>
    
    <!-- Status Colors -->
    <SolidColorBrush x:Key="SuccessColor" Color="#28A745"/>
    <SolidColorBrush x:Key="WarningColor" Color="#FFC107"/>
    <SolidColorBrush x:Key="ErrorColor" Color="#DC3545"/>
    <SolidColorBrush x:Key="InfoColor" Color="#17A2B8"/>
</Application.Resources>
```

## 📱 Layout Patterns

### **Main Layout Structure**
```xml
<!-- Views/MainView.axaml -->
<Grid RowDefinitions="56, *">
    <Grid.ColumnDefinitions>
        <ColumnDefinition MinWidth="250" MaxWidth="320" Width="2*" />
        <ColumnDefinition Width="5*" />
    </Grid.ColumnDefinitions>
    
    <!-- Header -->
    <Grid Grid.Row="0" Grid.Column="0" Grid.ColumnSpan="2">
        <!-- Header content -->
    </Grid>
    
    <!-- Sidebar -->
    <Grid Grid.Row="1" Grid.Column="0">
        <!-- Workspace list -->
    </Grid>
    
    <!-- Main Content -->
    <Grid Grid.Row="1" Grid.Column="1">
        <!-- Report list -->
    </Grid>
    
    <!-- Popups (Overlay) -->
    <Grid Grid.Row="0" Grid.Column="0" Grid.ColumnSpan="2" Grid.RowSpan="2" ZIndex="1">
        <!-- Popup views -->
    </Grid>
    
    <!-- Toast (Overlay) -->
    <Grid Grid.Row="0" Grid.Column="0" Grid.ColumnSpan="2" Grid.RowSpan="2" ZIndex="2">
        <!-- Toast notifications -->
    </Grid>
</Grid>
```

### **Popup Layout Pattern**
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
        <ScrollViewer Grid.Row="1" Margin="0 24">
            <StackPanel Spacing="16">
                <!-- Dynamic content -->
            </StackPanel>
        </ScrollViewer>
        
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

## 🔧 Creating Custom Controls

### **Step 1: Create Control Class**
```csharp
// Controls/MyCustomControl.axaml.cs
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AutoPBI.Controls;

public class MyCustomControl : TemplatedControl
{
    // Define dependency properties
    public static readonly StyledProperty<string> TextProperty = 
        AvaloniaProperty.Register<MyCustomControl, string>(nameof(Text));

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
    
    // Additional properties...
}
```

### **Step 2: Create XAML Template**
```xml
<!-- Controls/MyCustomControl.axaml -->
<Styles xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    
    <Style Selector="controls|MyCustomControl">
        <Setter Property="Template">
            <ControlTemplate>
                <Border Background="{TemplateBinding Background}"
                        BorderBrush="{TemplateBinding BorderBrush}"
                        BorderThickness="{TemplateBinding BorderThickness}"
                        CornerRadius="{TemplateBinding CornerRadius}">
                    
                    <StackPanel Orientation="Horizontal" Spacing="8">
                        <TextBlock Text="{TemplateBinding Text}"
                                  Foreground="{TemplateBinding Foreground}"/>
                        <!-- Additional content -->
                    </StackPanel>
                </Border>
            </ControlTemplate>
        </Setter>
    </Style>
</Styles>
```

### **Step 3: Register in App.axaml**
```xml
<!-- App.axaml -->
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <!-- ... other dictionaries -->
            <ResourceDictionary Source="Controls/MyCustomControl.axaml"/>
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

## 🎯 Common UI Patterns

### **Loading States**
```xml
<!-- Loading overlay -->
<Border IsVisible="{Binding IsLoading}"
        Background="{DynamicResource ForegroundSemiTransparent}"
        ZIndex="1000">
    <StackPanel HorizontalAlignment="Center" 
                VerticalAlignment="Center"
                Spacing="16">
        <ProgressRing IsIndeterminate="True"/>
        <TextBlock Text="Loading..." 
                  HorizontalAlignment="Center"/>
    </StackPanel>
</Border>
```

### **Empty States**
```xml
<!-- Empty state -->
<StackPanel IsVisible="{Binding IsEmpty}"
            HorizontalAlignment="Center"
            VerticalAlignment="Center"
            Spacing="16">
    <TextBlock Text="&#xf119;" 
              FontFamily="{DynamicResource FontAwesome}"
              FontSize="48"
              Foreground="{DynamicResource ForegroundSecondary}"
              HorizontalAlignment="Center"/>
    <TextBlock Text="No items found"
              FontSize="16"
              Foreground="{DynamicResource ForegroundSecondary}"
              HorizontalAlignment="Center"/>
</StackPanel>
```

### **Error States**
```xml
<!-- Error state -->
<StackPanel IsVisible="{Binding HasError}"
            HorizontalAlignment="Center"
            VerticalAlignment="Center"
            Spacing="16">
    <TextBlock Text="&#xf071;" 
              FontFamily="{DynamicResource FontAwesome}"
              FontSize="48"
              Foreground="{DynamicResource ErrorColor}"
              HorizontalAlignment="Center"/>
    <TextBlock Text="{Binding ErrorMessage}"
              FontSize="16"
              Foreground="{DynamicResource ErrorColor}"
              HorizontalAlignment="Center"/>
    <Button Content="Retry"
            Command="{Binding RetryCommand}"/>
</StackPanel>
```

## 📊 Data Binding Patterns

### **Two-Way Binding**
```xml
<TextBox Text="{Binding InputText}"/>
<CheckBox IsChecked="{Binding IsEnabled}"/>
<Slider Value="{Binding Progress}"/>
```

### **Command Binding**
```xml
<Button Command="{Binding ExecuteCommand}"
        IsEnabled="{Binding CanExecute}"/>
```

### **Collection Binding**
```xml
<ItemsControl ItemsSource="{Binding Items}">
    <ItemsControl.ItemTemplate>
        <DataTemplate>
            <TextBlock Text="{Binding Name}"/>
        </DataTemplate>
    </ItemsControl.ItemTemplate>
</ItemsControl>
```

### **Visibility Binding**
```xml
<Border IsVisible="{Binding IsVisible}">
    <!-- Content -->
</Border>
```

## 🎨 Theming Support

### **Theme Switching**
```csharp
// In MainViewModel
[RelayCommand]
private void ToggleTheme()
{
    if (Application.Current!.ActualThemeVariant == ThemeVariant.Light)
    {
        Application.Current.RequestedThemeVariant = ThemeVariant.Dark;
    }
    else
    {
        Application.Current.RequestedThemeVariant = ThemeVariant.Light;
    }
}
```

### **Theme-Aware Colors**
```xml
<!-- Use dynamic resources for theme-aware colors -->
<SolidColorBrush x:Key="Background" Color="{DynamicResource SystemBackgroundColor}"/>
<SolidColorBrush x:Key="Foreground" Color="{DynamicResource SystemForegroundColor}"/>
```

## 🔧 Best Practices

### **1. Use Dynamic Resources**
```xml
<!-- Good -->
<TextBlock Foreground="{DynamicResource Foreground}"/>

<!-- Avoid hardcoded colors -->
<TextBlock Foreground="Black"/>
```

### **2. Consistent Spacing**
```xml
<!-- Use consistent spacing values -->
<StackPanel Spacing="16">
    <TextBlock Text="Item 1"/>
    <TextBlock Text="Item 2"/>
</StackPanel>
```

### **3. Responsive Design**
```xml
<!-- Use Grid with proportional columns -->
<Grid ColumnDefinitions="*, 2*, *">
    <!-- Content adapts to available space -->
</Grid>
```

### **4. Accessibility**
```xml
<!-- Include accessibility properties -->
<Button Content="Submit"
        AutomationProperties.Name="Submit Button"
        AutomationProperties.HelpText="Click to submit the form"/>
```

### **5. Performance**
```xml
<!-- Use virtualization for large lists -->
<ItemsControl ItemsSource="{Binding LargeCollection}">
    <ItemsControl.ItemsPanel>
        <ItemsPanelTemplate>
            <VirtualizingStackPanel/>
        </ItemsPanelTemplate>
    </ItemsControl.ItemsPanel>
</ItemsControl>
```

---

**This UI component system provides a consistent and maintainable foundation for building user interfaces in AutoPBI. Follow these patterns to ensure your UI components integrate seamlessly with the existing design system.** 