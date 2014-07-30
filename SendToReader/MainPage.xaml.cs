using System;
using SendToReader.API;
using SendToReader.API.Models;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SendToReader
{
    public sealed partial class MainPage : Page
    {
        public static bool IsShareTarget = false;

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
                IsShareTarget = true;

                Uri document = new Uri(e.Parameter as string);
                this.txtURL.Text = document.ToString();
            }
        }

        private void RenderStatusBar()
        {
            var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
            statusBar.BackgroundColor = Color.FromArgb(255, 34, 34, 34);
            statusBar.BackgroundOpacity = 1.0;

            statusBar.ProgressIndicator.Text = "Send To Reader";
            statusBar.ProgressIndicator.ProgressValue = 0.0;
            statusBar.ProgressIndicator.ShowAsync();
        }

        private async void LoadData()
        {
            var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();

            statusBar.ProgressIndicator.ProgressValue = null;
            statusBar.ProgressIndicator.ShowAsync();

            await ServiceClient.GetQueueStatus((result) =>
            {
                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    this.txtQueueStatus.Text = result;

                    statusBar.ProgressIndicator.ProgressValue = 0.0;
                    statusBar.ProgressIndicator.ShowAsync();
                });
            });
        }

        private async void AppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();

            statusBar.ProgressIndicator.ProgressValue = null;
            statusBar.ProgressIndicator.ShowAsync();

            ReaderDocument data = new ReaderDocument();
            data.URL = this.txtURL.Text;
            data.EmailAddress = this.txtEmailAddress.Text;

            await ServiceClient.InsertWebsite((result) =>
            {
                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    this.txtQueueStatus.Text = result;

                    statusBar.ProgressIndicator.ProgressValue = 0.0;
                    statusBar.ProgressIndicator.ShowAsync();

                    this.txtEmailAddress.Text = "";

                    if (IsShareTarget == true)
                    {
                        if (Frame.CanGoBack == true)
                            Frame.GoBack();
                    }
                    else
                    {
                        MessageDialog dialog = new MessageDialog("Your website has been submitted successully. Depending on the length of the queue, it should appear on your Kindle in a few minutes.", "Success");
                        dialog.ShowAsync();
                    }
                });
            }, data);
        }
    }
}
