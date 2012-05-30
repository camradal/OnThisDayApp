using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using OnThisDayApp.Resources;

namespace OnThisDayApp
{
    public class NewItem
    {
        public string Version { get; set; }
        public string Description { get; set; }
    }

    public partial class AboutPage : PhoneApplicationPage
    {
        public List<NewItem> NewItems
        {
            get
            {
                return new List<NewItem>()
                {
                    new NewItem() { Version = "", Description = "We develop based on your feedback, please drop us a line with any suggestions." },
                    new NewItem() { Version = "1.8", Description = "Added a setting to increase font size" },
                    new NewItem() { Version = "1.7", Description = "Added a setting to sort the events by newest or oldest\nAd-free version is available\nAdded a button to reset local data" },
                    new NewItem() { Version = "1.5", Description = "Sharing functionality is now available, tap and hold any entry to share on Facebook or Twitter\nA lot of optimizations for best experience on Tango phones" },
                    new NewItem() { Version = "1.3", Description = "Major improvements to the live tile, it will now display interesting facts on the front" },
                    new NewItem() { Version = "1.2", Description = "Live tile now displays interesting events. Pin it to your start screen for daily historical highlights" },
                    new NewItem() { Version = "1.1", Description = "Displaying holidays\nDisplaying images in highlights" },
                    new NewItem() { Version = "1.0", Description = "On This Day... is released!" }
                };
            }
        }

        public AboutPage()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            ReadVersionFromManifest();
        }

        private void feedbackButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EmailComposeTask task = new EmailComposeTask();
                task.Subject = Strings.FeedbackOn;
                task.Body = Strings.FeedbackTemplate;
                task.To = Strings.ContactEmail;
                task.Show();
            }
            catch
            {
                // prevent exceptions from double-click
            }
        }

        private void rateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MarketplaceReviewTask task = new MarketplaceReviewTask();
                task.Show();
            }
            catch
            {
                // prevent exceptions from double-click
            }
        }

        private void ReadVersionFromManifest()
        {
            Uri manifest = new Uri("WMAppManifest.xml", UriKind.Relative);
            var si = Application.GetResourceStream(manifest);
            if (si != null)
            {
                using (StreamReader sr = new StreamReader(si.Stream))
                {
                    bool haveApp = false;
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (!haveApp)
                        {
                            int i = line.IndexOf("AppPlatformVersion=\"", StringComparison.InvariantCulture);
                            if (i >= 0)
                            {
                                haveApp = true;
                                line = line.Substring(i + 20);

                                int z = line.IndexOf("\"");
                                if (z >= 0)
                                {
                                    // if you're interested in the app plat version at all
                                    // AppPlatformVersion = line.Substring(0, z);
                                }
                            }
                        }

                        int y = line.IndexOf("Version=\"", StringComparison.InvariantCulture);
                        if (y >= 0)
                        {
                            int z = line.IndexOf("\"", y + 9, StringComparison.InvariantCulture);
                            if (z >= 0)
                            {
                                // We have the version, no need to read on.
                                versionText.Text = line.Substring(y + 9, z - y - 9);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                versionText.Text = "Unknown";
            }
        }
    }
}