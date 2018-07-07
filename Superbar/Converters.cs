using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using Start9.Api;
using Start9.Api.Tools;

namespace Superbar
{
    public class IconToImageBrushConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType,
            Object parameter, CultureInfo culture)
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

        public Object ConvertBack(Object value, Type targetType,
            Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class HwndToIsActiveWindowConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType,
            Object parameter, CultureInfo culture)
        {
            return (IntPtr) value == WinApi.GetForegroundWindow();
        }

        public Object ConvertBack(Object value, Type targetType,
            Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}