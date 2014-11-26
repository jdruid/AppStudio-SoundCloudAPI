using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.Storage.Streams;

namespace AppStudio.Data
{
    public class StaticHtmlDataProvider
    {
        private Uri _uri;

        public StaticHtmlDataProvider(string htmlPath)
        {
            string url = String.Format("ms-appx://{0}", htmlPath);
            _uri = new Uri(url);
        }

        public async Task<IEnumerable<HtmlSchema>> Load()
        {
            try
            {
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(_uri);
                IRandomAccessStreamWithContentType randomStream = await file.OpenReadAsync();

                using (StreamReader r = new StreamReader(randomStream.AsStreamForRead()))
                {
                    string data = await r.ReadToEndAsync();
                    Collection<HtmlSchema> records = new Collection<HtmlSchema>();
                    records.Add(new HtmlSchema() { Content = data });
                    return records;
                }
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("ServiceDataProvider.Load", ex);
                return null;
            }
        }
    }
}
