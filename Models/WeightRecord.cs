using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health_Track.Models
{
    public class WeightRecord : INotifyPropertyChanged
    {
        public double _weight;
        public double Weight
        {
            get { return _weight; }
            set 
            { 
                _weight = value;
                NotifyPropertyChanged("Weight");
            }
        }
        public DateTimeOffset _date;
        public DateTimeOffset Date
        {
            get { return _date; }
            set 
            {
                _date = value;
                NotifyPropertyChanged("Date");
            }
        }

        public bool _isSaveEnabled;
        public bool IsSaveEnabled
        {
            get { return _isSaveEnabled; }
            set
            {
                _isSaveEnabled = value;
                NotifyPropertyChanged("IsSavedEnabled");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
