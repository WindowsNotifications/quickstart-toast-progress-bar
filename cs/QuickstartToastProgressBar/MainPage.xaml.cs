using Microsoft.Toolkit.Uwp.Notifications;
using QuickstartToastProgressBar.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.System.Profile;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace QuickstartToastProgressBar
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void ButtonSendToast_Click(object sender, RoutedEventArgs e)
        {
            int downloadDuration;
            if (!int.TryParse(TextBoxSecondsToDownloadFor.Text, out downloadDuration) || downloadDuration <= 0)
            {
                var dontWait = new MessageDialog("Seconds to download for must be a positive integer").ShowAsync();
                return;
            }

            // In a real app, these would be initialized with actual data
            string title = "Andrew Bares";
            string content = "Cannot wait to try your UWP app!";

            string xml = @"<toast>
<visual>
<binding template='ToastGeneric'>
<text>Downloading...</text>
<progress title='InteractiveToastSample.zip' description='File download' value='{progressValue}' state='{progressState}'/>
</binding>
</visual>
</toast>";

            // Construct the toast content
            //ToastContent toastContent = new ToastContent()
            //{
            //    Visual = new ToastVisual()
            //    {
            //        BindingGeneric = new ToastBindingGeneric()
            //        {
            //            Children =
            //            {
            //                new AdaptiveText()
            //                {
            //                    Text = title
            //                },

            //                new AdaptiveText()
            //                {
            //                    Text = content
            //                }
            //            }
            //        }
            //    }
            //};

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            string tag = DateTime.Now.GetHashCode().ToString();

            var data = new Dictionary<string, string>
            {
                { "progressValue", "0" },
                { "progressState", $"{downloadDuration} seconds" }
            };

            // And create the toast notification
            ToastNotification notification = new ToastNotification(doc)
            {
                Tag = tag,
                Data = new NotificationData(data)
            };
            
            // And then send the toast
            ToastNotificationManager.CreateToastNotifier().Show(notification);

            DownloadsModel.StartDownload(tag, 5);
        }

        private static bool? _isProgressBarSupported;
        private static bool IsProgressBarSupported()
        {
            if (_isProgressBarSupported == null)
            {
                // Progress bar only supported in RS2, only on Desktop and Mobile
                _isProgressBarSupported = ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 4)
                    && (AnalyticsInfo.VersionInfo.DeviceFamily.Equals("Windows.Desktop")
                        || AnalyticsInfo.VersionInfo.DeviceFamily.Equals("Windows.Mobile"));
            }

            return _isProgressBarSupported.Value;
        }
    }
}
