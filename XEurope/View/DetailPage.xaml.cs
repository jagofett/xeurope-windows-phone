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

        public DetailPage()
        {
            this.InitializeComponent();

            DetailText.Text = " Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean porttitor sed libero nec feugiat. Pellentesque viverra quam leo, eget laoreet dui dictum accumsan. Vestibulum quis nunc arcu. Duis id rutrum turpis. Donec vulputate egestas massa, sit amet luctus augue posuere ac. Mauris eros ex, tincidunt eget dapibus ac, accumsan sit amet magna. Donec eleifend nec ex at consequat. Mauris rhoncus ligula ut neque semper, eget malesuada sem ultrices. Suspendisse rhoncus convallis lectus. Donec a maximus dolor, id imperdiet quam. Quisque elementum odio placerat turpis venenatis, sit amet finibus massa viverra. Cras et ullamcorper ex. Nulla facilisi. Pellentesque venenatis interdum arcu vel elementum. Phasellus sodales justo hendrerit, mollis libero in, rutrum neque. " +

            "Duis hendrerit consectetur porttitor. Proin vel massa rhoncus, suscipit nibh nec, tincidunt quam. Vivamus iaculis, mauris id egestas aliquet, magna libero vestibulum est, vitae finibus leo odio ac lorem. Sed lacinia, quam ut imperdiet porta, mi turpis accumsan arcu, vitae volutpat libero ex id turpis. Morbi vel ligula ac odio vestibulum tincidunt vitae non nunc. Aliquam vestibulum porta felis, a tincidunt odio auctor eget. In hac habitasse platea dictumst. Cras rhoncus orci ante, non volutpat quam ultricies vel. Nunc viverra congue scelerisque. Nulla facilisi. Proin condimentum ullamcorper nunc quis lacinia. Aenean condimentum velit id iaculis bibendum. Cras non libero dictum, pellentesque odio blandit, laoreet tortor. Curabitur luctus eros lectus, eu molestie purus fringilla eget. Integer odio velit, interdum a risus at, aliquam mattis dui. ";

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
            UserDTouchCode = e.Parameter as CodeJson;
            this.navigationHelper.OnNavigatedTo(e);
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

        private void ReadMoreClick(object sender, RoutedEventArgs e)
        {

        }
        #endregion
    }
}
