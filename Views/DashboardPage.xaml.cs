using Health_Track.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            this.InitializeComponent();
            
            this.DataContext = App.Current.Services.GetRequiredService<WeightRecordViewModel>().Profile;
            //CalculatePercentage();
        }

        public void CalculatePercentage()
        {
            var profile = App.Current.Services.GetRequiredService<WeightRecordViewModel>().Profile;
            var currentWeight = profile.CurrentWeight;
            var goalWeight = profile.GoalWeight;
            var amountToLose = profile.StartingWeight - profile.GoalWeight;
            var percentage = profile.TotalLost / amountToLose;
            ProgressBar.Value = percentage * 100;
        }
    }
}
