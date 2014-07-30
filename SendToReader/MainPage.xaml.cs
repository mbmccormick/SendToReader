using System;
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
            // TODO: Prepare page for display here.
            var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
            statusBar.BackgroundOpacity = 1.0;
            statusBar.BackgroundColor = Color.FromArgb(255, 34, 34, 34);

            statusBar.ProgressIndicator.Text = "Send To Reader";
            statusBar.ProgressIndicator.ProgressValue = 0;

            statusBar.ProgressIndicator.ShowAsync();

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.

            if (e.Parameter != null &&
                String.IsNullOrEmpty(e.Parameter as string) == false)
            {
                Uri document = new Uri(e.Parameter as string);

                this.txtURL.Text = document.ToString();
            }
        }
    }
}
