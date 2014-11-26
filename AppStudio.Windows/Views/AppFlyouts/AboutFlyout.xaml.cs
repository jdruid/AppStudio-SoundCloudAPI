using System;

using Windows.UI.Xaml.Controls;

using AppStudio.ViewModels;

namespace AppStudio.Views
{
    public sealed partial class AboutFlyout : SettingsFlyout
    {
        public AboutFlyout()
        {
            this.InitializeComponent();
            AboutThisAppModel = new AboutThisAppViewModel();
            this.DataContext = this;
        }

        public AboutThisAppViewModel AboutThisAppModel { get; private set; }
    }
}
