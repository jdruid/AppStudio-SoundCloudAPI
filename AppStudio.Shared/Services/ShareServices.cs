using System;

using Windows.ApplicationModel.DataTransfer;

namespace AppStudio.Services
{
    public class ShareServices : IDisposable
    {
        private Action<DataRequest> _getShareContent = null;
        private DataTransferManager _transferManager = null;

        public void RegisterForShareContent(Action<DataRequest> getShareContent)
        {
            _getShareContent = getShareContent;

            _transferManager = DataTransferManager.GetForCurrentView();
            _transferManager.DataRequested += OnDataRequested;
        }

        private void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            _getShareContent(args.Request);
        }

        public void Dispose()
        {
            if (_transferManager != null)
            {
                _transferManager.DataRequested -= OnDataRequested;
                _transferManager = null;
            }
        }
    }
}
