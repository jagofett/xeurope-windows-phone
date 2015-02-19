using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using XEurope.Common;
using XEurope.JsonClasses;

namespace XEurope.View
{
    public sealed partial class HistoryPage : Page
    {
        ObservableCollection<Scans> DB_ScanList = new ObservableCollection<Scans>();
        NavigationHelper navigationHelper;
        DatabaseHelperClass Db_Helper = new DatabaseHelperClass();

        public HistoryPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            this.Loaded += ReadScansList_Loaded;
        }

        private void ReadScansList_Loaded(object sender, RoutedEventArgs e)
        {
            ReadAllContactsList dbscans = new ReadAllContactsList();
            DB_ScanList = dbscans.GetAllContacts();//Get all DB contacts
            scansListBox.ItemsSource = DB_ScanList.OrderByDescending(i => i.Id).ToList();//Latest contact ID can Display first
        }

        private void scansListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (scansListBox.SelectedIndex != -1)
            {
                Scans listitem = scansListBox.SelectedItem as Scans;//Get slected listbox item contact ID
                (this.Parent as Frame).Navigate(typeof(DetailPage), new CodeJson { code = listitem.Code });
            }
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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            (this.Parent as Frame).Navigate(typeof(CameraPage));

        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            ConnHelper.Logout();
            
            (this.Parent as Frame).Navigate(typeof(MainPage));
        }

        private void NewPasswButton_Click(object sender, RoutedEventArgs e)
        {
            (this.Parent as Frame).Navigate(typeof(PasswordChangePage));

        }
    }
}
