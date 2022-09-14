using Health_Track.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Health_Track.Models
{
    public class Profile : INotifyPropertyChanged
    {
        [JsonPropertyName("Name")]
        public string _name;

        [JsonIgnore]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        [JsonPropertyName("GoalDate")]
        public DateTimeOffset _goalDate;
        [JsonIgnore]
        public DateTimeOffset GoalDate
        {
            get { return _goalDate; }
            set
            {
                _goalDate = value;
                NotifyPropertyChanged("GoalDate");
            }
        }

        [JsonPropertyName("GoalWeight")]
        public double _goalWeight;
        [JsonIgnore]
        public double GoalWeight
        {
            get { return _goalWeight; }
            set
            {
                _goalWeight = value;
                NotifyPropertyChanged(nameof(TargetWeightLabel));

            }
        }

        [JsonPropertyName("CurrentWeight")]
        public double _currentWeight;
        [JsonIgnore]
        public double CurrentWeight
        {
            get { return _currentWeight; }
            set
            {
                _currentWeight = value;
                var profile = App.Current.Services.GetRequiredService<WeightRecordViewModel>().Profile;
                var weightMonth = profile._currentWeight - (4 * profile._goalRate);
                var weight3Months = profile._currentWeight - ((4 * 3) * profile._goalRate);
                var weight6Months = profile._currentWeight - ((4 * 6) * profile._goalRate);

                _progressMonth = weightMonth;
                _progress3Months = weight3Months;
                _progress6Months = weight6Months;

                NotifyPropertyChanged(nameof(CurrentWeightLabel));
                NotifyPropertyChanged(nameof(ProgressMonthLabel));
                NotifyPropertyChanged(nameof(Progress3MonthLabel));
                NotifyPropertyChanged(nameof(Progress6MonthLabel));
            }
        }
        [JsonPropertyName("Weight7Days")]
        public double _weight7Days;
        [JsonIgnore]
        public double Weight7Days
        {
            get { return _weight7Days; }
            set
            {
                _weight7Days = value;
                NotifyPropertyChanged(nameof(Last7DaysLabel));
            }
        }
        [JsonPropertyName("Weight30Days")]
        public double _weight30Days;
        [JsonIgnore]
        public double Weight30Days
        {
            get { return _weight30Days; }
            set
            {
                _weight30Days = value;
                NotifyPropertyChanged(nameof(Last30DaysLabel));
            }
        }

        [JsonPropertyName("WeightLastYear")]
        public double _weightLastYear;
        [JsonIgnore]
        public double WeightLastYear
        {
            get { return _weightLastYear; }
            set
            {
                _weightLastYear = value;
                NotifyPropertyChanged(nameof(WeightLastYear));
            }
        }
        [JsonPropertyName("StartingWeight")]
        public double _startingWeight;

        [JsonIgnore]
        public double StartingWeight
        {
            get { return _startingWeight; }
            set
            {
                _startingWeight = value;
                NotifyPropertyChanged(nameof(StartingWeightLabel));
            }
        }
        [JsonPropertyName("TotalLost")]
        public double _totalLost;
        [JsonIgnore]
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

        [JsonPropertyName("GoalRate")]
        public int _goalRate;
        [JsonIgnore]
        public int GoalRate
        {
            get { return _goalRate; }
            set
            {
                _goalRate = value;
                NotifyPropertyChanged(nameof(GoalRateLabel));
                
            }
        }

        [JsonPropertyName("AverageRate")]
        public double _averageRate;
        public double AverageWeight
        {
            get { return _averageRate; }
            set
            {
                _averageRate = value;
                NotifyPropertyChanged("AverageWeight");
            }
        }

        [JsonIgnore]
        public double _goalPercentage;
        [JsonIgnore]
        public double GoalPercentage
        {
            get { return _goalPercentage; }
            set
            {
                _goalPercentage = value;
                NotifyPropertyChanged("GoalPercentage");
            }
        }
        [JsonIgnore]
        public string CurrentWeightLabel
        {
            get { return _currentWeight + " lbs"; }
        }
        [JsonIgnore]
        public string Last7DaysLabel
        {
            
            get {
                if (_weight7Days > 0)
                    return _weight7Days + " lost";
                else
                    return _weight7Days + " gained";
            }
        }
        [JsonIgnore]
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
        [JsonIgnore]
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
        [JsonIgnore]
        public string TargetWeightLabel
        {
            get
            {
                return _goalWeight + " lbs";
            }
        }
        [JsonIgnore]
        public string StartingWeightLabel
        {
            get
            {
                return _startingWeight + " lbs";
            }
        }
        [JsonIgnore]
        public string TotalLostLabel
        {
            get
            {
                return _totalLost + " lbs";
            }
        }
        [JsonIgnore]
        public string GoalRateLabel
        {
            get
            {
                return _goalRate + " lbs";
            }
        }
        [JsonIgnore]
        public double _progressMonth;
        [JsonIgnore]
        public double _progress3Months;
        [JsonIgnore]
        public double _progress6Months;
        [JsonIgnore]
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
        [JsonIgnore]
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
        [JsonIgnore]
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
        [JsonIgnore]
        public string ProgressMonthLabel { 
            get
            {
                return _progressMonth + " lbs";
            }
        }
        [JsonIgnore]
        public string Progress3MonthLabel
        {
            get
            {
                return _progress3Months + " lbs";
            }
        }
        [JsonIgnore]
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
