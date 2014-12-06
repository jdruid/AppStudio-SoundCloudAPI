﻿// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF 
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
// PARTICULAR PURPOSE. 
// 
// Copyright (c) Microsoft Corporation. All rights reserved 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackgroundAudioTask
{
    /// <summary>
    /// Collection of string constants used in the entire solution. This file is shared for all projects
    /// </summary>
    class Constants
    {
        public const string CurrentTrack = "trackname";
        public const string BackgroundTaskStarted = "BackgroundTaskStarted";
        public const string BackgroundTaskRunning = "BackgroundTaskRunning";
        public const string BackgroundTaskCancelled = "BackgroundTaskCancelled";
        public const string AppSuspended = "appsuspend";
        public const string AppResumed = "appresumed";
        public const string StartPlayback = "startplayback";
        public const string SkipNext = "skipnext";
        public const string Position = "position";
        public const string AppState = "appstate";
        public const string BackgroundTaskState = "backgroundtaskstate";
        public const string SkipPrevious = "skipprevious";
        public const string Trackchanged = "songchanged";
        public const string ForegroundAppActive = "Active";
        public const string ForegroundAppSuspended = "Suspended";
        public const string TrackStreamUrl = "trackstreamurl";
        public const string TrackTitle = "tracktitle";
        public const string TrackArtist = "trackartist";
        public const string TrackImage = "trackimage";
    }
}
