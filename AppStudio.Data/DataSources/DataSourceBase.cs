using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace AppStudio.Data
{
    public abstract class DataSourceBase<T> where T : BindableSchemaBase
    {
        private const int ContentExpirationHours = 2;

        protected abstract string CacheKey { get; }

        public abstract Task<IEnumerable<T>> LoadDataAsync();

        public abstract bool HasStaticData { get; }

        public async Task<DateTime> LoadDataAsync(ObservableCollection<T> viewItems, bool forceRefresh)
        {
            DateTime timeStamp = DateTime.Now;

            if (HasStaticData)
            {
                viewItems.AddRangeUnique(await LoadDataAsync());
            }
            else
            {
                var dataInCache = await AppCache.GetItemsAsync<T>(CacheKey);
                if (dataInCache != null)
                {
                    timeStamp = dataInCache.TimeStamp;

                    viewItems.AddRangeUnique(dataInCache.Items);
                }

                if (NetworkInterface.GetIsNetworkAvailable() && DataNeedToBeUpdated(forceRefresh, dataInCache))
                {
                    var freshData = new DataSourceContent<T>()
                    {
                        TimeStamp = DateTime.Now,
                        Items = await LoadDataAsync()
                    };

                    timeStamp = freshData.TimeStamp;

                    viewItems.AddRangeUnique(freshData.Items);

                    await AppCache.AddItemsAsync(CacheKey, freshData);
                }
            }
            return timeStamp;
        }

        private bool DataNeedToBeUpdated(bool forceRefresh, DataSourceContent<T> dataInCache)
        {
            return dataInCache == null || forceRefresh || IsContentExpirated(dataInCache.TimeStamp);
        }

        private bool IsContentExpirated(DateTime timeStamp)
        {
            return (DateTime.Now - timeStamp) > new TimeSpan(ContentExpirationHours, 0, 0);
        }
    }
}
