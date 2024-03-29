﻿using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.Devices;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using XEurope.Common;
using XEurope.JsonClasses;

namespace XEurope.View
{
    public sealed partial class CameraPage : Page
    {
        private const int NO_OF_TILES = 2;

        //private List<MatOfPoint> components;
        //private MarkerDetector markerDetector;
        private String MarkerString = "";
        private bool isMarkerDetected = false;
/*
        private Windows.Foundation.Rect markerPosition;
*/
        private byte[] sendingBytes;
/*
        private string _result;
*/

        private MediaCapture mediaCapture;
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        //private OpenCVComponent.OpenCVLib cvLib = new OpenCVComponent.OpenCVLib();

        public CameraPage()
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
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // Set the display to keep alive
            var displayRequest = new Windows.System.Display.DisplayRequest();
            displayRequest.RequestActive();

            // Start the camera preview
            await ScanPreviewBuffer();

            // Resize the analyzation area's frame according to the camera preview's size
            detectorRectangle.Visibility = Windows.UI.Xaml.Visibility.Visible;
            detectorRectangle.Width = PreviewElement.ActualHeight / 2;
            detectorRectangle.Height = PreviewElement.ActualHeight / 2;
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Release resources
            if (mediaCapture != null)
            {
                
                await mediaCapture.StopPreviewAsync();
                PreviewElement.Source = null;
                mediaCapture.Dispose();
                mediaCapture = null;
            }

            //var displayRequest = new Windows.System.Display.DisplayRequest();
            //displayRequest.RequestRelease();

            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async Task ScanPreviewBuffer()
        {
            try
            {
                // First need to find all webcams
                DeviceInformationCollection webcamList = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

                // Then I do a query to find the front webcam
                DeviceInformation frontWebcam = (from webcam in webcamList
                    where webcam.EnclosureLocation != null
                          && webcam.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Front
                    select webcam).FirstOrDefault();

                // Same for the back webcam
                DeviceInformation backWebcam = (from webcam in webcamList
                    where webcam.EnclosureLocation != null
                          && webcam.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Back
                    select webcam).FirstOrDefault();

                // Then you need to initialize your MediaCapture
                mediaCapture = new MediaCapture();
                await mediaCapture.InitializeAsync(new MediaCaptureInitializationSettings
                {
                    // Choose the webcam you want (backWebcam or frontWebcam)
                    VideoDeviceId = backWebcam.Id,
                    AudioDeviceId = "",
                    StreamingCaptureMode = StreamingCaptureMode.Video,
                    PhotoCaptureSource = PhotoCaptureSource.VideoPreview
                });

                var focusSettings = new FocusSettings();
                focusSettings.AutoFocusRange = AutoFocusRange.FullRange;
                focusSettings.Mode = FocusMode.Auto;
                focusSettings.WaitForFocus = true;
                focusSettings.DisableDriverFallback = false;

                mediaCapture.VideoDeviceController.FocusControl.Configure(focusSettings);

                var maxResolution = mediaCapture.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.Photo).Aggregate((i1, i2) => (i1 as VideoEncodingProperties).Width > (i2 as VideoEncodingProperties).Width ? i1 : i2);
                await mediaCapture.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.Photo, maxResolution);

                mediaCapture.SetPreviewRotation(VideoRotation.Clockwise90Degrees);
                mediaCapture.SetRecordRotation(VideoRotation.Clockwise90Degrees);

                // Set auto flashlight to false
                var fc = mediaCapture.VideoDeviceController.FlashControl;
                fc.Auto = false;

                // Set the source of the CaptureElement to your MediaCapture
                PreviewElement.Source = mediaCapture;

                // Start the preview
                await mediaCapture.StartPreviewAsync();
            }
            catch (Exception ex)
            {
                MessageDialog errorDialog = new MessageDialog(ex.Message, "Error");
                errorDialog.ShowAsync();
            }
        }

        private static async Task<DeviceInformation> GetCameraDeviceInfoAsync(
            Windows.Devices.Enumeration.Panel desiredPanel)
        {

            DeviceInformation device = (await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture))
                .FirstOrDefault(d => d.EnclosureLocation != null && d.EnclosureLocation.Panel == desiredPanel);

            if (device == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                    "No suitable devices found for the camera of type {0}.", desiredPanel));
            }
            return device;
        }

        private async void CaptureButton_Click(object sender, RoutedEventArgs e)
        {
            // Take snapshot and add to ListView
            // Disable button to prevent exception due to parallel capture usage
            BtnCapturePhoto.IsEnabled = false;
            AnalyzeProgressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;

            var stream = new InMemoryRandomAccessStream();

            await mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), stream);

            try
            {
                var responseData = await SendStreamToProcess(stream.CloneStream().AsStream());

                var title = responseData.error
                    ? "Failed"
                    : "Success";
                var message = string.IsNullOrEmpty(responseData.message)
                    ? "Marker is not recognized"
                    : responseData.message;
                var dialog = new MessageDialog(message, title);
                
                AnalyzeProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                BtnCapturePhoto.IsEnabled = true;

                if (!responseData.error)
                {
                    setMarkerDetected(true);
                    MarkerString = responseData.message;
                }
                else
                {
                    await dialog.ShowAsync();
                }

                if (isMarkerDetected)
                {
                    (this.Parent as Frame).Navigate(typeof(DetailPage), new CodeJson { code = MarkerString });
                }
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog(ex.Message, "Error");
                dialog.ShowAsync();
            }
        }

        private async void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            (this.Parent as Frame).Navigate(typeof(HistoryPage));
        }

        private async void TorchButton_Click(object sender, RoutedEventArgs e)
        {
            var videoDev = mediaCapture.VideoDeviceController;
            var tc = videoDev.TorchControl;
            if (tc.Supported)
            {
                if (!tc.Enabled)
                {
                    if (tc.PowerSupported)
                        tc.PowerPercent = 100;
                    tc.Enabled = true;
                }
                else
                {
                    if (tc.PowerSupported)
                        tc.PowerPercent = 0;
                    tc.Enabled = false;
                }   
            }
        }

        private async Task<ErrorJson> SendStreamToProcess(Stream mediaStream)
        {
            sendingBytes = Converters.ConvertStreamToBytes(mediaStream);
            var base64 = Convert.ToBase64String(sendingBytes);
            base64 = base64 ?? "";

            #if (EMULATOR)
            {  
                //get sample image byte[] from string.
                sendingBytes = ConnHelper.GetSampleImageBytes();
            }
            #endif

            var markerUri = new Uri(ConnHelper.DtouchProcessUri);
            var myUri = new Uri(ConnHelper.DtouchProcessUri);

            var postData = JsonConvert.SerializeObject(new JsonImage{ImageBytes = sendingBytes});

            var resp = await ConnHelper.PostToUri(markerUri, postData);
            var responseData = (ErrorJson)JsonConvert.DeserializeObject(resp, typeof(ErrorJson));
            if (responseData.error)
            {
                return responseData;
            }
            postData = JsonConvert.SerializeObject(new CodeJson { code = responseData.message });
            var serviceUri = new Uri(ConnHelper.BaseUri + "isValid");
            var validString = await ConnHelper.PostToUri(serviceUri, postData);
            var validData = (ErrorJson)JsonConvert.DeserializeObject(validString, typeof(ErrorJson));
            if (validData.error)
            {
                //the dtouch code does not belong to anyone or not exist.
                //todo additional message
                responseData.error = true;
                responseData.message = validData.message;
            }
            return responseData;
        }

        private void setMarkerDetected(bool detected)
        {
            isMarkerDetected = detected;
        }

        private void NewPasswButton_Click(object sender, RoutedEventArgs e)
        {
            (this.Parent as Frame).Navigate(typeof(PasswordChangePage));

        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            ConnHelper.Logout();

            (this.Parent as Frame).Navigate(typeof(MainPage));
        }
    }
}
