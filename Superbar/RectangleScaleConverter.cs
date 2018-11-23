using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Start9.UI.Wpf.Converters;

namespace Superbar
{
    public class RectangleScaleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double initWidth = ConversionInputHelper.GetDouble(values[0]);
            double initHeight = ConversionInputHelper.GetDouble(values[1]);

            string[] parameters = parameter.ToString().Split(',');

            double maxWidth = ConversionInputHelper.GetDouble(parameters[0]);
            double maxHeight = ConversionInputHelper.GetDouble(parameters[1]);
            bool resultAxis = parameters.Length > 2;
            /*string outAxis = "resultAxis: " + resultAxis.ToString();
            if (resultAxis)
                outAxis = outAxis + ", " + parameters[2];

            Debug.WriteLine(outAxis);*/

            double width = initWidth;
            double height = initHeight;

            while ((width > maxWidth) | (height > maxHeight))
            {
                width -= (initWidth / initHeight);
                height--; //-= (initHeight / initWidth);
            }

            if (resultAxis)
                return height;
            else
                return width;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}