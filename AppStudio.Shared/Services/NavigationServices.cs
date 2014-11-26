using System;
using System.Net;
using AppStudio;
using Windows.System;

using AppStudio.Views;
using AppStudio.ViewModels;

namespace AppStudio.Services
{
    public class NavigationServices
    {
        static public void NavigateToPage(string pageName, object parameter = null)
        {
            try
            {
                string pageTypeName = String.Format("{0}.{1}", typeof(MainPage).Namespace, pageName);
                Type pageType = Type.GetType(pageTypeName);
                App.RootFrame.Navigate(pageType, parameter);
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("NavigationServices.NavigateToPage", ex);
            }
        }

        static public async void NavigateTo(Uri uri)
        {
            try
            {
                await Launcher.LaunchUriAsync(uri);
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("NavigationServices.NavigateTo", ex);
            }
        }
    }
}
