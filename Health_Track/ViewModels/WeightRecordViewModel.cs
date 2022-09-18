using Health_Track.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

        public WeightRecordViewModel()
        {
            WeightRecords = new ObservableCollection<WeightRecord>();
            Profile = new Profile();
        }

        public async Task InitAsync()
        {
            // Read the WeightRecords.json
            await ReadFileFromSystem(true);
            // read the profile.json
            await ReadFileFromSystem(false);
            // Are there records?
            if (!WeightRecords.Any()) return;

            // Sort the collection that was read if there are any elements

            await SortWeightRecords();

        }

        private async Task SortWeightRecords()
        {
            var sortableCollection = new List<WeightRecord>(WeightRecords);
            sortableCollection = WeightRecords.OrderByDescending(weight => weight.Date).ToList();
            for (int i = 0; i < sortableCollection.Count; i++)
            {
                WeightRecords.Move(WeightRecords.IndexOf(sortableCollection[i]), i);
                await SerializeWeightRecordsAsync();
            }
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
                    // FIXME
                    // Avoid multiple rewrites given new AddWeightRecord Feature
                    if (weight == null) return;
                    WeightRecords.Insert(0, weight);
                    //await AddWeightRecord(weight);
                }
                Profile.StartingWeight = weights[weights.Length - 1].Weight;
                Profile.CurrentWeight = WeightRecords[0].Weight;

            }
            else
            {
                var profile = JsonSerializer.Deserialize<Profile>(json, options);
                if (profile == null) return;
                // Set the variables manually as they don't fire when deserialize for Label variables
                // TODO: FIX?
                Profile.Weight7Days = profile.Weight7Days;
                Profile.Weight30Days = profile.Weight30Days;
                Profile.WeightLastYear = profile.WeightLastYear;
                Profile.CurrentWeight = profile.CurrentWeight;
                Profile.TotalLost = profile.TotalLost;
                Profile.AverageWeight = profile.AverageWeight;
                Profile.GoalWeight = profile.GoalWeight;
                Profile.Name = profile.Name;
                Profile.TotalLost = profile.TotalLost;
                Profile.GoalRate = profile.GoalRate;
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
                // CHECK IF WEIGHT_RECORD JSON IS THERE.
                if (!File.Exists(storageFolder.Path + "\\weight_records.json")) return;
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

        public async Task AddWeightRecord(WeightRecord newWeightRecord, XamlRoot context)
        {
            if (newWeightRecord == null) return;

            // Make sure you don't add a date that is already there.
            // will throw null if there is no value, MUST check.
            WeightRecord searchedRecord = WeightRecords.FirstOrDefault(
                weight => weight.Date.Day == newWeightRecord.Date.Day
            && weight.Date.Month == newWeightRecord.Date.Month
            && weight.Date.Year == newWeightRecord.Date.Year);

            // There is a weight record for that day
            if (searchedRecord != null)
            {
                // Generate dialog
                ContentDialog dialog = new ContentDialog();

                // XAMLRoot must be set in the case of a contentdialog running in a desktop app
                if (context == null) return;
                dialog.XamlRoot = context;
                dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                dialog.Title = "Overwrite Existing?";
                dialog.PrimaryButtonText = "Save";
                dialog.SecondaryButtonText = "Cancel";
                dialog.DefaultButton = ContentDialogButton.Primary;
                dialog.Content = new TextBlock { Text = newWeightRecord.DateLabel };
                var result = await dialog.ShowAsync();

                // Cancel if cancel is hit.
                if (result == ContentDialogResult.Secondary) return;

                // If Save is hit
                // Search for previous record at the same date
                // get index
                int indexSearch = WeightRecords.IndexOf(searchedRecord);
                if (indexSearch == 0)
                {
                    Profile.CurrentWeight = newWeightRecord.Weight;

                }
                else
                {
                    
                }
                WeightRecords.Remove(searchedRecord);
                WeightRecords.Insert(indexSearch, newWeightRecord);

                // Calculate new statistics
                GenerateDashboard();
                // begin to save into JSON
                await SerializeWeightRecordsAsync();
                await SerializeProfileAsync();
                return;
            }
            else
            {
                // Check if its the latest date
                // for each loop to find exact spot 
                // TODO: Use LINQ?
                int looper = 0;
                foreach (WeightRecord record in WeightRecords)
                {
                    // if <0 added record is earlier than looped date
                    // if =0 added record is the same as the looped date (shouldn't happen)
                    // if >0 added record is later than looped date
                    int compareDate = newWeightRecord.Date.CompareTo(record.Date);

                    if (looper == WeightRecords.Count - 1)
                    {
                        // is at the end of the list
                        WeightRecords.Add(newWeightRecord);
                    }
                    else if (compareDate > 0)
                    {
                        WeightRecords.Insert(looper, newWeightRecord);
                        // Calculate new statistics
                        GenerateDashboard();
                        // begin to save into JSON
                        await SerializeWeightRecordsAsync();
                        await SerializeProfileAsync();
                        break;
                    }
                    looper++;
                }
            }

            
        }

        private void GenerateDashboard()
        {
            GetWeightMonthAgo();
            GetWeightWeekAgo();
            GetWeightYearAgo();
            GetTotalWeightLoss();
        }

        public async Task UpdateWeightRecord(WeightRecord newWeightRecord, WeightRecord selectedRecord)
        {
            var oldRecords = WeightRecords.FirstOrDefault(weight => weight.Weight == selectedRecord.Weight);
            oldRecords.Weight = newWeightRecord.Weight;
            oldRecords.Date = newWeightRecord.Date;
            GenerateDashboard();

            await SerializeWeightRecordsAsync();
            await SerializeProfileAsync();
        }

        public async Task DeleteWeightRecord(int index)
        {
            // if index is not valid or WeightRecords contains no elements.
            if (index < 0) return;

            WeightRecords.RemoveAt(index);

            if (WeightRecords.Count == 0) return;
            
            // If first in list -- make current weight next in line
            if (index == 0 )
            {
                Profile.CurrentWeight = WeightRecords[0].Weight;
            }

            await SerializeWeightRecordsAsync();
            await SerializeProfileAsync();
            GenerateDashboard();
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

            if (WeightRecords == null || WeightRecords.Count == 0 || items == null) return;

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

        public void ResetProgress()
        {
            var currentWeight = Profile.CurrentWeight;
            var goalWeight = Profile.GoalWeight;
            var amountToLose = Profile.StartingWeight - Profile.GoalWeight;
            var percentage = Profile.GoalWeight / Profile.CurrentWeight;
            Profile.GoalPercentage = percentage * 100;
            //NotifyPropertyChanged("GoalPercentage");
        }
    }
}
