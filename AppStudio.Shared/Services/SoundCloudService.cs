// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF 
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
// PARTICULAR PURPOSE. 
// 
// Copyright (c) Microsoft Corporation. All rights reserved 
using System;
using System.Threading.Tasks;

using Windows.System;

using AppStudio.Data.SoundCloud;

namespace AppStudio.Services
{
    static public class SoundCloudMusicService
    {
        //Get your client key at developers.soundcloud.com
        private const string _clientId = "your-key-here";

        public static SoundCloudTrackSchema trackModel { get; private set; }

        static public async Task<string> LaunchTrack(string trackUrl)
        {
            try
            {
                var serviceProvider = new AppStudio.Data.SoundCloudDataProvider(_clientId, trackUrl);
                SoundCloudTrackSchema track = await serviceProvider.LoadTrack<SoundCloudTrackSchema>();

                return string.Format("{0}?client_id={1}", track.stream_url, _clientId);

            }
            catch (Exception ex)
            {
                AppLogs.WriteError("SoundCloudMusicService", ex.ToString());
                return "";
            }

        }

    }
}
