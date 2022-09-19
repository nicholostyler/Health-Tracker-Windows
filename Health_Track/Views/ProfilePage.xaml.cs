using Health_Track.Helpers;
using Health_Track.Models;
using Health_Track.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Health_Track
{
    
    public sealed partial class ProfilePage : Page
    {
        ObservableCollection<int> lossRate = new ObservableCollection<int>();
        public ProfilePage()
        {
            this.InitializeComponent();
            this.DataContext = App.Current.Services.GetRequiredService<WeightRecordViewModel>().Profile;
            this.NavigationCacheMode = NavigationCacheMode.Required;
            // populate collection
            lossRate.Add(1);
            lossRate.Add(2);
            lossRate.Add(3);
            lossRate.Add(4);
            InfoBar.IsOpen = false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.DataContext = App.Current.Services.GetRequiredService<WeightRecordViewModel>().Profile;

        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Determine if all the profiles values are verified
            string name = txtName.Text;
            string weight = txtGoalWeight.Text;
            if (!datePicker.SelectedDate.HasValue) return;
            DateTimeOffset? date = datePicker.SelectedDate;
            Profile updatedProfile = new Profile();

            if (!FieldVerifier.ValidateName(name))
            {
                // name is not valid
                InfoBar.IsOpen = true;
                InfoBar.Message = "The name field is invalid, can only contain letters.";
                return;
            }

            App.Current.Services.GetRequiredService<WeightRecordViewModel>().Profile.Name = name;

            if (!FieldVerifier.ValidateWeight(weight))
            {
                // Weight is not valid
                InfoBar.IsOpen = true;
                InfoBar.Message = "The weight field is invalid, can only contains numbers.";
                return;
            }

            App.Current.Services.GetRequiredService<WeightRecordViewModel>().Profile.GoalWeight = Double.Parse(weight);

            if (!FieldVerifier.ValidateDate(date.Value))
            {
                InfoBar.IsOpen = true;
                InfoBar.Message = "The date field is invalid, must be a date before today.";
                return;
            }


            InfoBar.IsOpen = true;
            InfoBar.Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success;
            InfoBar.Title = "Success";
            InfoBar.Message = "Profile updated and saved.";

            // BUG
            if (date.Value.Year != 1600)
            {
                App.Current.Services.GetRequiredService<WeightRecordViewModel>().Profile.GoalDate = date.Value;
            }

            await App.Current.Services.GetRequiredService<WeightRecordViewModel>().SerializeProfileAsync();
        }
    }
}
