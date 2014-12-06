// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF 
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
// PARTICULAR PURPOSE. 
// 
// Copyright (c) Microsoft Corporation. All rights reserved 
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using Newtonsoft.Json;

using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;

using SoundCloud.Common;

namespace SoundCloud.DataProviders
{
    public class SoundCloudProvider
    {
        //Endpoint to RESOLVE a track id or playlist id
        const string RESOLVE_URL_ENDPOINT = "http://api.soundcloud.com/resolve.json?url={0}&client_id={1}";

        private Uri _uri;

        public SoundCloudProvider(string clientId, string resolveUrl)
        {
            string url = String.Format(RESOLVE_URL_ENDPOINT, resolveUrl, clientId);
            _uri = new Uri(url);
        }

        public async Task<T> LoadTrack<T>()
        {
            try
            {
                string data = await DownloadAsync(_uri);
                return JsonConvert.DeserializeObject<T>(data);
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("SoundCloudDataProvider.LoadTrack", ex);
                return default(T);
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
