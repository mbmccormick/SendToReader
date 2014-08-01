using System;
using System.Text.RegularExpressions;
using SendToReader.API;
using SendToReader.API.Models;
using Windows.Storage;
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
            statusBar.ForegroundColor = Color.FromArgb(255, 0, 0, 0);
            statusBar.BackgroundOpacity = 0.0;

            statusBar.ProgressIndicator.ProgressValue = 0.0;
            statusBar.ProgressIndicator.ShowAsync();
        }

        private async void LoadData()
        {
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("EmailAddress"))
            {
                this.txtEmailAddress.Text = ApplicationData.Current.RoamingSettings.Values["EmailAddress"] as string;
            }

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
            if (ValidateData() == false)
                return;

            ApplicationData.Current.RoamingSettings.Values["EmailAddress"] = this.txtEmailAddress.Text;

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

                    this.txtURL.Text = "";

                    if (IsShareTarget == true)
                    {
                        App.Current.Exit();
                    }
                    else
                    {
                        MessageDialog dialog = new MessageDialog("Your website has been submitted successully. Depending on the length of the queue, it should appear on your Kindle in a few minutes.", "Success");
                        dialog.ShowAsync();
                    }
                });
            }, data);
        }

        private bool ValidateData()
        {
            if (this.txtURL.Text.Length <= 0)
            {
                MessageDialog dialog = new MessageDialog("The Website URL field is required.", "Error");
                dialog.ShowAsync();

                return false;
            }

            if (Regex.IsMatch(this.txtURL.Text, @"^(http|https)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(/\S*)?$") == false)
            {
                MessageDialog dialog = new MessageDialog("Website URL is not valid.", "Error");
                dialog.ShowAsync();
                
                return false;
            }

            if (Regex.IsMatch(this.txtEmailAddress.Text, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$") == false)
            {
                MessageDialog dialog = new MessageDialog("The Email Address field is required.", "Error");
                dialog.ShowAsync();
                
                return false;
            }

            if (this.txtEmailAddress.Text.Length <= 0)
            {
                MessageDialog dialog = new MessageDialog("Email Address is not valid.", "Error");
                dialog.ShowAsync();
                
                return false;
            }

            return true;
        }
    }
}
