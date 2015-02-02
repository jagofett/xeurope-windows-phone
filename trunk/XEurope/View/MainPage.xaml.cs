using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using XEurope.Common;
using XEurope.JsonClasses;
using XEurope.View;

namespace XEurope
{
    public sealed partial class MainPage : Page
    {
        NavigationHelper navigationHelper;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

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

        #region NavigationHelper registration

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
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        #region Login
        private async void Login(object sender, RoutedEventArgs e)
        {
            string errors = "";
            if (String.IsNullOrEmpty(UsernameField.Text))
                errors += "Please fill the Username!\n";
            if (String.IsNullOrEmpty(PasswordField.Password))
                errors += "Please fill the Password!\n";

            if (errors != "")
            {
                MessageDialog errorDialog = new MessageDialog(errors, "Error");
                await errorDialog.ShowAsync();
            }
            else
            {
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    // Create the Json
                    var loginData = new LoginJson(UsernameField.Text, PasswordField.Password);

                    // Create the post data
                    var postData = JsonConvert.SerializeObject(loginData);

                    Uri myUri = new Uri(ConnHelper.BaseUri + "login");
                    var response = await ConnHelper.PostToUri(myUri, postData);

                    try
                    {
                        var responseData = (LoginOkJson)JsonConvert.DeserializeObject(response, typeof(LoginOkJson));

                        var title = responseData.error
                            ? "Error"
                            : "Success";
                        var dialog = new MessageDialog(responseData.message, title);

                        if (!responseData.error)
                        {
                            // Check if it's the first launch
                            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey("FirstLaunch"))
                            {
                                ApplicationData.Current.LocalSettings.Values.Add(new KeyValuePair<string, object>("FirstLaunch", false));
                                (this.Parent as Frame).Navigate(typeof(View.TutorialPage));
                            }
                            else
                            {
                                (this.Parent as Frame).Navigate(typeof(CameraPage));
                            }
                        }
                        else
                        {
                            dialog.ShowAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        var dialog = new MessageDialog(ex.Message, "Error");
                        dialog.ShowAsync();
                    }
                });
            }
        }
        #endregion

        #region Link eventhandlers
        private void ShowRegistrationPage(object sender, RoutedEventArgs e)
        {
            (this.Parent as Frame).Navigate(typeof(RegisterPage)); 
        }

        private void ShowAboutPage(object sender, RoutedEventArgs e)
        {
            (this.Parent as Frame).Navigate(typeof(DetailPage), new CodeJson { code = "1:1:1:24" }); 
        }

        private void ResetPassword(object sender, RoutedEventArgs e)
        {
            (this.Parent as Frame).Navigate(typeof(CameraPage));
        }
        #endregion
    }
}
