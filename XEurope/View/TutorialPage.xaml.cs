using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace XEurope.View
{
    public sealed partial class TutorialPage : Page
    {
        public TutorialPage()
        {
            this.InitializeComponent();

            TutorialText.Text = "1. Look for an X-Europe project poster in your local university\n" +
            "2. Press on start button\n" +
            "3. Point your device over a logo\n" +
            "4. Press Analyze to start scanning\n" +
            "5. Read about the projects\n" +
            "6. Vote on what you like\n\n" +
            "Note: The application needs internet access for scanning and voting!";
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            (this.Parent as Frame).Navigate(typeof(CameraPage));
        }
    }
}
