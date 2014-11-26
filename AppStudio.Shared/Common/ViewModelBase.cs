using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Net.NetworkInformation;

using Windows.UI.Xaml;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;

using AppStudio.Data;
using AppStudio.Services;

namespace AppStudio.ViewModels
{
    public enum ViewTypes
    {
        List,
        Detail
    }

    abstract public class ViewModelBase : BindableBase
    {
        private Visibility _progressBarVisibility = Visibility.Visible;

        public Visibility ProgressBarVisibility
        {
            get { return _progressBarVisibility; }
            set { SetProperty(ref _progressBarVisibility, value); }
        }

        public Visibility AppBarVisibility
        {
            get
            {
                if (TextToSpeechVisibility == Visibility.Visible ||
                    PinToStartVisibility == Visibility.Visible ||
                    GoToSourceVisibility == Visibility.Visible ||
                    ShareItemVisibility == Visibility.Visible ||
                    RefreshVisibility == Visibility.Visible)
                {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        virtual public ViewTypes ViewType { get; set; }

        virtual public Visibility TextToSpeechVisibility
        {
            get { return Visibility.Collapsed; }
        }

        virtual public Visibility PinToStartVisibility
        {
            get { return Visibility.Collapsed; }
        }

        virtual public Visibility GoToSourceVisibility
        {
            get { return Visibility.Collapsed; }
        }

        virtual public Visibility ShareItemVisibility
        {
            get { return Visibility.Collapsed; }
        }

        virtual public Visibility RefreshVisibility
        {
            get { return Visibility.Collapsed; }
        }

        virtual public void NavigateToSectionList() { }

        abstract public Task LoadItemsAsync(bool forceRefresh = false);

        virtual protected void TextToSpeech() { }
        virtual protected void PinToStart() { }
        virtual protected void GoToSource() { }
        virtual protected void ShareItem() { }

        abstract public void GetShareContent(DataRequest dataRequest);
    }

    abstract public class ViewModelBase<T> : ViewModelBase where T : BindableSchemaBase
    {
        private const int PREVIEWITEMS_COUNT = 6;

        protected ObservableCollection<T> _items = new ObservableCollection<T>();
        protected ObservableCollection<T> _previewItems = new ObservableCollection<T>();

        protected T _selectedItem = null;

        protected DataSourceBase<T> _dataSource;

        protected virtual void NavigateToSelectedItem() { }

        protected abstract DataSourceBase<T> CreateDataSource();

        public ObservableCollection<T> Items
        {
            get { return _items; }
        }

        public ObservableCollection<T> PreviewItems
        {
            get
            {
                if (_items != null)
                {
                    _previewItems.AddRangeUnique(_items.Take(PREVIEWITEMS_COUNT));
                }
                return _previewItems;
            }
        }

        virtual public bool HasMoreItems
        {
            get { return _items != null ? _items.Count > PREVIEWITEMS_COUNT : false; }
        }


        public T SelectedItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem, value); }
        }

        public bool IsItemSelected
        {
            get { return SelectedItem != null; }
        }

        public DataSourceBase<T> DataSource
        {
            get { return _dataSource ?? (_dataSource = CreateDataSource()); }
        }

        public void SelectItem(object item)
        {
            this.SelectedItem = item as T;
        }

        public override async Task LoadItemsAsync(bool forceRefresh = false)
        {
            ProgressBarVisibility = Visibility.Visible;

            var timeStamp = await DataSource.LoadDataAsync(Items, forceRefresh);

            OnPropertyChanged("PreviewItems");
            OnPropertyChanged("HasMoreItems");

            ProgressBarVisibility = Visibility.Collapsed;
        }

        protected override void ShareItem()
        {
            DataTransferManager.ShowShareUI();
        }

        /// <summary>
        /// Called from DataTransferManager when user wants to share App content.
        /// </summary>
        override public void GetShareContent(DataRequest dataRequest)
        {
            var currentItem = GetCurrentItem();
            if (currentItem != null)
            {
                // Share SelectedItem Title
                dataRequest.Data.Properties.Title = currentItem.DefaultTitle ?? App.APP_NAME;

                // Share SelectedItem Summary
                if (!String.IsNullOrEmpty(currentItem.DefaultSummary))
                {
                    dataRequest.Data.SetText(Utility.DecodeHtml(currentItem.DefaultSummary));
                }

                // Share SelectedItem Content
                if (!String.IsNullOrEmpty(currentItem.DefaultContent))
                {
                    dataRequest.Data.SetHtmlFormat(HtmlFormatHelper.CreateHtmlFormat(currentItem.DefaultContent));
                }

                // Share SelectedItem DefaultImageUrl
                string imageUrl = currentItem.DefaultImageUrl;
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    if (imageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    {
                        dataRequest.Data.SetWebLink(new Uri(imageUrl));
                    }
                    else
                    {
                        imageUrl = string.Format("ms-appx://{0}", imageUrl);
                    }
                    dataRequest.Data.SetBitmap(RandomAccessStreamReference.CreateFromUri(new Uri(imageUrl)));
                }
            }
        }

        //
        // ICommands
        //
        public ICommand TextToSpeechCommand
        {
            get { return new DelegateCommand(TextToSpeech); }
        }

        public ICommand PinToStartCommand
        {
            get { return new DelegateCommand(PinToStart); }
        }

        public ICommand GoToSourceCommand
        {
            get { return new DelegateCommand(GoToSource); }
        }

        public ICommand ShareItemCommand
        {
            get { return new DelegateCommand(ShareItem); }
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new DelegateCommand(async () =>
                {
                    await LoadItemsAsync(true);
                });
            }
        }

        //
        // Command implementation helpers
        //
        protected async Task SpeakText(params string[] propertyNames)
        {
            var currentItem = GetCurrentItem();
            if (currentItem != null)
            {
                await SpeechServices.StartTextToSpeech(currentItem.GetValues(propertyNames));
            }
        }

        protected void PinToStart(string path, string titleToShare, string messageToShare, string imageToShare)
        {
            var currentItem = GetCurrentItem();
            if (currentItem != null)
            {
                if (String.IsNullOrEmpty(path))
                {
                    path = "MainPage";
                }
                // TODO: Not implemented
            }
        }

        protected void GoToSource(string linkProperty)
        {
            var currentItem = GetCurrentItem();
            if (currentItem != null)
            {
                string url = GetBindingValue(linkProperty);
                if (!String.IsNullOrEmpty(url) && Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    NavigationServices.NavigateTo(new Uri(url, UriKind.Absolute));
                }
            }
        }

        private string GetBindingValue(string binding)
        {
            binding = binding ?? String.Empty;
            if (binding.StartsWith("{") && binding.EndsWith("}"))
            {
                var currentItem = GetCurrentItem();
                if (currentItem != null)
                {
                    string propertyName = binding.Substring(1, binding.Length - 2);
                    return currentItem.GetValue(propertyName);
                }
            }
            return binding;
        }

        protected T GetCurrentItem()
        {
            if (SelectedItem != null)
            {
                return SelectedItem;
            }
            if (Items != null && Items.Count > 0)
            {
                return Items[0];
            }
            return null;
        }

        protected Uri TryCreateUri(string uriString)
        {
            try
            {
                return new Uri(uriString, UriKind.Absolute);
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("ViewModelBase.TryCreateUri", ex);
            }
            return null;
        }
    }
}
