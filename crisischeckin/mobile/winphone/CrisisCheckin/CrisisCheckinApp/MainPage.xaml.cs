using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CrisisCheckinApp.ServiceClient;
using CrisisCheckinApp.ViewModels;

namespace CrisisCheckinApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            DataContext = App.MainViewModel;
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.MainViewModel.IsDataLoaded)
            {
                App.MainViewModel.LoadData();
            }
        }
       
        private void OngoingDisaster_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var selectedDisaster = ((sender as TextBlock).DataContext as DisasterViewModel).Disaster;

            // checkin URI should include the disaster id and disaster name
            var uri = string.Format("/CheckinPage.xaml?did={0}&dname={1}", selectedDisaster.Id, selectedDisaster.Name); 
            NavigationService.Navigate(new Uri(uri, UriKind.Relative));

        }

    }
}