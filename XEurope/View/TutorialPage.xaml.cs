using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace XEurope.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TutorialPage : Page
    {
        public TutorialPage()
        {
            this.InitializeComponent();

            TutorialText.Text = "1. Look for an X-Europe roject poster in your local university\n" +
            "2. Press on start button\n" +
            "3. Point your device over a logo\n" +
            "4. Press Analyze to start scanning\n" +
            "5. Read about the projects\n" +
            "6. Vote on what you like\n\n" +
            "Note: The application needs internet access for scanning";
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
