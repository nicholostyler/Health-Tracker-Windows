using Health_Track.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health_Track.ViewModels
{
    public class WeightRecordViewModel : INotifyPropertyChanged
    {
        //TODO: Put Data into a seperate class
        public ObservableCollection<WeightRecord> WeightRecords { get; set; }
        public Profile Profile;

        public WeightRecord _selectedWeightRecord;
        public WeightRecord SelectedWeightRecord
        {
            get { return _selectedWeightRecord; }
            set
            {
                _selectedWeightRecord = value;
                NotifyPropertyChanged("SelectedWeightRecord");
            }
        }


        public WeightRecordViewModel()
        {
            WeightRecords = new ObservableCollection<WeightRecord>();
            Profile = new Profile();
            //week ago dates
            var weekAgo1 = DateTimeOffset.Now.AddDays(-4);
            var weekAgo2 = DateTimeOffset.Now.AddDays(-3);
            // Month ago dates
            var monthAgo1 = DateTimeOffset.Now.AddDays(-10);
            var monthAgo2 = DateTimeOffset.Now.AddDays(-11);
            // Year ago dates
            var yearAgo1 = DateTimeOffset.Now.AddDays(-90);
            var yearAgo2 = DateTimeOffset.Now.AddDays(-120);

            AddWeightRecord(new WeightRecord { Date = yearAgo1, Weight = 230.0 });
            AddWeightRecord(new WeightRecord { Date = yearAgo2, Weight = 234.0 });

            AddWeightRecord(new WeightRecord { Date = monthAgo1, Weight = 214.0 });
            AddWeightRecord(new WeightRecord { Date = monthAgo2, Weight = 215.0 });

            AddWeightRecord(new WeightRecord { Date = weekAgo2, Weight = 200.0 });
            AddWeightRecord(new WeightRecord { Date = weekAgo1, Weight = 205.0 });

            GetWeightWeekAgo();
            GetWeightMonthAgo();
            GetWeightYearAgo();
            GetTotalWeightLoss();
            Profile.GoalWeight = 180.0;
            Profile.GoalDate = DateTimeOffset.Now.AddYears(1);
            Profile.Name = "Nicholos Tyler";
            Profile.GoalRate = 1;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public void AddWeightRecord(WeightRecord newWeightRecord)
        {
            if (newWeightRecord == null) return;
            if (WeightRecords.Count == 0) Profile.StartingWeight = newWeightRecord.Weight;
            WeightRecords.Add(newWeightRecord);
            Profile.CurrentWeight = newWeightRecord.Weight;
        }

        public void UpdateWeightRecord(WeightRecord newWeightRecord, WeightRecord selectedRecord)
        {
            var oldRecords = WeightRecords.FirstOrDefault(weight => weight.Weight == selectedRecord.Weight);
            oldRecords.Weight = newWeightRecord.Weight;
            oldRecords.Date = newWeightRecord.Date;
        }

        public void GetWeightWeekAgo()
        {
            DateTimeOffset weekAgo = new DateTimeOffset();
            DateTimeOffset today = DateTimeOffset.Now;
            weekAgo = today.DateTime.AddDays(-7);

            var largestWeight = 0.0;
            var currentWeight = 0.0;
            var lostTotal = 0.0;
            foreach (var weight in WeightRecords)
            {
                TimeSpan diff = weight.Date - weekAgo;
                currentWeight = weight.Weight;
                if (diff.Days > 7 || diff.Days < 0) continue;
                if (currentWeight == Profile.CurrentWeight) continue;

                if (largestWeight == 0.0)
                {
                    currentWeight = weight.Weight;
                    largestWeight = weight.Weight;
                }


                if (currentWeight > largestWeight)
                {
                    largestWeight = currentWeight;
                }

            }
            lostTotal = (currentWeight - largestWeight);
            Profile.Weight7Days = lostTotal;
        }

        public void GetWeightMonthAgo()
        {
            DateTimeOffset monthAgo = new DateTimeOffset();
            DateTimeOffset today = DateTimeOffset.Now;
            monthAgo = today.DateTime.AddMonths(-1);

            var largestWeight = 0.0;
            var currentWeight = 0.0;
            var lostTotal = 0.0;
            foreach (var weight in WeightRecords)
            {
                TimeSpan diff = weight.Date - monthAgo;
                currentWeight = weight.Weight;
                if (diff.Days > 30 || diff.Days < 0) continue;
                if (currentWeight == Profile.CurrentWeight) continue;

                if (largestWeight == 0.0)
                {
                    currentWeight = weight.Weight;
                    largestWeight = weight.Weight;
                }


                if (currentWeight > largestWeight)
                {
                    largestWeight = currentWeight;
                }

            }
            lostTotal = (largestWeight - currentWeight);
            Profile.Weight30Days = lostTotal;
        }
        public void GetWeightYearAgo()
        {
            DateTimeOffset yearAgo = new DateTimeOffset();
            DateTimeOffset today = DateTimeOffset.Now;
            yearAgo = today.DateTime.AddYears(-1);

            var largestWeight = 0.0;
            var currentWeight = 0.0;
            var lostTotal = 0.0;
            foreach (var weight in WeightRecords)
            {
                TimeSpan diff = weight.Date - yearAgo;
                currentWeight = weight.Weight;
                if (diff.Days > 365 || diff.Days < 0) continue;
                if (currentWeight == Profile.CurrentWeight) continue;

                if (largestWeight == 0.0)
                {
                    currentWeight = weight.Weight;
                    largestWeight = weight.Weight;
                }


                if (currentWeight > largestWeight)
                {
                    largestWeight = currentWeight;
                }

            }
            lostTotal = (largestWeight - currentWeight);
            Profile.WeightLastYear = lostTotal;
        }

        public void GetTotalWeightLoss()
        {
            var largestWeight = 0.0;
            var currentWeight = 0.0;
            var lostTotal = 0.0;

            foreach (var weight in WeightRecords)
            {
                currentWeight = weight.Weight;
                if (currentWeight == Profile.CurrentWeight) continue;

                if (largestWeight == 0.0)
                {
                    currentWeight = weight.Weight;
                    largestWeight = weight.Weight;
                }


                if (currentWeight > largestWeight)
                {
                    largestWeight = currentWeight;
                }

            }
            lostTotal = (largestWeight - currentWeight);
            Profile.TotalLost = lostTotal;
        
        }
    }
}
