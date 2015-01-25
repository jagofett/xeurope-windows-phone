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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using XEurope.Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556
using XEurope.JsonClasses;

namespace XEurope.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DetailPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private CodeJson UserDTouchCode;
        private UserJson _userJson;

        public DetailPage()
        {
            this.InitializeComponent();

            DetailText.Text = "Loading...";

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        #region NavigationHelper
        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);

            UserDTouchCode = e.Parameter as CodeJson;
            if (UserDTouchCode == null)
            {
                return;
            }
            var uri = new Uri(ConnHelper.BaseUri + "users/" + UserDTouchCode.code);
            var resp = await ConnHelper.GetFromUri(uri);
            _userJson = (UserJson)JsonConvert.DeserializeObject(resp, typeof(UserJson));
            if (_userJson.error)
            {
                return;
            }
            //description
            DetailText.Text = _userJson.description;

            //logo
            var link = ConnHelper.AddHttpToUrl(_userJson.image_url);
            if (Uri.IsWellFormedUriString(link, UriKind.RelativeOrAbsolute))
            {
                var myUri = new Uri(link, UriKind.RelativeOrAbsolute);
                var bmi = new BitmapImage { CreateOptions = BitmapCreateOptions.IgnoreImageCache, UriSource = myUri };
                LogoImage.Source = bmi;
            } 
            //name
            ProjectNameText.Text = _userJson.name;

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        #region EventHandlers
        private void VoteClick(object sender, RoutedEventArgs e)
        {

        }

        private void ShareClick(object sender, RoutedEventArgs e)
        {

        }

        private async void ReadMoreClick(object sender, RoutedEventArgs e)
        {
            var link = ConnHelper.AddHttpToUrl(_userJson.link);
            var url = Uri.IsWellFormedUriString(link, UriKind.RelativeOrAbsolute) ? link : "http://xeurope.eitictlabs.hu/";

            var defUri = new Uri("http://xeurope.eitictlabs.hu/");
            var uri = new Uri(url);
            
            var options = new Windows.System.LauncherOptions {TreatAsUntrusted = true, FallbackUri = defUri};

            // Launch the URI with a warning prompt
            var success = await Windows.System.Launcher.LaunchUriAsync(uri, options);
            if (!success)
            {
                //todo can not open the link(?)
            }
        }
        #endregion
    }
}
