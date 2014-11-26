using System;

namespace AppStudio.Data
{
    /// <summary>
    /// Implementation of the RssSchema class.
    /// </summary>
    public class RssSchema : BindableSchemaBase, IEquatable<RssSchema>, IComparable<RssSchema>
    {
        private string _title;
        private string _summary;
        private string _content;
        private string _imageUrl;
        private string _extraImageUrl;
        private string _mediaUrl;
        private string _feedUrl;
        private string _author;
        private DateTime _publishDate;

        public string Id { get; set; }

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public string Summary
        {
            get { return _summary; }
            set { SetProperty(ref _summary, value); }
        }

        public string Content
        {
            get { return _content; }
            set { SetProperty(ref _content, value); }
        }

        public string ImageUrl
        {
            get { return _imageUrl; }
            set { SetProperty(ref _imageUrl, value); }
        }

        public string ExtraImageUrl
        {
            get { return _extraImageUrl; }
            set { SetProperty(ref _extraImageUrl, value); }
        }

        public string MediaUrl
        {
            get { return _mediaUrl; }
            set { SetProperty(ref _mediaUrl, value); }
        }

        public string FeedUrl
        {
            get { return _feedUrl; }
            set { SetProperty(ref _feedUrl, value); }
        }

        public string Author
        {
            get { return _author; }
            set { SetProperty(ref _author, value); }
        }

        public DateTime PublishDate
        {
            get { return _publishDate; }
            set { SetProperty(ref _publishDate, value); }
        }

        public override string DefaultTitle
        {
            get { return Title; }
        }

        public override string DefaultSummary
        {
            get { return Summary; }
        }

        public override string DefaultImageUrl
        {
            get { return ImageUrl; }
        }

        public override string DefaultContent
        {
            get { return Content; }
        }

        override public string GetValue(string fieldName)
        {
            if (!String.IsNullOrEmpty(fieldName))
            {
                switch (fieldName.ToLower())
                {
                    case "id":
                        return String.Format("{0}", Id);
                    case "title":
                        return String.Format("{0}", Title);
                    case "summary":
                        return String.Format("{0}", Summary);
                    case "content":
                        return String.Format("{0}", Content);
                    case "imageurl":
                        return String.Format("{0}", ImageUrl);
                    case "extraimageurl":
                        return String.Format("{0}", ExtraImageUrl);
                    case "mediaurl":
                        return String.Format("{0}", MediaUrl);
                    case "feedurl":
                        return String.Format("{0}", FeedUrl);
                    case "author":
                        return String.Format("{0}", Author);
                    case "publishdate":
                        return String.Format("{0}", PublishDate);
                    case "defaulttitle":
                        return String.Format("{0}", DefaultTitle);
                    case "defaultsummary":
                        return String.Format("{0}", DefaultSummary);
                    case "defaultimageurl":
                        return String.Format("{0}", DefaultImageUrl);
                    default:
                        break;
                }
            }
            return String.Empty;
        }

        public bool Equals(RssSchema other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (ReferenceEquals(null, other)) return false;

            return this.Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as RssSchema);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public int CompareTo(RssSchema other)
        {
            return -1 * this.PublishDate.CompareTo(other.PublishDate);
        }
    }
}
