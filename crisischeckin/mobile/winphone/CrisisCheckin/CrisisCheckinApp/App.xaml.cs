using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Resources;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CrisisCheckinApp.Resources;
using CrisisCheckinApp.ViewModels;
using System.Net;
using CrisisCheckinApp.ServiceClient;
using System.IO.IsolatedStorage;
using System.Threading;

namespace CrisisCheckinApp
{
    public partial class App : Application
    {
        private Mutex isoSettingsMutex = new Mutex(false, AppSettings.IsoSettingsMutexName);
        public ICrisisCheckinService checkinClient { get; private set; }

        public ManualResetEvent UserDataLoaded = new ManualResetEvent(false);

        # region App State

        public static bool IsSignedIn { get; private set; } 
        public static UserProfile UserProfile { get ; private set; }
        public static UserCredentials UserCredentials { get; private set; }
        public static IEnumerable<string> Clusters { get; private set; }
        public static IEnumerable<Disaster> OngoingDisasters { get; private set; }

        #endregion 

        #region View Models

        private static MainViewModel mainViewModel = null;

        /// <summary>
        /// A static ViewModel used by the views to bind against.
        /// </summary>
        /// <returns>The MainViewModel object.</returns>
        public static MainViewModel MainViewModel
        {
            get
            {
                // Delay creation of the view model until necessary
                if (mainViewModel == null)
                    mainViewModel = new MainViewModel();

                return mainViewModel;
            }
        }

        private static CheckinViewModel checkiViewModel = null;

        /// <summary>
        /// A static ViewModel used by the views to bind against.
        /// </summary>
        /// <returns>The MainViewModel object.</returns>
        public static CheckinViewModel CheckinViewModel
        {
            get
            {
                // Delay creation of the view model until necessary
                if (checkiViewModel == null)
                    checkiViewModel = new CheckinViewModel();

                return checkiViewModel;
            }
        }

        #endregion
 
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public static PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions.
            UnhandledException += Application_UnhandledException;

            App.OngoingDisasters = new List<Disaster>{};

            this.checkinClient = new CrisisCheckinServiceClient();
            
            // Standard XAML initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Language display initialization
            InitializeLanguage();

            // Show graphics profiling information while debugging.
            if (Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode,
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Prevent the screen from turning off while under the debugger by disabling
                // the application's idle detection.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            InitializeApp();
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            InitializeApp();
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            // Ensure that required application state is persisted here.
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        private void InitializeApp()
        {
            LoadState();
            BeginUpdateDisasters();
            // Ensure that application state is restored appropriately
            if (!App.MainViewModel.IsDataLoaded)
            {
                App.MainViewModel.LoadData();
            }
        }
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
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Handle reset requests for clearing the backstack
            RootFrame.Navigated += CheckForResetNavigation;

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

        private void CheckForResetNavigation(object sender, NavigationEventArgs e)
        {
            // If the app has received a 'reset' navigation, then we need to check
            // on the next navigation to see if the page stack should be reset
            if (e.NavigationMode == NavigationMode.Reset)
                RootFrame.Navigated += ClearBackStackAfterReset;
        }

        private void ClearBackStackAfterReset(object sender, NavigationEventArgs e)
        {
            // Unregister the event so it doesn't get called again
            RootFrame.Navigated -= ClearBackStackAfterReset;

            // Only clear the stack for 'new' (forward) and 'refresh' navigations
            if (e.NavigationMode != NavigationMode.New && e.NavigationMode != NavigationMode.Refresh)
                return;

            // For UI consistency, clear the entire page stack
            while (RootFrame.RemoveBackEntry() != null)
            {
                ; // do nothing
            }
        }

        private void LoadState()
        {
            if (isoSettingsMutex.WaitOne(AppSettings.MutexWaitTimeout))
            {
                try
                {
                    // Load the IsSignedIn flag
                    bool isSignedIn = false;
                    App.IsSignedIn = isSignedIn;
                    if (IsolatedStorageSettings.ApplicationSettings.TryGetValue<bool>(AppSettings.IsSignedInKey, out isSignedIn))
                    {
                        App.IsSignedIn = isSignedIn;
                    }

                    // Load the IsSignedIn flag
                    UserCredentials creds = null;
                    if (IsolatedStorageSettings.ApplicationSettings.TryGetValue<UserCredentials>(AppSettings.UserCredentialsKey, out creds))
                    {
                        App.UserCredentials = creds;
                    }

                    // Load the user profile
                    UserProfile user = null;
                    IsolatedStorageSettings.ApplicationSettings.TryGetValue<UserProfile>(AppSettings.UserProfileKey, out user);
                    UserProfile = user;

                    // Load Disasters 
                    IEnumerable<Disaster> disasters = null;
                    IsolatedStorageSettings.ApplicationSettings.TryGetValue<IEnumerable<Disaster>>(AppSettings.DisastersKey, out disasters);
                       OngoingDisasters = disasters;
                }
                finally
                {
                    isoSettingsMutex.ReleaseMutex();
                }
            }
            else
            {
                // TODO: output error to logger/
                // Throw exception and exit as we have nothing to do...
                throw new TimeoutException("Waiting for application settings mutex timed out");
            }
        }

        private void StoreState()
        {
            if (isoSettingsMutex.WaitOne(AppSettings.MutexWaitTimeout))
            {
                try
                {
                    IsolatedStorageSettings.ApplicationSettings[AppSettings.IsSignedInKey] = IsSignedIn;
                    IsolatedStorageSettings.ApplicationSettings[AppSettings.UserProfileKey] = UserProfile;
                    IsolatedStorageSettings.ApplicationSettings[AppSettings.UserCredentialsKey] = UserCredentials;
                    IsolatedStorageSettings.ApplicationSettings[AppSettings.DisastersKey] = OngoingDisasters;
                    IsolatedStorageSettings.ApplicationSettings.Save();
                }
                finally
                {
                    isoSettingsMutex.ReleaseMutex();
                }
            }
            else
            {
                // TODO: output error to logger/
            }
        }

        #endregion

        // Initialize the app's font and flow direction as defined in its localized resource strings.
        //
        // To ensure that the font of your application is aligned with its supported languages and that the
        // FlowDirection for each of those languages follows its traditional direction, ResourceLanguage
        // and ResourceFlowDirection should be initialized in each resx file to match these values with that
        // file's culture. For example:
        //
        // AppResources.es-ES.resx
        //    ResourceLanguage's value should be "es-ES"
        //    ResourceFlowDirection's value should be "LeftToRight"
        //
        // AppResources.ar-SA.resx
        //     ResourceLanguage's value should be "ar-SA"
        //     ResourceFlowDirection's value should be "RightToLeft"
        //
        // For more info on localizing Windows Phone apps see http://go.microsoft.com/fwlink/?LinkId=262072.
        //
        private void InitializeLanguage()
        {
            try
            {
                // Set the font to match the display language defined by the
                // ResourceLanguage resource string for each supported language.
                //
                // Fall back to the font of the neutral language if the Display
                // language of the phone is not supported.
                //
                // If a compiler error is hit then ResourceLanguage is missing from
                // the resource file.
                RootFrame.Language = XmlLanguage.GetLanguage(AppResources.ResourceLanguage);

                // Set the FlowDirection of all elements under the root frame based
                // on the ResourceFlowDirection resource string for each
                // supported language.
                //
                // If a compiler error is hit then ResourceFlowDirection is missing from
                // the resource file.
                FlowDirection flow = (FlowDirection)Enum.Parse(typeof(FlowDirection), AppResources.ResourceFlowDirection);
                RootFrame.FlowDirection = flow;
            }
            catch
            {
                // If an exception is caught here it is most likely due to either
                // ResourceLangauge not being correctly set to a supported language
                // code or ResourceFlowDirection is set to a value other than LeftToRight
                // or RightToLeft.

                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                throw;
            }
        }

        
        #region Service Requests

        public void BeginSignin(UserCredentials credentials)
        {
            // TODO: add service calls for autentication if needed
            try
            {
                if (!IsSignedIn || App.UserProfile == null)
                {
                    checkinClient.SigninAsync(credentials, Signin_Completed);
                }
            }
            catch (Exception)
            {
                UserDataLoaded.Set();
            }
        }

        public void Signin_Completed(ServiceResponseStatus status, UserProfile user)
        {
            if (status != null && status.IsSuccess)
            {
                App.UserProfile = user;
                IsSignedIn = true;
                UserDataLoaded.Set();
            }
        }

        private void BeginUpdateDisasters()
        {
            if (App.OngoingDisasters == null)
            {
                checkinClient.GetDisastersAsync(null, GetDisasters_Completed);
            }
        }

        public void  GetDisasters_Completed(ServiceResponseStatus status, IEnumerable<Disaster> disasters)
        {
            if (status != null && status.IsSuccess)
            {
                App.OngoingDisasters = disasters;
            }
        }

        #endregion
    }
}