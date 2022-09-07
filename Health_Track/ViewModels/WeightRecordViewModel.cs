using Health_Track.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;

namespace Health_Track.ViewModels
{
    public class WeightRecordViewModel : INotifyPropertyChanged
    {
        //TODO: Put Data into a seperate class
        public ObservableCollection<WeightRecord> WeightRecords { get; set; }
        private Profile _profile;
        public Profile Profile
        {
            get
            {
                return _profile;
            }
            set
            {
                _profile = value;
                NotifyPropertyChanged("Profile");
            }
        }

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
            /*
            WeightRecords.Add(new WeightRecord { Weight = 245.0, Date = yearAgo1 });
            WeightRecords.Add(new WeightRecord { Weight = 240.0, Date = yearAgo2 });
            WeightRecords.Add(new WeightRecord { Weight = 215.0, Date = monthAgo1 });
            WeightRecords.Add(new WeightRecord { Weight = 213.0, Date = monthAgo2 });
            WeightRecords.Add(new WeightRecord { Weight = 210.0, Date = weekAgo1 });
            WeightRecords.Add(new WeightRecord { Weight = 209.0, Date = weekAgo2 });
            */
            Profile.GoalWeight = 180.0;
            Profile.GoalDate = DateTimeOffset.Now.AddYears(1);
            Profile.Name = "Nicholos Tyler";
            Profile.GoalRate = 1;
        }

        public async Task InitAsync()
        {
            //await SerializeJSONAsync();
            await ReadFileFromSystem();
            GetWeightWeekAgo();
            GetWeightMonthAgo();
            GetWeightYearAgo();
            GetTotalWeightLoss();
        }

        public async Task SerializeJSONAsync()
        {
            var options = new JsonSerializerOptions
            {
                IncludeFields = true,
                WriteIndented = true,
            };

            string jsonString = JsonSerializer.Serialize(WeightRecords, options);
            await WriteFileToSystem(jsonString);
        }

        public void DeserializeJSON(string json)
        {
            var options = new JsonSerializerOptions
            {
                IncludeFields = true,
                WriteIndented = true,
            };
            if (string.IsNullOrEmpty(json))
            {
                Console.Error.WriteLine("Json is empty!");
            }

            var weights = JsonSerializer.Deserialize<WeightRecord[]>(json, options);
            
            // iterate through weights and add them to WeightRecords
            foreach(var weight in weights)
            {
                AddWeightRecord(weight);
            }
        }

        public async Task WriteFileToSystem(string content)
        {
            // Create file; stop if exists
            // is backed up to the cloud.
            StorageFolder storageFolder =
                ApplicationData.Current.LocalFolder;
            StorageFile jsonFile =
                await storageFolder.CreateFileAsync("weight_records.json",
                    CreationCollisionOption.ReplaceExisting);

            // write to file
            await FileIO.WriteTextAsync(jsonFile, content);
            var temp = String.Format("File is located at {0}", jsonFile.Path.ToString());
        }

        public async Task ReadFileFromSystem()
        {
            StorageFolder storageFolder =
                ApplicationData.Current.LocalFolder;
            StorageFile jsonFile =
                await storageFolder.GetFileAsync("weight_records.json");
            // reading text from file
            try
            {
                string json = await FileIO.ReadTextAsync(jsonFile);
                DeserializeJSON(json);


            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
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

        public void AddWeightRecord(WeightRecord newWeightRecord)
        {
            if (newWeightRecord == null) return;
            if (WeightRecords.Count == 0)
            {
                Profile.StartingWeight = newWeightRecord.Weight;
            }

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
            var currentLargest = 0.0;
            var lostTotal = 0.0;
            foreach (var weight in WeightRecords)
            {
                TimeSpan diff = weight.Date - weekAgo;
                currentLargest = weight.Weight;
                if (diff.Days > 7 || diff.Days < 0) continue;
                if (currentLargest == Profile.CurrentWeight) continue;

                if (largestWeight == 0.0)
                {
                    currentLargest = weight.Weight;
                    largestWeight = weight.Weight;
                }


                if (currentLargest > largestWeight)
                {
                    largestWeight = currentLargest;
                }

            }
            //TODO: BUG if all dates are the same
            if (largestWeight == 0)
            {
                Profile.Weight7Days = 0;
            }
            else
            {
                lostTotal = (largestWeight - Profile.CurrentWeight);
                Profile.Weight7Days = lostTotal;
            }
            
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
