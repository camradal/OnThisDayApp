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
                    new NewItem
                    {
                        Version = "",
                        Description = "We read all emails, please drop us a line with any suggestions."
                    },
                    new NewItem
                    {
                        Version = "2.9.4",
                        Description =
                            "- Bug fixes for Windows Phone 7"
                    },
                    new NewItem
                    {
                        Version = "2.9.2",
                        Description =
                            "- Transparent live tile"
                    },
                    new NewItem
                    {
                        Version = "2.9.1",
                        Description =
                            "- Performance improvements\n" +
                            "- Bug fixes"
                    },
                    new NewItem
                    {
                        Version = "2.9",
                        Description =
                            "- Implement sharing from wikipedia page\n" +
                            "- Important fix for settings getting reset"
                    },
                    new NewItem
                    {
                        Version = "2.8",
                        Description =
                            "- Orientation lock option\n" +
                            "- Remove ads on wikipedia page\n" +
                            "- Share via clipboard"
                    },
                    new NewItem
                    {
                        Version = "2.7",
                        Description =
                            "- Sharing improvements: text, e-mail and social networks\n" +
                            "- Option to show only the front of the live tile\n" +
                            "- Option to open articles in Internet Explorer\n" +
                            "- Live tile improvements"
                    },
                    new NewItem
                    {
                        Version = "2.6",
                        Description =
                            "- Fix reported bugs"
                    },
                    new NewItem
                    {
                        Version = "2.5",
                        Description =
                            "- Support for Windows Phone 7.8\n" +
                            "- Secondary tile bugs fixed"
                    },
                    new NewItem
                    {
                        Version = "2.4",
                        Description =
                            "- High-resolution and large tiles for Windows Phone 8\n" +
                            "- Bugs fixed"
                    },
                    new NewItem
                    {
                        Version = "2.3",
                        Description =
                            "- Windows Phone 8 compatibility updates\n" + 
                            "- Low-memory phones improvements\n" + 
                            "- Share historical events via email\n" +
                            "- Bugs fixed"
                    },
                    new NewItem
                    {
                        Version = "2.2",
                        Description =
                            "- Visual update, new animations\n" + 
                            "- Pin live tile from the app\n" + 
                            "- Bugs fixed"
                    },
                    new NewItem
                    {
                        Version = "2.1",
                        Description =
                            "- Major performance improvements - events load twice as fast\n" + 
                            "- Live tile updates more reliably now"
                    },
                    new NewItem
                    {
                        Version = "2.0",
                        Description =
                            "- Group and filter events, births and deaths by centuries and decades\n" + 
                            "- Check what happened on any birthday or event from your calendar\n" + 
                            "- New setting to increase font size"
                    },
                    new NewItem
                    {
                        Version = "1.7",
                        Description =
                            "- Added a setting to sort the events by newest or oldest\n" + 
                            "- Ad-free version is available\n" + 
                            "- Added a button to reset local data"
                    },
                    new NewItem
                    {
                        Version = "1.5",
                        Description =
                            "- Sharing functionality is now available, tap and hold any entry to share on Facebook or Twitter\n" + 
                            "- A lot of optimizations for best experience on Tango phones"
                    },
                    new NewItem
                    {
                        Version = "1.3",
                        Description =
                            "- Major improvements to the live tile, it will now display interesting facts on the front"
                    },
                    new NewItem
                    {
                        Version = "1.2",
                        Description =
                            "- Live tile now displays interesting events. Pin it to your start screen for daily historical highlights"
                    },
                    new NewItem
                    {
                        Version = "1.1",
                        Description =
                            "- Displaying holidays\n" + 
                            "- Displaying images in highlights"
                    },
                    new NewItem
                    {
                        Version = "1.0",
                        Description =
                            "- On This Day... is released!"
                    }
                };
            }
        }

        public AboutPage()
        {
            InitializeComponent();
            ReadVersionFromManifest();
            DataContext = this;
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
            versionText.Text = "2.9.4";
        }
    }
}