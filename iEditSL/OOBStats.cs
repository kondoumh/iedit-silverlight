using System.ComponentModel;

namespace iEditSL
{
    public class OOBStats : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool isNetworkAvailable;
        private string runningModeMessage;
        private string networkAvailabilityStatus;
        private double networkIsNotAvailableOpacity;
        private double networkIsAvailableOpacity;
        private string installStateMessage;

        protected void FirePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public string NetworkAvailabilityStatus
        {
            get { return networkAvailabilityStatus; }
            set
            {
                networkAvailabilityStatus = value;
                FirePropertyChanged("NetworkAvailabilityStatus");
            }
        }

        public double NetworkIsNotAvailableOpacity
        {
            get { return networkIsNotAvailableOpacity; }
            set
            {
                networkIsNotAvailableOpacity = value;
                FirePropertyChanged("NetworkIsNotAvailableOpacity");
            }
        }

        public double NetworkIsAvailableOpacity
        {
            get { return networkIsAvailableOpacity; }
            set
            {
                networkIsAvailableOpacity = value;
                FirePropertyChanged("NetworkIsAvailableOpacity");
            }
        }

        public bool IsNetworkAvailable
        {
            get { return isNetworkAvailable; }
            set 
            { 
                isNetworkAvailable = value; 
                FirePropertyChanged("IsNetworkAvailable");
                NetworkIsAvailableOpacity = (isNetworkAvailable ? 1 : .25);
                NetworkIsNotAvailableOpacity = (isNetworkAvailable ? .25 : 1);
                NetworkAvailabilityStatus = (isNetworkAvailable ? "ネットワークOK" : "ネットワークNG");
            }
        }

        public string RunningModeMessage
        {
            get { return runningModeMessage; }
            set
            {
                runningModeMessage = value;
                FirePropertyChanged("RunningModeMessage");
            }
        }

        public string InstallStateMessage
        {
            get { return installStateMessage; }
            set
            {
                installStateMessage = value;
                FirePropertyChanged("InstallStateMessage");
            }
        }
    }
}