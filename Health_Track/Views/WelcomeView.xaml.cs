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
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace Health_Track.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WelcomeView : Page
    {
        double weight = 0.0;
        double currentWeight = 0.0;
        string name = string.Empty;
        DateTimeOffset? date;
        ObservableCollection<int> lossRate = new ObservableCollection<int>();

        public WelcomeView()
        {
            this.InitializeComponent();
            lossRate.Add(1);
            lossRate.Add(2);
            lossRate.Add(3);
            lossRate.Add(4);
        }

        private async void FinishBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!pickerDate.SelectedDate.HasValue) return;
            if (!FieldVerifier.ValidateName(txtName.Text)) return;
            if (!FieldVerifier.ValidateWeight(txtGoalWeight.Text)) return;
            if (!FieldVerifier.ValidateWeight(txtCurrentWeight.Text)) return;
            if (!FieldVerifier.ValidateDate((DateTimeOffset)pickerDate.SelectedDate)) return;

            Profile newProfile = new Profile();
            //newProfile.Name = name;
            //newProfile.GoalDate = date.Value;
            //newProfile.GoalRate = lossRate[cbRate.SelectedIndex];
            // Call the view model to set Profile and serialize it;
            App.Current.Services.GetRequiredService<WeightRecordViewModel>().Profile.Name = name;
            App.Current.Services.GetRequiredService<WeightRecordViewModel>().Profile.GoalDate = date.Value;
            App.Current.Services.GetRequiredService<WeightRecordViewModel>().Profile.GoalRate = lossRate[cbRate.SelectedIndex];
            App.Current.Services.GetRequiredService<WeightRecordViewModel>().Profile.GoalWeight = weight;
            App.Current.Services.GetRequiredService<WeightRecordViewModel>().Profile.CurrentWeight = currentWeight;

            // add weight to viewmodel
            await App.Current.Services.GetRequiredService<WeightRecordViewModel>().AddWeightRecord(new WeightRecord { Date = DateTimeOffset.Now, Weight = currentWeight }, this.XamlRoot);
            await App.Current.Services.GetRequiredService<WeightRecordViewModel>().SerializeProfileAsync();

            // Save App setting to not show this again
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["isStarter"] = "false";



            // OK to navigate to MainPage
            this.Frame.Navigate(typeof(MainPage));
        }

        
    }
}
