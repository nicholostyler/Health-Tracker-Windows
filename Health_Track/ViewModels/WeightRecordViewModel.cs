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
using Windows.UI;

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
            await ReadFileFromSystem(true);
            await ReadFileFromSystem(false);
            var sortableCollection = new List<WeightRecord>(WeightRecords);
            sortableCollection.Sort((a, b) => a.Weight.CompareTo(b.Weight));
            for (int i = 0; i < sortableCollection.Count; i++)
            {
                WeightRecords.Move(WeightRecords.IndexOf(sortableCollection[i]), i);
                await SerializeWeightRecordsAsync();
            }
            //GetWeightWeekAgo();
            //GetWeightMonthAgo();
            //GetWeightYearAgo();
            //GetTotalWeightLoss();
            //await SerializeProfileAsync();
        }

        public async Task SerializeWeightRecordsAsync()
        {
            var options = new JsonSerializerOptions
            {
                IncludeFields = true,
                WriteIndented = true,
            };

            string jsonString = JsonSerializer.Serialize(WeightRecords, options);
            await WriteFileToSystem(jsonString, true);
        }

        public async Task SerializeProfileAsync()
        {
            var options = new JsonSerializerOptions
            {
                IncludeFields = true,
                WriteIndented = true,
            };

            string jsonString = JsonSerializer.Serialize(Profile, options);
            await WriteFileToSystem(jsonString, false);
        }

        public async Task DeserializeJSON(string json, bool isRecords)
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

            if (isRecords)
            {
                var weights = JsonSerializer.Deserialize<WeightRecord[]>(json, options);

                // iterate through weights and add them to WeightRecords
                foreach (var weight in weights)
                {
                    await AddWeightRecord(weight);
                }
                Profile.StartingWeight = weights[weights.Length - 1].Weight;

            }
            else
            {
                var profile = JsonSerializer.Deserialize<Profile>(json, options);
                // Set the variables manually as they don't fire when deserialize for Label variables
                // TODO: FIX?
                Profile.Weight7Days = profile.Weight7Days;
                Profile.Weight30Days = profile.Weight30Days;
                Profile.WeightLastYear = profile.WeightLastYear;
                Profile.CurrentWeight = profile.CurrentWeight;
                Profile.TotalLost = profile.TotalLost;
                Profile.AverageWeight = profile.AverageWeight;
                Profile = profile;
            }


        }

        public async Task WriteFileToSystem(string content, bool isRecords)
        {
            // Create file; stop if exists
            // is backed up to the cloud.
            StorageFolder storageFolder =
                ApplicationData.Current.LocalFolder;
            StorageFile jsonFile;

            if (isRecords)
            {
                jsonFile =
                await storageFolder.CreateFileAsync("weight_records.json",
                    CreationCollisionOption.ReplaceExisting);
            }
            else
            {
                jsonFile =
                await storageFolder.CreateFileAsync("profile.json",
                    CreationCollisionOption.ReplaceExisting);
            }
            

            // write to file
            await FileIO.WriteTextAsync(jsonFile, content);
            var temp = String.Format("File is located at {0}", jsonFile.Path.ToString());
        }

        public async Task ReadFileFromSystem(bool isRecords)
        {
            StorageFolder storageFolder =
                ApplicationData.Current.LocalFolder;
            StorageFile jsonFile;
            if (isRecords)
            {
                 jsonFile =
                await storageFolder.GetFileAsync("weight_records.json");
                // reading text from file
                try
                {
                    string json = await FileIO.ReadTextAsync(jsonFile);
                    await DeserializeJSON(json, true);
                }
                catch (Exception e)
                {
                    Debug.Write(e.Message);
                }
            }
            else
            {
                jsonFile =
                    await storageFolder.GetFileAsync("profile.json");
                // reading text from file
                try
                {
                    string json = await FileIO.ReadTextAsync(jsonFile);
                    await DeserializeJSON(json, false);
                }
                catch (Exception e)
                {
                    Debug.Write(e.Message);
                }
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

        public async Task AddWeightRecord(WeightRecord newWeightRecord)
        {
            if (newWeightRecord == null) return;
            if (WeightRecords.Count == 0)
            {
                Profile.CurrentWeight = newWeightRecord.Weight;
            }

            WeightRecords.Insert(0, newWeightRecord);

            // begin to save into JSON
            await SerializeWeightRecordsAsync();
        }

        public void UpdateWeightRecord(WeightRecord newWeightRecord, WeightRecord selectedRecord)
        {
            var oldRecords = WeightRecords.FirstOrDefault(weight => weight.Weight == selectedRecord.Weight);
            oldRecords.Weight = newWeightRecord.Weight;
            oldRecords.Date = newWeightRecord.Date;
        }

        public void GetWeightWeekAgo()
        {
            DateTimeOffset weekAgo = new DateTime();
            DateTimeOffset today = DateTime.Now;
            weekAgo = today.DateTime.AddDays(-7);

            // get all dates in the past 7 days
            var items = WeightRecords
     .OrderByDescending(a => a.Date)
     .Where(m => m.Date <= today && m.Date > weekAgo)
     .Concat(WeightRecords.Where(d => d.Date > weekAgo).TakeLast(1));

            if (WeightRecords == null || WeightRecords.Count == 0) return;
            var largestWeight = items.Max(weight => weight.Weight);
            var lostTotal = largestWeight - Profile.CurrentWeight;
            Profile.Weight7Days = lostTotal;   
        }

        public void GetWeightMonthAgo()
        {
            DateTimeOffset monthAgo = new DateTimeOffset();
            DateTimeOffset today = DateTimeOffset.Now;
            monthAgo = today.DateTime.AddMonths(-1);


            // get all dates in the past 7 days
            var items = WeightRecords
     .OrderByDescending(a => a.Date)
     .Where(m => m.Date <= today && m.Date > monthAgo)
     .Concat(WeightRecords.Where(d => d.Date > monthAgo).TakeLast(1));

            if (WeightRecords == null || WeightRecords.Count == 0) return;

            var largestWeight = items.Max(weight => weight.Weight);
            var lostTotal = largestWeight - Profile.CurrentWeight;
            Profile.Weight30Days = lostTotal;
        }
        public void GetWeightYearAgo()
        {
            DateTimeOffset yearAgo = new DateTimeOffset();
            DateTimeOffset today = DateTimeOffset.Now;
            yearAgo = today.DateTime.AddYears(-1);

            // get all dates in the past 7 days
            var items = WeightRecords
     .OrderByDescending(a => a.Date)
     .Where(m => m.Date <= today && m.Date > yearAgo)
     .Concat(WeightRecords.Where(d => d.Date > yearAgo).TakeLast(1));

            if (WeightRecords == null || WeightRecords.Count == 0) return;

            var largestWeight = items.Max(weight => weight.Weight);
            var lostTotal = largestWeight - Profile.CurrentWeight;

            Profile.WeightLastYear = lostTotal;
        }

        public void GetTotalWeightLoss()
        {
            if (WeightRecords == null || WeightRecords.Count == 0) return;

            var largestWeight = WeightRecords.Max(weight => weight.Weight);
            var lostTotal = largestWeight - Profile.CurrentWeight;
            var averageWeight = WeightRecords.Sum(weight => weight.Weight) / WeightRecords.Count;
            Profile.AverageWeight = averageWeight;
            Profile.TotalLost = lostTotal;
        }

        public void GetAverageWeightLoss()
        {

        }
    }
}
