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
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.Storage;
using Windows.UI.Text;

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

        private DataTransferManager _dataTransferManager;
        DatabaseHelperClass Db_Helper = new DatabaseHelperClass();

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

            // Register the current page as a share source.
            _dataTransferManager = DataTransferManager.GetForCurrentView();
            _dataTransferManager.DataRequested += OnDataRequested;

            UserDTouchCode = e.Parameter as CodeJson;
            if (UserDTouchCode == null)
                return;

            var uri = new Uri(ConnHelper.BaseUri + "users/" + UserDTouchCode.code);
            var resp = await ConnHelper.GetFromUri(uri);
            _userJson = (UserJson)JsonConvert.DeserializeObject(resp, typeof(UserJson));
            if (_userJson.error)
                return;

            // Description
            DetailText.Text = _userJson.description ?? "No description!";

            // Logo
            var link = ConnHelper.AddHttpToUrl(_userJson.image_url);
            if (Uri.IsWellFormedUriString(link, UriKind.RelativeOrAbsolute))
            {
                var myUri = new Uri(link, UriKind.RelativeOrAbsolute);
                var bmi = new BitmapImage { CreateOptions = BitmapCreateOptions.IgnoreImageCache, UriSource = myUri };
                LogoImage.Source = bmi;
            } 
            else
            {
                LogoImage.Source = new BitmapImage(new Uri(@"ms-appx:///../Assets/no_image.png"));
            }

            // Name
            ProjectNameText.Text = _userJson.name ?? "Anonymus Project";

            // Save to database
            try
            {
                var scan = Db_Helper.ReadScan(_userJson.code);
                if (scan == null)
                {
                    Db_Helper.Insert(new Scans(_userJson.name, _userJson.image_url, _userJson.code, "Not voted"));
                }           
                else
                {
                    if (scan.Voted == "Voted!")
                        VoteButton.Content = "VOTED!";
                    else
                        VoteButton.Content = "VOTE";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            // Comments
            uri = new Uri(ConnHelper.BaseUri + "messages/" + UserDTouchCode.code);
            resp = await ConnHelper.GetFromUri(uri);
            var _commentsJson = (CommentsJson)JsonConvert.DeserializeObject(resp, typeof(CommentsJson));
            if (_commentsJson.error)
                return;

            foreach (var m in _commentsJson.messages)
            {
                var titleBlock = new TextBlock()
                {
                    Text = m.voterName + ":",
                    FontSize = 20,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Left,
                    FontStyle = Windows.UI.Text.FontStyle.Italic,
                    Margin = new Thickness(19,10,19,0),
                    Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 63, 63, 62))
                };
                PageStackPanel.Children.Add(titleBlock);

                var commentBlock = new TextBlock()
                {
                    Text = m.message,
                    FontSize = 16,
                    TextAlignment = TextAlignment.Justify,
                    TextWrapping = Windows.UI.Xaml.TextWrapping.Wrap,
                    Height = double.NaN,
                    Margin = new Thickness(19,0,19,0),
                    Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255,63,63,62))
                };
                PageStackPanel.Children.Add(commentBlock);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Unregister the current page as a share source.
            _dataTransferManager.DataRequested -= OnDataRequested;

            this.navigationHelper.OnNavigatedFrom(e);
        }
        #endregion

        protected void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            e.Request.Data.Properties.Title = "X-Europe project";
            e.Request.Data.Properties.Description = "Check out the X-Europe Project by EIT"; // Optional 

            var link = ConnHelper.AddHttpToUrl(_userJson.link);
            var url = Uri.IsWellFormedUriString(link, UriKind.RelativeOrAbsolute) ? link : "http://xeurope.eitictlabs.hu/";

            var defUri = new Uri("http://xeurope.eitictlabs.hu/");
            var uri = new Uri(url);

            e.Request.Data.SetUri(uri);
        }

        #region EventHandlers
        private void VoteClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Db_Helper.UpdateScan(new Scans(_userJson.name, _userJson.image_url, _userJson.code, "Voted!"));
            }
            catch { }

            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                // Create the Json
                var voteData = string.IsNullOrEmpty(CommentField.Text)
                    ? new VoteJson(UserDTouchCode.code)
                    : new VoteWithCommentJson(UserDTouchCode.code, CommentField.Text);

                // Create the post data
                var postData = JsonConvert.SerializeObject(voteData);

                Uri myUri = new Uri(ConnHelper.BaseUri + "vote");
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
                        await dialog.ShowAsync();
                        navigationHelper.GoBack();
                    }
                    else
                    {
                        VoteButton.Content = "VOTED!";
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

        private void ShareClick(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
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
