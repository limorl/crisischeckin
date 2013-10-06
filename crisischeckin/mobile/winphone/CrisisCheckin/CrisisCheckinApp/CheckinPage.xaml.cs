using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CrisisCheckinApp.ServiceClient;

namespace CrisisCheckinApp
{
    public partial class CheckinPage : PhoneApplicationPage
    {
        public CheckinPage()
        {
            InitializeComponent();

            DataContext = App.CheckinViewModel;            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string disasterId = string.Empty;

            if (NavigationContext.QueryString.TryGetValue("did", out disasterId) && App.OngoingDisasters != null)
            {
                App.CheckinViewModel.Disaster = App.OngoingDisasters.FirstOrDefault(d => d.Id == disasterId);
            }

            // TODO: handle error
        }

        private void checkinOk_Click(object sender, RoutedEventArgs e)
        {
            // TODO: checkin
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));

        }
    }
}