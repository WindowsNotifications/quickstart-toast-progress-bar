using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace QuickstartToastProgressBar.Model
{
    public static class DownloadsModel
    {
        private class DownloadInstance
        {
            private static Random RAND = new Random();
            private static ToastNotifier NOTIFIER = ToastNotificationManager.CreateToastNotifier();

            public string ToastTag { get; private set; }
            public double DurationInSeconds { get; private set; }
            public double SecondsRemaining { get; private set; }
            private double _intervalOfDownloadProgress { get; set; }
            public double ProgressValue
            {
                get
                {
                    if (SecondsRemaining <= 0)
                    {
                        return 1;
                    }

                    return (DurationInSeconds - SecondsRemaining) / DurationInSeconds;
                }
            }
            private uint _version = 0;

            public DownloadInstance(string toastTag, int durationInSeconds)
            {
                ToastTag = toastTag;
                DurationInSeconds = durationInSeconds;
                SecondsRemaining = durationInSeconds;

                _intervalOfDownloadProgress = durationInSeconds;
            }

            public async Task DownloadAsync()
            {
                while (SecondsRemaining > 0)
                {
                    // Somewhere between 0.5 and 1.5 seconds
                    double thisDownloadSegment = RAND.NextDouble() + 0.5;

                    await Task.Delay((int)(thisDownloadSegment * 1000));

                    SecondsRemaining -= thisDownloadSegment;
                    _version++;

                    UpdateToast();
                }

                SendCompletedToast();
            }

            private void UpdateToast()
            {
                var data = new Dictionary<string, string>
                {
                    { "progressValue", ProgressValue.ToString() },
                    { "progressValueStringOverride", $"{(int)SecondsRemaining} seconds" }
                };

                try
                {
                    NOTIFIER.Update(new NotificationData(data, _version), ToastTag);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }

            private void SendCompletedToast()
            {
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
                                    Text = "Download completed!"
                                },

                                new AdaptiveText()
                                {
                                    Text = "InteractiveToastSample.zip"
                                }
                            }
                        }
                    }
                };

                ToastNotification notification = new ToastNotification(toastContent.GetXml())
                {
                    Tag = ToastTag
                };

                NOTIFIER.Show(notification);
            }
        }

        private static List<DownloadInstance> _downloads = new List<DownloadInstance>();

        public static async void StartDownload(string toastTag, int durationInSeconds)
        {
            var instance = new DownloadInstance(toastTag, durationInSeconds);
            _downloads.Add(instance);

            await instance.DownloadAsync();

            // Once it's finished downloading, remove
            _downloads.Remove(instance);
        }
    }
}
