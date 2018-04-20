using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using Start9.Api.Tools;

namespace Superbar
{
    public class IconToImageBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            try
            {
                return new ImageBrush((value as Icon).ToBitmapSource());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return new ImageBrush();
            }
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class HwndToIsActiveWindowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if ((IntPtr)(value) == WinApi.GetForegroundWindow())
                return true;
            else return false;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}