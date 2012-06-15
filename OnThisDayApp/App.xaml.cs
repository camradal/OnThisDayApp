using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using AgFx;
using BugSense;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Utilities;

namespace OnThisDayApp
{
    public partial class App : Application
    {
        private const string ApiKeyValue = "B425M7FWBTPLWEPT3XFW";

        public static bool FirstLoad { get; set; }
        public static bool IsMemoryLimited { get; set; }
        public static bool ReloadRequired { get; set; }
        public static bool ReverseRequired { get; set; }
        public static bool FontSizeChanged { get; set; }
        
        public static bool MyDateTimeSet { get; set; }
        public static DateTime MyDateTime { get; set; }
        
        public static Stopwatch Watch { get; set; }

        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            Watch = new Stopwatch();
            Watch.Start();

            var overridenOptions = BugSenseHandler.Instance.GetDefaultOptions();
            overridenOptions.Title = "Oops! Something is wrong";
            overridenOptions.Text = "We've noticed an error has occurred. We've logged it and will fix it in the next update.";
            overridenOptions.Type = enNotificationType.MessageBox;
            BugSenseHandler.Instance.Init(this, "52782d56", overridenOptions);

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Current.Host.Settings.EnableFrameRateCounter = true;

                // Display metro grid helper lines
                MetroGridHelper.IsVisible = false;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            FirstLoad = true;
            IsMemoryLimited = LowMemoryHelper.IsLowMemDevice;
            FlurryWP7SDK.Api.StartSession(ApiKeyValue);
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            FlurryWP7SDK.Api.StartSession(ApiKeyValue);
            IsMemoryLimited = LowMemoryHelper.IsLowMemDevice;
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            //PrintReport();
            DataManager.Current.Flush();
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            //PrintReport();
            DataManager.Current.Flush();
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Data manager statistics
        
        [Conditional("DEBUG")]
        private static void PrintReport()
        {
            using (StringWriter writer = new StringWriter())
            {
                DataManager.Current.GetStatisticsReport(writer, false);

                writer.Flush();

                string output = writer.ToString();
                int time = 0;

                foreach (string ln in output.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    time += 5;
                    Debug.WriteLine(ln);
                }

                Thread.Sleep(time);
            }
        }

        #endregion

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new TransitionFrame()
            {
                Background = new SolidColorBrush(Colors.Transparent)
            };
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Add global loading
            GlobalLoading.Instance.Initialize(RootFrame);

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}