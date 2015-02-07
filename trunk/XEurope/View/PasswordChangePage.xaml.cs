using Newtonsoft.Json;
using System;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using XEurope.Common;
using XEurope.JsonClasses;

namespace XEurope.View
{
    public sealed partial class PasswordChangePage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public PasswordChangePage()
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
        private async void ChangePassword(object sender, RoutedEventArgs e)
        {
            string errors = "";
            if (String.IsNullOrEmpty(OldPasswordField.Password))
                errors += "Please fill the Old password!\n";
            if (String.IsNullOrEmpty(NewPasswordField1.Password))
                errors += "Please fill the new password!\n";
            if (NewPasswordField1.Password != NewPasswordField2.Password)
                errors += "New passwords don't match!";

            if (errors != "")
            {
                var errorDialog = new MessageDialog(errors, "Error");
                await errorDialog.ShowAsync();
            }
            else
            {
                var myUri = new Uri(ConnHelper.BaseUri + "newPassword");

                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    // Create the Json
                    var passwordChangeData = new NewPasswordJson(ApplicationData.Current.LocalSettings.Values["CurrentUserMail"].ToString(),
                        OldPasswordField.Password, NewPasswordField1.Password);

                    // Create the post data
                    var postData = JsonConvert.SerializeObject(passwordChangeData);

                    var resp = await ConnHelper.PostToUri(myUri, postData);

                    try
                    {
                        var responseData = (ErrorJson)JsonConvert.DeserializeObject(resp, typeof(ErrorJson));
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
                });
            }
        }

        #endregion

        private void CancelChange(object sender, RoutedEventArgs e)
        {
            this.navigationHelper.GoBack();
        }
    }
}
