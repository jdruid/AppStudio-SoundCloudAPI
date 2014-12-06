// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF 
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
// PARTICULAR PURPOSE. 
// 
// Copyright (c) Microsoft Corporation. All rights reserved 
using System;
using System.Diagnostics;
using System.Threading; //AutoResetEvent
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Playback;

namespace BackgroundAudioTask
{
    /// <summary>
    /// Enum to identify foreground app state
    /// </summary>
    enum ForegroundAppStatus
    {
        Active,
        Suspended,
        Unknown
    }

    /// <summary>
    /// Impletements IBackgroundTask to provide an entry point for app code to be run in background. 
    /// Also takes care of handling UVC and communication channel with foreground
    /// </summary>
    public sealed class BackgroundAudioTask: IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;
        private SystemMediaTransportControls _systemMediaTransportControl;
        private ForegroundAppStatus foregroundAppState = ForegroundAppStatus.Unknown;
        private AutoResetEvent BackgroundTaskStarted = new AutoResetEvent(false);
        private bool backgroundtaskrunning = false;
              
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            Debug.WriteLine("Background Audio Task " + taskInstance.Task.Name + " starting...");
            _systemMediaTransportControl = SystemMediaTransportControls.GetForCurrentView();
            _systemMediaTransportControl.IsEnabled = true;

            //Add handlers for MediaPlayer
            BackgroundMediaPlayer.MessageReceivedFromForeground += MessageReceivedFromForeground;
            //Initialize message channel 
            BackgroundMediaPlayer.Current.CurrentStateChanged += BackgroundMediaPlayerCurrentStateChanged;
            // Associate a cancellation and completed handlers with the background task.
            taskInstance.Canceled += OnCanceled;
            taskInstance.Task.Completed += Taskcompleted;

            var value = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.AppState);
            if (value == null)
                foregroundAppState = ForegroundAppStatus.Unknown;
            else
                foregroundAppState = (ForegroundAppStatus)Enum.Parse(typeof(ForegroundAppStatus), value.ToString());

            //Send information to foreground that background task has been started if app is active
            if (foregroundAppState != ForegroundAppStatus.Suspended)
            {
                ValueSet message = new ValueSet();
                message.Add(Constants.BackgroundTaskStarted, "");
                BackgroundMediaPlayer.SendMessageToForeground(message);
            }
            BackgroundTaskStarted.Set();
            backgroundtaskrunning = true;

            ApplicationSettingsHelper.SaveSettingsValue(Constants.BackgroundTaskState, Constants.BackgroundTaskRunning);

            _deferral = taskInstance.GetDeferral();
        }

        /// <summary>
        /// Fires when a message is recieved from the foreground app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MessageReceivedFromForeground(object sender, MediaPlayerDataReceivedEventArgs e)
        {
            ValueSet valueSet = e.Data;
            foreach (string key in valueSet.Keys)
            {
                switch (key.ToLower())
                {
                    case Constants.StartPlayback:
                        Debug.WriteLine("Starting Playback");
                        Play(valueSet[key].ToString());
                        break;
                }
            }
        }

        private void Play(string toPlay)
        {
            string[] trackInfo = toPlay.Split('|');
            
            MediaPlayer mediaPlayer = BackgroundMediaPlayer.Current;
            mediaPlayer.AutoPlay = true;
            mediaPlayer.SetUriSource(new Uri(trackInfo[0]));

            //Update the universal volume control
            _systemMediaTransportControl.ButtonPressed += MediaTransportControlButtonPressed;
            _systemMediaTransportControl.IsPauseEnabled = true;
            _systemMediaTransportControl.IsPlayEnabled = true;
            _systemMediaTransportControl.DisplayUpdater.Type = MediaPlaybackType.Music;
            _systemMediaTransportControl.DisplayUpdater.MusicProperties.Artist = trackInfo[1];
            _systemMediaTransportControl.DisplayUpdater.MusicProperties.Title = trackInfo[2];            
            _systemMediaTransportControl.DisplayUpdater.Update();
        }
        
        /// <summary>
        /// The MediaPlayer's state changes, update the Universal Volume Control to reflect the correct state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void BackgroundMediaPlayerCurrentStateChanged(MediaPlayer sender, object args)
        {
            if (sender.CurrentState == MediaPlayerState.Playing)
            {
                _systemMediaTransportControl.PlaybackStatus = MediaPlaybackStatus.Playing;
            }
            else if (sender.CurrentState == MediaPlayerState.Paused)
            {
                _systemMediaTransportControl.PlaybackStatus = MediaPlaybackStatus.Paused;
            }
        }

        /// <summary>
        /// Handle the buttons on the Universal Volume Control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void MediaTransportControlButtonPressed(SystemMediaTransportControls sender,
            SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Play:
                    BackgroundMediaPlayer.Current.Play();
                    break;
                case SystemMediaTransportControlsButton.Pause:
                    BackgroundMediaPlayer.Current.Pause();
                    break;
            }
        }


        private void Taskcompleted(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            BackgroundMediaPlayer.Shutdown();
            _deferral.Complete();
        }

        /// <summary>
        /// Handles background task cancellation. Task cancellation happens due to :
        /// 1. Another Media app comes into foreground and starts playing music 
        /// 2. Resource pressure. Your task is consuming more CPU and memory than allowed.
        /// In either case, save state so that if foreground app resumes it can know where to start.
        /// </summary>
        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {

            try
            {
                //save state - need to save state for track url, name, artist AND position
                ApplicationSettingsHelper.SaveSettingsValue(Constants.BackgroundTaskState, Constants.BackgroundTaskCancelled);
                ApplicationSettingsHelper.SaveSettingsValue(Constants.AppState, Enum.GetName(typeof(ForegroundAppStatus), foregroundAppState));
                backgroundtaskrunning = false;
                BackgroundMediaPlayer.Shutdown(); // shutdown media pipeline
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            _deferral.Complete(); // signals task completion. 
            Debug.WriteLine("MyBackgroundAudioTask Cancel complete...");

        }
    }
}
