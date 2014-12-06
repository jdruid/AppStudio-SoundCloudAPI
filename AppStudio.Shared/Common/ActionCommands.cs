// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF 
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
// PARTICULAR PURPOSE. 
// 
// Copyright (c) Microsoft Corporation. All rights reserved 

using System;
using System.Windows.Input;

using AppStudio.Services;

namespace AppStudio
{
    public class ActionCommands
    {
        static public ICommand ShowImage
        {
            get
            {
                return new RelayCommandEx<string>((param) =>
                {
                    if (!String.IsNullOrEmpty(param))
                    {
                        NavigationServices.NavigateToPage("ImageViewer", param);
                    }
                });
            }
        }

        static public ICommand MailTo
        {
            get
            {
                return new RelayCommandEx<string>((param) =>
                {
                    if (!String.IsNullOrEmpty(param))
                    {
                        string url = String.Format("mailto:{0}", param);
                        NavigationServices.NavigateTo(new Uri(url));
                    }
                });
            }
        }

        static public ICommand CallToPhone
        {
            get
            {
                return new RelayCommandEx<string>((param) =>
                {
                    if (!String.IsNullOrEmpty(param))
                    {
                        string url = String.Format("tel:{0}", param);
                        NavigationServices.NavigateTo(new Uri(url));
                    }
                });
            }
        }
        static public ICommand NavigateToUrl
        {
            get
            {
                return new RelayCommandEx<string>((param) =>
                {
                    if (!String.IsNullOrEmpty(param))
                    {
                        string url =  param;
                        NavigationServices.NavigateTo(new Uri(url));
                    }
                });
            }
        }

        static public ICommand MusicPlayArtistMix
        {
            get
            {
                return new RelayCommandEx<string>(async (param) =>
                {
                    if (!String.IsNullOrEmpty(param))
                    {
                        await NokiaMusicServices.PlayArtistMix(param);
                    }
                });
            }
        }

        static public ICommand MusicLaunchSearch
        {
            get
            {
                return new RelayCommandEx<string>(async (param) =>
                {
                    if (!String.IsNullOrEmpty(param))
                    {
                        await NokiaMusicServices.LaunchSearch(param);
                    }
                });
            }
        }

        static public ICommand MusicLaunchArtist
        {
            get
            {
                return new RelayCommandEx<string>(async (param) =>
                {
                    if (!String.IsNullOrEmpty(param))
                    {
                        await NokiaMusicServices.LaunchArtist(param);
                    }
                });
            }
        }
        static public ICommand MapsPosition
        {
            get
            {
                return new RelayCommandEx<string>(async (param) =>
                {
                    if (!String.IsNullOrEmpty(param))
                    {
                        await NokiaMapsServices.MapPosition(param);
                    }
                });
            }
        }

        static public ICommand MapsHowToGet
        {
            get
            {
                return new RelayCommandEx<string>(async (param) =>
                {
                    if (!String.IsNullOrEmpty(param))
                    {
                        await NokiaMapsServices.HowToGet(param);
                    }
                });
            }
        }

        private static void NavigateTo(string protocol, string param)
        {
            if (!String.IsNullOrEmpty(param))
            {
                string url = String.Format("{0}:{1}", protocol, param);
                var uri = new Uri(url);
                NavigationServices.NavigateTo(uri);
            }
        }

        /// <summary>
        /// SoundCloudLaunchTrack take the paramater from the 'click' and looks up the proper track id and returns
        /// the stream url to launch in the WebBrowser
        /// </summary>
        static public ICommand SoundCloudLaunchTrack
        {
            get
            {
                return new RelayCommandEx<string>(async (param) =>
                {
                    if (!String.IsNullOrEmpty(param))
                    {
                        string streamUrl = await SoundCloudMusicService.LaunchTrack(param);
                        NavigationServices.NavigateTo(new Uri(streamUrl));
                    }

                });
            }
        }

       

      
    }
}
