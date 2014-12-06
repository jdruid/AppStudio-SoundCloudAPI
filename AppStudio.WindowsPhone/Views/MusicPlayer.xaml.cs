// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF 
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
// PARTICULAR PURPOSE. 
// 
// Copyright (c) Microsoft Corporation. All rights reserved 
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using System.Diagnostics;
using Windows.UI.Core;
using Windows.Media.Playback;

using BackgroundAudioTask;

using Windows.ApplicationModel.DataTransfer; //DataTransferManager
using AppStudio.ViewModels;//TracksViewModel
using AppStudio.Services; //SoundCloudService
using AppStudio.Data.SoundCloud; //Schema

namespace AppStudio.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MusicPlayer : Page
    {
        //standard app studio
        private NavigationHelper _navigationHelper;
        private DataTransferManager _dataTransferManager;

        public NavigationHelper NavigationHelper
        {
            get { return _navigationHelper; }
        }

        public TracksViewModel TracksModel { get; private set; }

        /// <summary>
        /// Storage of current flip index
        /// </summary>
        public int FlipSelectedItem { get; set; }

        /// <summary>
        /// Gets the information about background task is running or not by reading the setting saved by background task
        /// </summary>
        private bool isBackgroundTaskRunning = false;

        /// <summary>
        /// Gets the information about background task is running or not by reading the setting saved by background task
        /// </summary>
        private bool IsBackgroundTaskRunning
        {
            get
            {
                if (isBackgroundTaskRunning)
                    return true;

                object value = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.BackgroundTaskState);
                if (value == null)
                {
                    return false;
                }
                else
                {
                    isBackgroundTaskRunning = ((String)value).Equals(Constants.BackgroundTaskRunning);
                    return isBackgroundTaskRunning;
                }
            }
        }

        public MusicPlayer()
        {
            this.InitializeComponent();
            _navigationHelper = new NavigationHelper(this);

            TracksModel = new TracksViewModel();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            _dataTransferManager = DataTransferManager.GetForCurrentView();
            _dataTransferManager.DataRequested += OnDataRequested;

            _navigationHelper.OnNavigatedTo(e);

            if (TracksModel != null)
            {
                await TracksModel.LoadItemsAsync();
                if (e.NavigationMode != NavigationMode.Back)
                {
                    TracksModel.SelectItem(e.Parameter);                    
                }

                TracksModel.ViewType = ViewTypes.Detail;
            }
            FlipSelectedItem = Flip.SelectedIndex;
            DataContext = this;
            
            //Adding App suspension handlers here so that we can unsubscribe handlers 
            //that access to BackgroundMediaPlayer events
            App.Current.Suspending += ForegroundApp_Suspending;
            App.Current.Resuming += ForegroundApp_Resuming;
            ApplicationSettingsHelper.SaveSettingsValue(Constants.AppState, Constants.ForegroundAppActive);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedFrom(e);
            _dataTransferManager.DataRequested -= OnDataRequested;
        }

        private void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            
        }

        #region Foreground App Lifecycle Handlers
        /// <summary>
        /// Sends message to background informing app has resumed
        /// Subscribe to MediaPlayer events
        /// </summary>
        void ForegroundApp_Resuming(object sender, object e)
        {
            ApplicationSettingsHelper.SaveSettingsValue(Constants.AppState, Constants.ForegroundAppActive);

            // Verify if the task was running before
            if (IsBackgroundTaskRunning)
            {
                //if yes, reconnect to media play handlers
                AddMediaPlayerEventHandlers();

                //send message to background task that app is resumed, so it can start sending notifications
                ValueSet messageDictionary = new ValueSet();
                messageDictionary.Add(Constants.AppResumed, DateTime.Now.ToString());
                BackgroundMediaPlayer.SendMessageToBackground(messageDictionary);

                if (BackgroundMediaPlayer.Current.CurrentState == MediaPlayerState.Playing)
                {
                    AppBarPlayButton.Icon = new SymbolIcon(Symbol.Pause);     // Change to pause button
                }
                else
                {
                    AppBarPlayButton.Icon = new SymbolIcon(Symbol.Play);     // Change to play button
                }
                
            }
            else
            {
                AppBarPlayButton.Icon = new SymbolIcon(Symbol.Play);     // Change to play button                
            }

        }

        /// <summary>
        /// Send message to Background process that app is to be suspended
        /// Stop clock and slider when suspending
        /// Unsubscribe handlers for MediaPlayer events
        /// </summary>
        void ForegroundApp_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            ValueSet messageDictionary = new ValueSet();
            messageDictionary.Add(Constants.AppSuspended, DateTime.Now.ToString());
            BackgroundMediaPlayer.SendMessageToBackground(messageDictionary);
            RemoveMediaPlayerEventHandlers();
            ApplicationSettingsHelper.SaveSettingsValue(Constants.AppState, Constants.ForegroundAppSuspended);
            deferral.Complete();
        }
        #endregion

        #region UI Click event handlers
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Play button pressed from App");

            if (IsBackgroundTaskRunning)
            {
                if (MediaPlayerState.Playing == BackgroundMediaPlayer.Current.CurrentState)
                {
                    //Audio is playing..need to Pause
                    BackgroundMediaPlayer.Current.Pause();
                }
                else if (MediaPlayerState.Paused == BackgroundMediaPlayer.Current.CurrentState)
                {
                     //Audio is paused so start playing
                    BackgroundMediaPlayer.Current.Play();
                } 
                else if (MediaPlayerState.Closed == BackgroundMediaPlayer.Current.CurrentState) 
                {
                    //No audio playing
                    StartBackgroundAudio();
                }
            }
            else
            {
                StartBackgroundAudio();
            }
            
            
        }

        /// <summary>
        /// Restarts the media from the beginning
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsBackgroundTaskRunning)
            {
                RefreshBackgroundAudio();
            }
        }
        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if (Flip.SelectedIndex > 0)
            {
                Flip.SelectedIndex = FlipSelectedItem - 1;
            }
        }
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (Flip.Items.Count-1 > FlipSelectedItem) 
            {
                Flip.SelectedIndex = FlipSelectedItem + 1;
            }
            
        }
        #endregion

        private void Flip_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //start new media
            FlipSelectedItem = Flip.SelectedIndex;
            //enable or disable buttons
            if (Flip.Items.Count-1 < FlipSelectedItem) AppBarNextButton.IsEnabled = false; else AppBarNextButton.IsEnabled = true;
            if (Flip.SelectedIndex == 0) AppBarPrevButton.IsEnabled = false; else AppBarPrevButton.IsEnabled = true;

            //Play new media
            StartBackgroundAudio();

        }

        private async void StartBackgroundAudio()
        {
            SoundCloudTrackSchema trackSchema = await SoundCloudMusicService.GetTrackInfo(TracksModel.SelectedItem.TrackUrl.ToString());
            ValueSet message = new ValueSet();
            message.Add(Constants.StartPlayback, String.Format("{0}|{1}|{2}", 
                                                    SoundCloudMusicService.AuthUrl(trackSchema.stream_url),
                                                    trackSchema.user.username,
                                                    trackSchema.title));

            AddMediaPlayerEventHandlers();
            BackgroundMediaPlayer.SendMessageToBackground(message);
        }

        //private void StopBackgroundAudio()
        //{
        //    AppBarPlayButton.Icon = new SymbolIcon(Symbol.Play);
        //}

        private void RefreshBackgroundAudio()
        {
            StartBackgroundAudio();
        }

        #region Media Playback Helper methods
        /// <summary>
        /// Unsubscribes to MediaPlayer events. Should run only on suspend
        /// </summary>
        private void RemoveMediaPlayerEventHandlers()
        {
            BackgroundMediaPlayer.Current.CurrentStateChanged -= this.MediaPlayer_CurrentStateChanged;
            BackgroundMediaPlayer.MessageReceivedFromBackground -= this.BackgroundMediaPlayer_MessageReceivedFromBackground;
        }

        /// <summary>
        /// Subscribes to MediaPlayer events
        /// </summary>
        private void AddMediaPlayerEventHandlers()
        {
            BackgroundMediaPlayer.Current.CurrentStateChanged += this.MediaPlayer_CurrentStateChanged;
            BackgroundMediaPlayer.MessageReceivedFromBackground += this.BackgroundMediaPlayer_MessageReceivedFromBackground;
        }

        /// <summary>
        /// MediaPlayer state changed event handlers. 
        /// Note that we can subscribe to events even if Media Player is playing media in background
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        async void MediaPlayer_CurrentStateChanged(MediaPlayer sender, object args)
        {
            switch (sender.CurrentState)
            {
                case MediaPlayerState.Playing:
                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        AppBarPlayButton.Icon = new SymbolIcon(Symbol.Pause);                       
                    }
                    );
                    break;
                case MediaPlayerState.Paused:
                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        AppBarPlayButton.Icon = new SymbolIcon(Symbol.Play);     // Change to play button
                    }
                    );
                    break;
            }
        }

        /// <summary>
        /// This event fired when a message is recieved from Background Process
        /// </summary>
        async void BackgroundMediaPlayer_MessageReceivedFromBackground(object sender, MediaPlayerDataReceivedEventArgs e)
        {
            foreach (string key in e.Data.Keys)
            {
                switch (key)
                {
                    case Constants.Trackchanged:
                        //When foreground app is active change track based on background message
                        await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            //To Do: Possible swap out image
                        }
                        );
                        break;
                    case Constants.BackgroundTaskStarted:
                        //Wait for Background Task to be initialized before starting playback
                        Debug.WriteLine("Background Task started");
                        //To Do: Handle anything before playback
                        break;
                }
            }
        }
        #endregion
    }
}
