using System;
using System.Diagnostics;

using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace AppStudio.Controls
{
    public class ThumbnailConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value != null && value.ToString() != String.Empty)
                {
                    Uri uri = null;
                    string url = value.ToString();
                    if (url.StartsWith("/"))
                    {
                        uri = new Uri(string.Concat("ms-appx://", url), UriKind.Absolute);
                    }
                    else
                    {
                        uri = new Uri(url, UriKind.Absolute);
                    }
                    var bm = new BitmapImage(uri)
                    {
                        CreateOptions = BitmapCreateOptions.None,
                        DecodePixelHeight = System.Convert.ToInt32(parameter)
                    };

                    return bm;
                }
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("ThumbnailConverter.Convert", ex);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
