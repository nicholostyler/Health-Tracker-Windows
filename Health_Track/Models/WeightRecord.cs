using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Health_Track.Models
{
    public class WeightRecord : INotifyPropertyChanged
    {
        [JsonIgnore]
        public double _weight;

        [JsonInclude]
        public double Weight
        {
            get { return _weight; }
            set 
            { 
                _weight = value;
                NotifyPropertyChanged("Weight");
            }
        }

        [JsonIgnore]
        public DateTimeOffset _date;

        [JsonInclude]
        public DateTimeOffset Date
        {
            get { return _date; }
            set 
            {
                _date = value;
                NotifyPropertyChanged("Date");
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
