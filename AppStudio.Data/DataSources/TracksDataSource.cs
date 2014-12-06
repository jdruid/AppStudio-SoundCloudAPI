using System;
using System.Windows;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppStudio.Data
{
    public class TracksDataSource : DataSourceBase<TracksSchema>
    {
        private const string _appId = "a2a8169b-40e2-41c0-b95b-aaca3fda8123";
        private const string _dataSourceName = "c8c710b7-ea9f-4781-aa0d-25af84e19bb9";

        protected override string CacheKey
        {
            get { return "TracksDataSource"; }
        }

        public override bool HasStaticData
        {
            get { return false; }
        }

        public async override Task<IEnumerable<TracksSchema>> LoadDataAsync()
        {
            try
            {
                var serviceDataProvider = new ServiceDataProvider(_appId, _dataSourceName);
                
                return await serviceDataProvider.Load<TracksSchema>();
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("TracksDataSource.LoadData", ex.ToString());
                return new TracksSchema[0];
            }
        }
    }
}
