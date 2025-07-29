using System;
using System.Globalization;
using AutoPBI.Controls;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace AutoPBI.Services
{
    public class StatusToForegroundConverter : IValueConverter
    {
        public static readonly StatusToForegroundConverter Instance = new();

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var app = Avalonia.Application.Current;
            if (app == null) return null!;

            var res = app.Resources;
            
            if (value is StatusIcon.StatusType status)
            {
                return (status switch
                {
                    StatusIcon.StatusType.Success => res["Success"] as IBrush,
                    StatusIcon.StatusType.Warning => res["Warning"] as IBrush,
                    StatusIcon.StatusType.Error => res["Error"] as IBrush,
                    _ => res["Foreground"] as IBrush
                })!;
            }
            return (res["Foreground"] as IBrush)!;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}