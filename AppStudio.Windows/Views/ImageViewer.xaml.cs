using System;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace AppStudio.Views
{
    public sealed partial class ImageViewer : Page
    {
        private NavigationHelper _navigationHelper;

        public ImageViewer()
        {
            this.InitializeComponent();
            _navigationHelper = new NavigationHelper(this);
            DataContext = this;
        }

        public NavigationHelper NavigationHelper
        {
            get { return _navigationHelper; }
        }

        #region NavigationHelper registration
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string imageSource = e.Parameter as String;
            SetImageSource(imageSource);

            _navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedFrom(e);
        }
        #endregion

        private void SetImageSource(string imageSource)
        {
            if (!String.IsNullOrEmpty(imageSource))
            {
                Uri uri = new Uri(imageSource, UriKind.RelativeOrAbsolute);
                if (!uri.IsAbsoluteUri)
                {
                    uri = new Uri(new Uri("ms-appx://"), imageSource);
                }
                image.Source = new BitmapImage(uri)
                {
                    CreateOptions = BitmapCreateOptions.None,
                    //DecodePixelHeight = 1024
                };
            }
        }
    }
}
