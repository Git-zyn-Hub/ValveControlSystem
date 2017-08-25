using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ValveControlSystem
{
    public class UserName2VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string userName = value as String;
            if (userName != null)
            {
                if (userName == "admin")
                {
                    return Visibility.Visible;
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TestConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class Password2CircleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string password = value.ToString();
            string result = string.Empty;
            for (int i = 0; i < password.Length; i++)
            {
                result += "●";
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class Culture2WidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string cultureBind = Thread.CurrentThread.CurrentUICulture.Name.ToString();
            if (cultureBind == "zh-CN")
            {
                return 60;
            }
            else if (cultureBind == "en-US")
            {
                return 120;
            }
            else
            {
                return 120;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ConfigWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string cultureBind = Thread.CurrentThread.CurrentUICulture.Name.ToString();
            if (cultureBind == "zh-CN")
            {
                return 480;
            }
            else if (cultureBind == "en-US")
            {
                return 600;
            }
            else
            {
                return 600;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 这个绑定不好使，不知道为什么。ConfigWidthConverter好使。好奇怪呀！
    /// </summary>
    public class CurveNameColumnWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string cultureBind = Thread.CurrentThread.CurrentUICulture.Name.ToString();
            if (cultureBind == "zh-CN")
            {
                return 91;
            }
            else if (cultureBind == "en-US")
            {
                return 180;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Color2RectangleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color? color = value as Color?;
            if (color.HasValue)
            {
                Rectangle newRect = new Rectangle();
                newRect.Fill = new SolidColorBrush(color.Value);
                newRect.Height = 20;
                newRect.Width = 60;
                newRect.Margin = new Thickness(0);
                return newRect;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Rectangle rect = value as Rectangle;
            Brush brush = rect.Fill;
            Color color = (Color)ColorConverter.ConvertFromString(brush.ToString());
            return color;
        }
    }

    public class Color2BrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color? color = value as Color?;
            if (color.HasValue)
            {
                return new SolidColorBrush(color.Value);
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Unit2UnitListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string unit = value as string;
            if (unit == "PSI" || unit == "MPa")
            {
                List<string> units = new List<string>();
                units.Add("PSI");
                units.Add("MPa");
                return units;
            }
            else if (unit == "摄氏度" || unit == "华氏度")
            {
                List<string> units = new List<string>();
                units.Add("℃");
                units.Add("℉");
                return units;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class UnitConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string unit = value as string;
            if (unit == "PSI" || unit == "MPa")
            {
                return unit;
            }
            else if (unit == "摄氏度" || unit == "华氏度")
            {
                if (unit == "摄氏度")
                {
                    return "℃";
                }
                else
                {
                    return "℉";
                }
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string unit = value as string;
            if (unit == "PSI" || unit == "MPa")
            {
                return unit;
            }
            else if (unit == "℃" || unit == "℉")
            {
                if (unit == "℃")
                {
                    return "摄氏度";
                }
                else
                {
                    return "华氏度";
                }
            }
            else
            {
                return null;
            }
        }
    }

    public class CurveSetWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string cultureBind = Thread.CurrentThread.CurrentUICulture.Name.ToString();
            if (cultureBind == "zh-CN")
            {
                return 500;
            }
            else if (cultureBind == "en-US")
            {
                return 600;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ColumnWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //string cultureBind = Thread.CurrentThread.CurrentUICulture.Name.ToString();
            string cultureBind = value as string;
            if (cultureBind == "zh-CN")
            {
                return 150;
            }
            else if (cultureBind == "en-US")
            {
                return 200;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StaffWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string cultureBind = value as string;
            if (cultureBind == "zh-CN")
            {
                return new Thickness(24, 0, 0, 0);
            }
            else if (cultureBind == "en-US")
            {
                return new Thickness(0);
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class WellDepthWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string cultureBind = value as string;
            if (cultureBind == "zh-CN")
            {
                return new Thickness(30, 0, 0, 0);
            }
            else if (cultureBind == "en-US")
            {
                return new Thickness(0);
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class LayerWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string cultureBind = value as string;
            if (cultureBind == "zh-CN")
            {
                return new Thickness(12, 0, 0, 0);
            }
            else if (cultureBind == "en-US")
            {
                return new Thickness(0);
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string cultureBind = Thread.CurrentThread.CurrentUICulture.Name.ToString();
            DateTime time = (DateTime)value;
            if (cultureBind == "zh-CN")
            {
                return time.ToString("yyyy/MM/dd HH:mm:ss");
            }
            else if (cultureBind == "en-US")
            {
                return time.ToString("MM/dd/yyyy HH:mm:ss");
            }
            else
            {
                return time.ToString("MM/dd/yyyy HH:mm:ss");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IsChecked2VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isChecked = (bool)value;
            if (isChecked)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
