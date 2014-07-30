using System;
using SendToReader.API;
using SendToReader.API.Models;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SendToReader
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            RenderStatusBar();

            LoadData();

            if (e.Parameter != null &&
                String.IsNullOrEmpty(e.Parameter as string) == false)
            {
                Uri document = new Uri(e.Parameter as string);

                this.txtURL.Text = document.ToString();
            }
        }

        private void RenderStatusBar()
        {
            var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
            statusBar.BackgroundOpacity = 1.0;
            statusBar.BackgroundColor = Color.FromArgb(255, 34, 34, 34);

            statusBar.ProgressIndicator.Text = "Send To Reader";
            statusBar.ProgressIndicator.ProgressValue = 0;

            statusBar.ProgressIndicator.ShowAsync();
        }

        private async void LoadData()
        {
            await ServiceClient.GetQueueStatus((result) =>
            {
                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    this.txtQueueStatus.Text = result;
                });
            });
        }

        private async void AppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ReaderDocument data = new ReaderDocument();
            data.URL = this.txtURL.Text;
            data.EmailAddress = this.txtEmailAddress.Text;

            await ServiceClient.InsertWebsite((result) =>
            {
                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    this.txtQueueStatus.Text = result;
                });
            }, data);
        }
    }
}
