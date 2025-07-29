using System;
using System.Globalization;
using AutoPBI.Controls;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace AutoPBI.Services
{
    public class StatusToBackgroundConverter : IValueConverter
    {
        public static readonly StatusToBackgroundConverter Instance = new();

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var app = Avalonia.Application.Current;
            if (app == null) return null!;

            var res = app.Resources;
            var themedRes = res.ThemeDictionaries[app.ActualThemeVariant] as ResourceDictionary;
            
            if (value is StatusIcon.StatusType status)
            {
                return (status switch
                {
                    StatusIcon.StatusType.Success => themedRes?["SuccessBackground"] as IBrush,
                    StatusIcon.StatusType.Warning => themedRes?["WarningBackground"] as IBrush,
                    StatusIcon.StatusType.Error => themedRes?["ErrorBackground"] as IBrush,
                    _ => res["NeutralMuted"] as IBrush
                })!;
            }
            return (res["NeutralMuted"] as IBrush)!;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}