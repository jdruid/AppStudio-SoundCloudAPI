using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace AppStudio.Data
{
    public class ServiceDataProvider
    {
        const string DATASERVICES_URL_PATTERN = "http://ds.winappstudio.com/api/data/collection?dataRowListId={0}";

        private Uri _uri;

        public ServiceDataProvider(string appId, string dataSourceName, int pageIndex = 0, int blockSize = 40)
        {
            string url = String.Format(DATASERVICES_URL_PATTERN, dataSourceName, appId);
            url = url + String.Format("&pageIndex={0}&blockSize={1}", pageIndex, blockSize);
            _uri = new Uri(url);
        }

        public async Task<IEnumerable<T>> Load<T>()
        {
            try
            {
                string data = await DownloadAsync(_uri);
                IEnumerable<T> records = JsonConvert.DeserializeObject<IEnumerable<T>>(data);
                return records;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("ServiceDataProvider.Load", ex);
                return null;
            }
        }

        public async Task<string> DownloadAsync(Uri url)
        {
            HttpClient client = new HttpClient();
            var message = new HttpRequestMessage(HttpMethod.Get, url);
            
            using (var response = await client.SendAsync(message))
            {
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
