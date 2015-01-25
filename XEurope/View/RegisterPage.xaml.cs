using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using XEurope.Common;
using XEurope.JsonClasses;

namespace XEurope
{
    public sealed partial class RegisterPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private string userName;
        private string userMail;
        private string userPass;
        private string userPass2;

        public RegisterPage()
        {
            this.InitializeComponent();

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
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        #region Register
        private async void RegisterUser(object sender, RoutedEventArgs e)
        {
            string errors = "";
            if (String.IsNullOrEmpty(userName))
                errors += "Please fill the Username!\n";
            if (String.IsNullOrEmpty(userMail))
                errors += "Please fill the Email address!\n";
            else if (!IsValidEmail(userMail))
                errors += "Please give valid email address!\n";

            if (String.IsNullOrEmpty(userPass))
                errors += "Please fill the Password!\n";
            if (userPass != userPass2)
                errors += "Passwords don't match!";

            if (errors != "")
            {
                var errorDialog = new MessageDialog(errors, "Error");
                await errorDialog.ShowAsync();
            }
            else
            {
                var myUri = new Uri(ConnHelper.BaseUri + "register");

                // Create the Json
                var registerData = new RegisterJson(userMail, userPass, userName);

                // Create the post data
                var postData = JsonConvert.SerializeObject(registerData);

                var resp = await ConnHelper.PostToUri(myUri, postData);

                try
                {


                    var responseData = (ErrorJson) JsonConvert.DeserializeObject(resp, typeof (ErrorJson));

                    var title = responseData.error
                        ? "Error"
                        : "Success";
                    var dialog = new MessageDialog(responseData.message, title);

                    dialog.ShowAsync();


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
                 */
            }
        }
        /*
        void GetRequestStreamCallback(IAsyncResult callbackResult)
        {
            HttpWebRequest myRequest = (HttpWebRequest)callbackResult.AsyncState;

            // End the stream request operation
            Stream postStream = myRequest.EndGetRequestStream(callbackResult);

            // Create the Json
            var registerData = new RegisterJson(userMail, userPass, userName);
            
            // Create the post data
            var postData = JsonConvert.SerializeObject(registerData);
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            // Add the post data to the web request
            postStream.Write(byteArray, 0, byteArray.Length);
            postStream.Flush();
            postStream.Dispose();

            // Start the web request
            myRequest.BeginGetResponse(new AsyncCallback(GetResponsetStreamCallback), myRequest);
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
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                    dialog.ShowAsync();
                });
                
            }
            catch (Exception e)
            {
                var dialog = new MessageDialog(e.Message, "Error");
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                   dialog.ShowAsync();
               });
            }
        }
         */
        #endregion

        #region EventHandlers
        private void UsernameChangedEvent(object sender, TextChangedEventArgs e)
        {
            userName = (string)(sender as TextBox).Text;
        }

        private void UserPassChangedEvent(object sender, RoutedEventArgs e)
        {
            userPass = (string)(sender as PasswordBox).Password;
        }

        private void UserPass2ChangedEvent(object sender, RoutedEventArgs e)
        {
            userPass2 = (string)(sender as PasswordBox).Password;
        }

        private void UserMailChangedEvent(object sender, TextChangedEventArgs e)
        {
            userMail = (string)(sender as TextBox).Text;
        }
        #endregion

        private void CancelRegister(object sender, RoutedEventArgs e)
        {
            this.navigationHelper.GoBack();
        }

        public bool IsValidEmail(string strIn)
        {
            /*
                Be warned that this will fail if:
                - There is a subdomain after the @ symbol.
                - You use a TLD with a length greater than 3, such as .info
            */

            if (String.IsNullOrEmpty(strIn))
                return false;

            string email = strIn;
            Regex regex = new Regex(@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                                    + "@"
                                    + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$");
            Match match = regex.Match(email);

            return match.Success;
        }
    }
}
