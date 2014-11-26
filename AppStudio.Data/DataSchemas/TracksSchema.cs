using System;
using System.Collections;
using Newtonsoft.Json;

namespace AppStudio.Data
{
    /// <summary>
    /// Implementation of the TracksSchema class.
    /// </summary>
    public class TracksSchema : BindableSchemaBase, IEquatable<TracksSchema>, ISyncItem<TracksSchema>
    {
        private string _trackTitle;
        private string _trackUrl;
        private string _image;
        [JsonProperty("_id")]
        public string Id { get; set; }

 
        public string TrackTitle
        {
            get { return _trackTitle; }
            set { SetProperty(ref _trackTitle, value); }
        }
 
        public string TrackUrl
        {
            get { return _trackUrl; }
            set { SetProperty(ref _trackUrl, value); }
        }
 
        public string Image
        {
            get { return _image; }
            set { SetProperty(ref _image, value); }
        }

        public override string DefaultTitle
        {
            get { return TrackTitle; }
        }

        public override string DefaultSummary
        {
            get { return null; }
        }

        public override string DefaultImageUrl
        {
            get { return Image; }
        }

        public override string DefaultContent
        {
            get { return null; }
        }

        override public string GetValue(string fieldName)
        {
            if (!String.IsNullOrEmpty(fieldName))
            {
                switch (fieldName.ToLowerInvariant())
                {
                    case "tracktitle":
                        return String.Format("{0}", TrackTitle); 
                    case "trackurl":
                        return String.Format("{0}", TrackUrl); 
                    case "image":
                        return String.Format("{0}", Image); 
                    case "defaulttitle":
                        return DefaultTitle;
                    case "defaultsummary":
                        return DefaultSummary;
                    case "defaultimageurl":
                        return DefaultImageUrl;
                    default:
                        break;
                }
            }
            return String.Empty;
        }

        public bool Equals(TracksSchema other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (ReferenceEquals(null, other)) return false;
            return this.Id == other.Id;
        }

        public bool NeedSync(TracksSchema other)
        {

            return this.Id == other.Id && (this.TrackTitle != other.TrackTitle || this.TrackUrl != other.TrackUrl || this.Image != other.Image);
        }

        public void Sync(TracksSchema other)
        {
            this.TrackTitle = other.TrackTitle;
            this.TrackUrl = other.TrackUrl;
            this.Image = other.Image;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TracksSchema);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
