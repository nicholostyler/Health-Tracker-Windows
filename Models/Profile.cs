using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health_Track.Models
{
    public class Profile : INotifyPropertyChanged
    {
        private string _name;
        public String Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private DateTimeOffset _goalDate;
        public DateTimeOffset GoalDate
        {
            get { return _goalDate; }
            set
            {
                _goalDate = value;
                NotifyPropertyChanged("GoalDate");
            }
        }

        private double _goalWeight;
        public double GoalWeight
        {
            get { return _goalWeight; }
            set
            {
                _goalWeight = value;
                NotifyPropertyChanged(nameof(TargetWeightLabel));
            }
        }

        private double _currentWeight;
        public double CurrentWeight
        {
            get { return _currentWeight; }
            set
            {
                _currentWeight = value;
                NotifyPropertyChanged(nameof(CurrentWeightLabel));
            }
        }

        private double _weight7Days;
        public double Weight7Days
        {
            get { return _weight7Days; }
            set
            {
                _weight7Days = value;
                NotifyPropertyChanged(nameof(Last7DaysLabel));
            }
        }

        private double _weight30Days;
        public double Weight30Days
        {
            get { return _weight30Days; }
            set
            {
                _weight30Days = value;
                NotifyPropertyChanged(nameof(Last30DaysLabel));
            }
        }

        public double _weightLastYear;
        public double WeightLastYear
        {
            get { return _weightLastYear; }
            set
            {
                _weightLastYear = value;
                NotifyPropertyChanged(nameof(WeightLastYear));
            }
        }

        public double _startingWeight;
        public double StartingWeight
        {
            get { return _startingWeight; }
            set
            {
                _startingWeight = value;
                NotifyPropertyChanged(nameof(StartingWeightLabel));
            }
        }

        public double _totalLost;
        public double TotalLost
        {
            get { return _totalLost; }
            set
            {
                _totalLost = value;
                NotifyPropertyChanged(nameof(TotalLostLabel));
            }
        }

        public int _goalRate;
        public int GoalRate
        {
            get { return _goalRate; }
            set
            {
                _goalRate = value;
                NotifyPropertyChanged("GoalRate");
            }
        }

        public string CurrentWeightLabel
        {
            get { return _currentWeight + " lbs"; }
        }

        public string Last7DaysLabel
        {
            
            get {
                if (_weight7Days > 0)
                    return _weight7Days + " lost";
                else
                    return _weight7Days + " gained";
            }
        }

        public string Last30DaysLabel
        {

            get
            {
                if (_weight30Days > 0)
                    return _weight30Days + " lost";
                else
                    return _weight30Days + " gained";
            }
        }

        public string LastYearLabel
        {

            get
            {
                if (_weightLastYear > 0)
                    return _weightLastYear + " lost";
                else
                    return _weightLastYear + " gained";
            }
        }

        public string TargetWeightLabel
        {
            get
            {
                return _goalWeight + " lbs";
            }
        }

        public string StartingWeightLabel
        {
            get
            {
                return _startingWeight + " lbs";
            }
        }

        public string TotalLostLabel
        {
            get
            {
                return _totalLost + " lbs";
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
