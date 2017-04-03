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

            // Construct the toast content
            ToastContent toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = "File downloading..."
                            },

                            new AdaptiveProgressBar()
                            {
                                Title = "InteractiveToastSample.zip",
                                Value = new BindableProgressBarValue("progressValue"),
                                ValueStringOverride = new BindableString("progressValueStringOverride"),
                                Status = "Downloading..."
                            }
                        }
                    }
                }
            };

            string tag = DateTime.Now.GetHashCode().ToString();

            var data = new Dictionary<string, string>
            {
                { "progressValue", "0" },
                { "progressValueStringOverride", $"{downloadDuration} seconds" }
            };

            // And create the toast notification
            ToastNotification notification = new ToastNotification(toastContent.GetXml())
            {
                Tag = tag,
                Data = new NotificationData(data)
            };
            
            // And then send the toast
            ToastNotificationManager.CreateToastNotifier().Show(notification);

            DownloadsModel.StartDownload(tag, downloadDuration);
        }

        private static bool? _isProgressBarSupported;
        private static bool IsProgressBarSupported()
        {
            if (_isProgressBarSupported == null)
            {
                // Progress bar only supported in RS2, only on Desktop
                _isProgressBarSupported = ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 4)
                    && (AnalyticsInfo.VersionInfo.DeviceFamily.Equals("Windows.Desktop"));
            }

            return _isProgressBarSupported.Value;
        }
    }
}
