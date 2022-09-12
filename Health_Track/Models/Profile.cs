﻿using Health_Track.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Health_Track.Models
{
    public class Profile : INotifyPropertyChanged
    {
        private string _name;
        [JsonInclude]
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
        [JsonInclude]
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
        [JsonInclude]
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
        [JsonInclude]
        public double CurrentWeight
        {
            get { return _currentWeight; }
            set
            {
                _currentWeight = value;
                var profile = App.Current.Services.GetRequiredService<WeightRecordViewModel>().Profile;
                var weightMonth = profile.CurrentWeight - (4 * profile.GoalRate);
                var weight3Months = profile.CurrentWeight - ((4 * 3) * profile.GoalRate);
                var weight6Months = profile.CurrentWeight - ((4 * 6) * profile.GoalRate);

                _progressMonth = weightMonth;
                _progress3Months = weight3Months;
                _progress6Months = weight6Months;

                NotifyPropertyChanged(nameof(CurrentWeightLabel));
                NotifyPropertyChanged(nameof(ProgressMonthLabel));
                NotifyPropertyChanged(nameof(Progress3MonthLabel));
                NotifyPropertyChanged(nameof(Progress6MonthLabel));
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
        [JsonInclude]
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
                var currentWeight = _currentWeight;
                var goalWeight = _goalWeight;
                var amountToLose = _startingWeight - _goalWeight;
                var percentage = _goalWeight / _currentWeight;
                _goalPercentage = percentage * 100;
                NotifyPropertyChanged(nameof(TotalLostLabel));
                NotifyPropertyChanged("GoalPercentage");
            }
        }

        public int _goalRate;
        [JsonInclude]
        public int GoalRate
        {
            get { return _goalRate; }
            set
            {
                _goalRate = value;
                NotifyPropertyChanged(nameof(GoalRateLabel));
                
            }
        }

        public double _goalPercentage;
        public double GoalPercentage
        {
            get { return _goalPercentage; }
            set
            {
                _goalPercentage = value;
                NotifyPropertyChanged("GoalPercentage");
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

        public string GoalRateLabel
        {
            get
            {
                return _goalRate + " lbs";
            }
        }
        public double _progressMonth;
        public double _progress3Months;
        public double _progress6Months;
        public double ProgressMonth
        {
            get
            {
                return _progressMonth;
            }
            set
            {
                _progressMonth = value;
                NotifyPropertyChanged(nameof(ProgressMonthLabel));
            }
        }
        public double Progress3Months { 
            get
            {
                return _progress3Months;
            }
            set
            {
                _progress3Months = value;
                NotifyPropertyChanged(nameof(Progress3MonthLabel));
            }
        }

        public double Progress6Months
        {
            get
            {
                return _progress6Months;
            }
            set
            {
                _progress6Months = value;
                NotifyPropertyChanged(nameof(Progress6MonthLabel));
            }
        }

        public string ProgressMonthLabel { 
            get
            {
                return _progressMonth + " lbs";
            }
        }
        public string Progress3MonthLabel
        {
            get
            {
                return _progress3Months + " lbs";
            }
        }
        public string Progress6MonthLabel
        {
            get
            {
                return _progress6Months + " lbs";
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
