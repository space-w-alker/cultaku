using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace culTAKU.Misc.Converters
{
    public class ImageUriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if((string)value == "Image/AnimeImage-387.jpg")
            {
                int j = 0;
            }

            if (value == null || !File.Exists((string)value))
            {
                return Path.GetFullPath("Icons/unknown.png");   
            }

            return Path.GetFullPath((string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
