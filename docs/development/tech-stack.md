# Tech Stack

AutoPBI is built using modern .NET technologies with a focus on cross-platform desktop development and Power BI integration.

## 🎯 Core Technologies

### **.NET 9.0**
- **Target Framework**: `net9.0`
- **Language**: C# with nullable reference types enabled
- **Type**: Windows Executable (`WinExe`)

### **Avalonia UI 11.3.2**
- **Cross-platform UI framework** for .NET
- **XAML-based** UI definitions
- **MVVM pattern** support
- **Custom controls** and styling

### **CommunityToolkit.Mvvm 8.4.0**
- **MVVM implementation** with source generators
- **ObservableObject** for property change notifications
- **RelayCommand** for command binding
- **ObservableProperty** attributes for automatic property generation

## 📦 Key Dependencies

### **UI & Controls**
```xml
<PackageReference Include="Avalonia" Version="11.3.2" />
<PackageReference Include="Avalonia.Controls.DataGrid" Version="11.3.2" />
<PackageReference Include="Avalonia.Themes.Fluent" Version="11.3.2" />
<PackageReference Include="Avalonia.Fonts.Inter" Version="11.3.2" />
<PackageReference Include="Avalonia.Svg.Skia" Version="11.3.0" />
<PackageReference Include="LoadingIndicators.Avalonia" Version="11.0.11.1" />
```

### **PowerShell Integration**
```xml
<PackageReference Include="Microsoft.PowerShell.SDK" Version="7.5.2" />
<PackageReference Include="System.Management.Automation" Version="7.5.2" />
<PackageReference Include="CliWrap" Version="3.9.0" />
```

### **Security & Utilities**
```xml
<PackageReference Include="SecureStore" Version="1.2.2" />
<PackageReference Include="TextCopy" Version="6.2.1" />
```

## 🏗️ Architecture Overview

### **MVVM Pattern**
```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│      Views      │    │   ViewModels     │    │     Models      │
│   (UI Layer)    │◄──►│  (Logic Layer)   │◄──►│  (Data Layer)   │
│                 │    │                  │    │                 │
│ • MainView      │    │ • MainViewModel  │    │ • User          │
│ • PopupViews    │    │ • PopupViewModels│    │ • Workspace     │
│ • CustomControls│    │ • ViewModelBase  │    │ • Report        │
└─────────────────┘    └──────────────────┘    │ • Dataset       │
                                               └─────────────────┘
```

### **Service Layer**
```
┌─────────────────────────────────────────────────────────────┐
│                    Services                                │
├─────────────────────────────────────────────────────────────┤
│ • Psr.cs                    - PowerShell wrapper          │
│ • DialogService.cs          - File/folder dialogs         │
│ • SecureStorageService.cs   - Encrypted credential storage│
│ • ObservableHashMap.cs      - Observable dictionary       │
│ • ColorGenerator.cs         - User avatar colors          │
└─────────────────────────────────────────────────────────────┘
```

## 🎨 UI Framework Details

### **Avalonia UI Features Used**
- **Templated Controls** - Custom controls like Toast, StatusIcon
- **Data Binding** - Two-way binding with ViewModels
- **Styles & Themes** - Custom styling with Fluent theme
- **Fonts** - Inter font family for modern typography
- **SVG Support** - Power BI logo and icons
- **Loading Indicators** - Progress indicators for operations

### **Custom Controls**
```csharp
// Example: Toast Control
public class Toast : TemplatedControl
{
    public enum StatusType { Normal, Success, Warning, Error }
    public static readonly StyledProperty<StatusType> StatusProperty;
    public static readonly StyledProperty<string> TitleProperty;
    public static readonly StyledProperty<string> DescriptionProperty;
}
```

### **Styling System**
```xml
<!-- AppDefaultStyles.axaml -->
<Style Selector="TextBox">
    <Setter Property="FontFamily" Value="{DynamicResource InterRegular}"/>
    <Setter Property="FontSize" Value="14" />
    <Setter Property="CornerRadius" Value="8" />
    <Setter Property="Padding" Value="12 6" />
</Style>
```

## 🔧 PowerShell Integration

### **PowerShell SDK**
- **Microsoft.PowerShell.SDK** - Core PowerShell functionality
- **System.Management.Automation** - PowerShell cmdlet execution
- **CliWrap** - Command-line interface wrapper

### **Power BI Operations**
```csharp
// Example: PowerShell command execution
await Psr.Wrap()
    .WithArguments(args => args.Add("Connect-PowerBIServiceAccount"))
    .WithArguments(args => args.Add("-Credential").Add(credential))
    .ExecuteAsync();
```

## 🔐 Security Features

### **Credential Management**
- **SecureStore** - Encrypted credential storage
- **Local storage** - AppData folder for user data
- **Auto-login** - Remember me functionality

### **Error Handling**
```csharp
// Error message mapping
public Dictionary<string, string> ErrorMessages = new()
{
    ["DMTS_MonikerWithUnboundDataSources"] = "We are unable to access some data source...",
    ["ModelRefreshFailed_CredentialsNotSpecified"] = "It looks like scheduled refresh failed...",
    ["ModelRefresh_ShortMessage_ServiceError"] = "The dataset refresh failed due to a temporary Power BI service issue..."
};
```

## 📱 Platform Support

### **Current Platform**
- **Windows** - Primary target platform
- **Desktop Application** - Native Windows executable
- **PowerShell Integration** - Windows-specific PowerShell cmdlets

### **Future Considerations**
- **Cross-platform** - Avalonia UI supports multiple platforms
- **Linux/macOS** - Potential future expansion
- **Web API** - Alternative to PowerShell integration

## 🛠️ Development Tools

### **IDE Support**
- **Visual Studio 2022** - Primary development environment
- **JetBrains Rider** - Alternative IDE
- **Avalonia Designer** - XAML preview and design

### **Debugging**
- **Avalonia.Diagnostics** - UI debugging tools
- **PowerShell debugging** - Command execution tracing
- **Logging** - Console output for operations

## 📊 Performance Considerations

### **Memory Management**
- **ObservableCollections** - Efficient UI updates
- **CancellationTokenSource** - Operation cancellation
- **Disposable patterns** - Resource cleanup

### **UI Responsiveness**
- **Async operations** - Non-blocking UI
- **Progress indicators** - User feedback
- **Background processing** - Heavy operations off UI thread

---

**This tech stack provides a solid foundation for desktop application development with modern .NET technologies and cross-platform UI capabilities.** 