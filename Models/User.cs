using System.Linq;
using AutoPBI.Services;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoPBI.Models;

public class User : ObservableObject
{
    private string? _environment;
    private string? _tenantId;
    private string? _userName;
    private string[] _initials = [];
    private IBrush? _color;
    
    public User(string? environment, string? tenantId, string? userName)
    {
        Environment = environment;
        TenantId = tenantId;
        UserName = userName;
        Initials = ExtractInitials(userName!);
        Color = ColorGenerator.GenerateColor(userName!);
    }
    
    public string? Environment
    {
        get => _environment;
        set => SetProperty(ref _environment, value);
    }
    
    public string? TenantId
    {
        get => _tenantId;
        set => SetProperty(ref _tenantId, value);
    }
    
    public string? UserName
    {
        get => _userName;
        set => SetProperty(ref _userName, value);
    }
    
    public string[] Initials
    {
        get => _initials;
        set => SetProperty(ref _initials, value);
    }
    
    static string[] ExtractInitials(string userName)
    {
        var localPart = userName.Split('@')[0];
        var names = localPart.Split('.');
        var initials = names.Select(name => name[0].ToString().ToUpper()).ToArray();

        return initials;
    }
    
    public IBrush? Color
    {
        get => _color;
        set => SetProperty(ref _color, value);
    }
}