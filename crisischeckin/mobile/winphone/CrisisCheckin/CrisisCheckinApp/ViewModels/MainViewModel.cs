using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;
using CrisisCheckinApp.Resources;
using CrisisCheckinApp.ServiceClient;

namespace CrisisCheckinApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            this.OngoingDisasters = new ObservableCollection<DisasterViewModel>();
            this.CheckinHistory = new ObservableCollection<CheckinViewModel>();
        }

        public string userName = string.Empty;
        public string UserName
        {
            get
            {
                return userName;
            }
            set
            {
                if (value != userName)
                {
                    userName = value;
                    SigninTitle = value;
                    NotifyPropertyChanged("UserName");
                }
            }
        }

        public string signinTitle = "signin";
        public string SigninTitle
        {
            get
            {
                return signinTitle;
            }
            set
            {
                if (value != signinTitle)
                {
                    signinTitle = value;
                    NotifyPropertyChanged("SigninTitle");
                }
            }
        }

        /// <summary>
        /// A collection for DisasterViewModel objects.
        /// </summary>
        public ObservableCollection<DisasterViewModel> OngoingDisasters { get; private set; }

        /// <summary>
        /// A collection for CheckinViewModel objects.
        /// </summary>
        public ObservableCollection<CheckinViewModel> CheckinHistory { get; private set; }


        private string _sampleProperty = "Sample Runtime Property Value";
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding
        /// </summary>
        /// <returns></returns>
        public string SampleProperty
        {
            get
            {
                return _sampleProperty;
            }
            set
            {
                if (value != _sampleProperty)
                {
                    _sampleProperty = value;
                    NotifyPropertyChanged("SampleProperty");
                }
            }
        }

        /// <summary>
        /// Sample property that returns a localized string
        /// </summary>
        public string LocalizedSampleProperty
        {
            get
            {
                return AppResources.SampleProperty;
            }
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            foreach (var od in App.OngoingDisasters)
            {
                this.OngoingDisasters.Add(new DisasterViewModel { Disaster = od });
            }
          
            this.IsDataLoaded = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}