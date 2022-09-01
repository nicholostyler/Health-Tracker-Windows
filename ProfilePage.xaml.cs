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
            // populate collection
            lossRate.Add(1);
            lossRate.Add(2);
            lossRate.Add(3);
            lossRate.Add(4);
            lossRate.Add(5);
            lossRate.Add(6);
            lossRate.Add(7);
            lossRate.Add(8);

        }
    }
}
