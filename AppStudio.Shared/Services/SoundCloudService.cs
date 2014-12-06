// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF 
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
// PARTICULAR PURPOSE. 
// 
// Copyright (c) Microsoft Corporation. All rights reserved 
using System;
using System.Threading.Tasks;

using Windows.System;

using AppStudio.Data.SoundCloud; //Schema

namespace AppStudio.Services
{
    static public class SoundCloudMusicService
    {
        //Get your client key at developers.soundcloud.com
        private const string _clientId = "YOUR_KEY_HERE";

        public static SoundCloudTrackSchema trackModel { get; private set; }

        /// <summary>
        /// Used to "Launch" the track in the media player. No background audio.
        /// </summary>
        /// <param name="trackUrl"></param>
        /// <returns></returns>
        public static async Task<string> LaunchTrack(string trackUrl)
        {
            try
            {
                var serviceProvider = new AppStudio.Data.SoundCloudProvider(_clientId, trackUrl);
                SoundCloudTrackSchema track = await serviceProvider.LoadTrack<SoundCloudTrackSchema>();

                return string.Format("{0}?client_id={1}", track.stream_url, _clientId);

            }
            catch (Exception ex)
            {
                AppLogs.WriteError("SoundCloudMusicService", ex.ToString());
                return "";
            }

        }

        /// <summary>
        /// Returns the full Track JSON in object format
        /// </summary>
        /// <param name="trackUrl"></param>
        /// <returns></returns>
        public static async Task<SoundCloudTrackSchema> GetTrackInfo(string trackUrl)
        {
            try
            {
                var serviceProvider = new AppStudio.Data.SoundCloudProvider(_clientId, trackUrl);
                return await serviceProvider.LoadTrack<SoundCloudTrackSchema>();
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("SoundCloudMusicService", ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Returns the streaming url with the auth in the request
        /// </summary>
        /// <param name="streamUrl"></param>
        /// <returns></returns>
        public static String AuthUrl(string streamUrl)
        {
            return string.Format("{0}?client_id={1}", streamUrl, _clientId);
        }

    }
}
