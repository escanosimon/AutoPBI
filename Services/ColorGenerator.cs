using System.Security.Cryptography;
using System.Text;
using Avalonia.Media;

namespace AutoPBI.Services;

public static class ColorGenerator
{
    public static IBrush GenerateColor(string str)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(str));

        var r = hash[0];  // Red
        var g = hash[1];  // Green
        var b = hash[2];  // Blue

        return new SolidColorBrush(Color.FromArgb(255, r, g, b));
    }
}