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
        public RelayCommandEx<TracksSchema> ItemClickCommand
        {
            get
            {
                if (itemClickCommand == null)
                {
                    itemClickCommand = new RelayCommandEx<TracksSchema>(
                        (item) =>
                        {
                            NavigationServices.NavigateToPage("TracksDetail", item);
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
            NavigationServices.NavigateToPage("TracksDetail");
        }
    }
}
