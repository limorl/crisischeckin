using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CrisisCheckinApp.ServiceClient;

namespace CrisisCheckinApp
{
    public partial class LandingPage : PhoneApplicationPage
    {
        public LandingPage()
        {
            InitializeComponent();      
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (App.IsSignedIn && App.UserProfile != null)
            {
                this.signingPanel.Visibility = Visibility.Collapsed;
            }

            base.OnNavigatedTo(e);
        }
        
        private void signin_Click(object sender, RoutedEventArgs e)
        {
            App app = Application.Current as App;
            if (app != null)
            {
                app.BeginSignin(new UserCredentials{ UserName = this.txtUserName.Text, AuthToken = "dummy-token" });
                 Thread waitForUserDataThread = new Thread(WaitForUserData);
                 waitForUserDataThread.Start();
            }
        }

        private void WaitForUserData()
        {
            App app = Application.Current as App;
            if (app != null)
            {
                // Next - wait for the user data to be loaded
                bool userLoaded = app.UserDataLoaded.WaitOne(0);
                if (!userLoaded)
                {
                    userLoaded = app.UserDataLoaded.WaitOne(TimeSpan.FromSeconds(3));
                }

                if (userLoaded && App.UserProfile != null)
                {
                    App.MainViewModel.UserName = App.UserProfile.UserName;
                    Dispatcher.BeginInvoke(() =>
                    {
                        // If the was opened by toast mark it so we will navigate directly to the nudges page
                        if (NavigationContext.QueryString.Any())
                        {
                            NavigationContext.QueryString.Clear();
                        }
                        NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                    });
                }
                else
                {
                    // TODO : handle error
                }
            }
        }

    }
}