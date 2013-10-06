using CrisisCheckinApp.ServiceClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisisCheckinApp.ViewModels
{
    public class CheckinViewModel : INotifyPropertyChanged
    {
        private Disaster disaster;

        public Disaster Disaster
        {
            get
            {
                return this.disaster;
            }
            set
            {
                this.disaster = value;
                NotifyPropertyChanged("Disaster");
            }
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
