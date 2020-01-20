using Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Console_Application
{
    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new SolidColorBrush((Color)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as SolidColorBrush).Color;
        }
    }
    public class TypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is LogType)) return null;
            LogType type = (LogType)value;
            switch (type)
            {
                case LogType.Information:
                    return new SolidColorBrush(Colors.Black);
                case LogType.Error:
                    return new SolidColorBrush(Colors.DarkRed);
                case LogType.Debug:
                    return new SolidColorBrush(Colors.Purple);
                case LogType.Warning:
                    return new SolidColorBrush(Colors.Orange);
                case LogType.Connection:
                    return new SolidColorBrush(Colors.Green);
                case LogType.TransmissionOut:
                    return new SolidColorBrush(Colors.LightBlue);
                case LogType.TransmissionIn:
                    return new SolidColorBrush(Colors.LightGreen);
                default:
                    return new SolidColorBrush(Colors.Black);
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Color)) return null;
            Color color = (Color)value;
            if (color == Colors.Black)
                return LogType.Information;
            else if (color == Colors.DarkRed)
                return LogType.Error;
            else if (color == Colors.Purple)
                return LogType.Debug;
            else if (color == Colors.Orange)
                return LogType.Warning;
            else if (color == Colors.Green)
                return LogType.Connection;
            else if (color == Colors.LightBlue)
                return LogType.TransmissionOut;
            else if (color == Colors.LightGreen)
                return LogType.TransmissionIn;
            else
                return null;
        }
    }
}
