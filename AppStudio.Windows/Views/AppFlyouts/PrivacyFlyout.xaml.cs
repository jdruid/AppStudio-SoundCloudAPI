using System;

using Windows.UI.Xaml.Controls;

using AppStudio.ViewModels;

namespace AppStudio.Views
{
    public sealed partial class PrivacyFlyout : SettingsFlyout
    {
        public PrivacyFlyout()
        {
            this.InitializeComponent();
            PrivacyViewModel = new PrivacyViewModel();
            this.DataContext = this;
        }

        public PrivacyViewModel PrivacyViewModel { get; private set; }
    }
}
