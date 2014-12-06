using System;
using System.Windows;
using System.Windows.Input;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using AppStudio.Services;
using AppStudio.Data;

namespace AppStudio.ViewModels
{
    
    public class TracksViewModel : ViewModelBase<TracksSchema>
    {
        private RelayCommandEx<TracksSchema> itemClickCommand;

        /// <summary>
        /// NOTE: We are changing the "navigate" page to go to the new page MusicPlayer to work with the Windows PHone 8.1 BackgroundAudio task. This will not
        /// work with Windows 8.1 Store apps. Please comment and put back prior line. Fix for Windows 8.1 is coming in the future.
        /// </summary>
        public RelayCommandEx<TracksSchema> ItemClickCommand
        {
            get
            {
                if (itemClickCommand == null)
                {
                    itemClickCommand = new RelayCommandEx<TracksSchema>(
                        (item) =>
                        {
                            //NavigationServices.NavigateToPage("TracksDetail", item);
                            NavigationServices.NavigateToPage("MusicPlayer", item);
                        });
                }

                return itemClickCommand;
            }
        }

        override protected DataSourceBase<TracksSchema> CreateDataSource()
        {
            return new TracksDataSource(); // CollectionDataSource
        }


        override public Visibility RefreshVisibility
        {
            get { return ViewType == ViewTypes.List ? Visibility.Visible : Visibility.Collapsed; }
        }

        public RelayCommandEx<Slider> IncreaseSlider
        {
            get
            {
                return new RelayCommandEx<Slider>(s => s.Value++);
            }
        }

        public RelayCommandEx<Slider> DecreaseSlider
        {
            get
            {
                return new RelayCommandEx<Slider>(s => s.Value--);
            }
        }

        override public void NavigateToSectionList()
        {
            NavigationServices.NavigateToPage("TracksList");
        }

        override protected void NavigateToSelectedItem()
        {
            //NavigationServices.NavigateToPage("TracksDetail");
            NavigationServices.NavigateToPage("SoundCloudPlayer");
        }
    }
}
