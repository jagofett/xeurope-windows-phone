using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using XEurope.Common;
using XEurope.JsonClasses;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641
using XEurope.View;

namespace XEurope
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
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
                Uri myUri = new Uri(ConnHelper.BaseUri + "login");

                // Create the Json
                var loginData = new LoginJson(UsernameField.Text, PasswordField.Password);

                // Create the post data
                var postData = JsonConvert.SerializeObject(loginData);
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
                        (this.Parent as Frame).Navigate(typeof (View.CameraPage));
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
                /*

                HttpWebRequest myRequest = (HttpWebRequest)HttpWebRequest.Create(myUri);
                
                myRequest.ContentType = "application/json";
                // Needed: Vote, voteCountByCode, Users
                //myRequest.Headers["Authorization"] = "a065bde13113778966eacdeff21a5ead";
                myRequest.Method = "POST";
                myRequest.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), myRequest);
                  * */
            }
        }
        /*
        void GetRequestStreamCallback(IAsyncResult callbackResult)
        {
            HttpWebRequest myRequest = (HttpWebRequest)callbackResult.AsyncState;

            // End the stream request operation
            Stream postStream = myRequest.EndGetRequestStream(callbackResult);

            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Create the Json
                var loginData = new LoginJson(UsernameField.Text, PasswordField.Password);

                // Create the post data
                var postData = JsonConvert.SerializeObject(loginData);
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);

                // Add the post data to the web request
                postStream.Write(byteArray, 0, byteArray.Length);
                postStream.Flush();
                postStream.Dispose();

                // Start the web request
                myRequest.BeginGetResponse(new AsyncCallback(GetResponsetStreamCallback), myRequest);
            });
        }

        void GetResponsetStreamCallback(IAsyncResult callbackResult)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)callbackResult.AsyncState;
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(callbackResult);

                var stream = new StreamReader(response.GetResponseStream());
                var responseString = stream.ReadToEnd();

                JsonClasses.ErrorJson responseData = (JsonClasses.ErrorJson)JsonConvert.DeserializeObject(responseString, typeof(JsonClasses.ErrorJson));

                var title = responseData.error
                    ? "Error"
                    : "Success";
                var dialog = new MessageDialog(responseData.message, title);
                
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
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
                            (this.Parent as Frame).Navigate(typeof(MainPage));
                        }
                    }
                    else
                    {
                        dialog.ShowAsync();
                    }     
                });
            }
            catch (Exception e)
            {
                var dialog = new MessageDialog(e.Message, "Error");
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    dialog.ShowAsync();
                });
            }
        }
         * */
        #endregion

        #region Link eventhandlers
        private void ShowRegistrationPage(object sender, RoutedEventArgs e)
        {
            (this.Parent as Frame).Navigate(typeof(RegisterPage)); 
        }

        private void ShowAboutPage(object sender, RoutedEventArgs e)
        {
            (this.Parent as Frame).Navigate(typeof(DetailPage), new CodeJson { code = "1:2:3" });
        }

        private void ResetPassword(object sender, RoutedEventArgs e)
        {
            (this.Parent as Frame).Navigate(typeof(ResetPasswordPage));
        }
        #endregion
    }
}
