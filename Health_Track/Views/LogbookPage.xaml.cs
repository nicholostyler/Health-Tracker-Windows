using Health_Track.Models;
using Health_Track.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Uwp.UI.Controls;
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
    public sealed partial class LogbookPage : Page
    {
        private WeightRecordViewModel viewModel;
        public LogbookPage()
        {
            this.InitializeComponent();
            // Use this to prevent infinite page creation.
            this.NavigationCacheMode = NavigationCacheMode.Required;
            viewModel = App.Current.Services.GetService<WeightRecordViewModel>();
            //viewModel = new WeightRecordViewModel();
            //viewModel = App.Current.Services.GetService<WeightRecordViewModel>();
            //viewModel.AddWeightRecord(new WeightRecord { Date = DateTime.Now, Weight = 200.0 });
            this.DataContext = App.Current.Services.GetRequiredService<WeightRecordViewModel>();

            // set listview to first item in WeightRecords
            LogbookListView.SelectedIndex = 0;
        }

        public WeightRecordViewModel ViewModel => (WeightRecordViewModel)DataContext;

        private async void ABBNewWeightRecord_Click(object sender, RoutedEventArgs e)
        {
            // Call Content Dialog
            ContentDialog dialog = new ContentDialog();

            dialog.XamlRoot = this.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = "New Weight Record";
            dialog.PrimaryButtonText = "Add";
            dialog.SecondaryButtonText = "Cancel";
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content = new NewWeightRecordDialogContent();

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var dialogPage = dialog.Content as NewWeightRecordDialogContent;
                DateTimeOffset newDate = dialogPage.NewDate;
                double newWeight = dialogPage.NewWeight;
                viewModel.AddWeightRecord(new Models.WeightRecord { Weight = newWeight, Date = newDate });
            }
        }

        private void btnEditCancel_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            StackPanel buttonStack = (sender as Button).Parent as StackPanel;
            StackPanel detailsStack = buttonStack.Parent as StackPanel;
            StackPanel weightStack = detailsStack.Children[0] as StackPanel;
            string newWeight = (weightStack.Children[1] as TextBox).Text;

            // NewDate
            StackPanel dateStack = detailsStack.Children[1] as StackPanel;
            DateTimeOffset datePicker = (dateStack.Children[1] as DatePicker).Date;
            var newWeight2 = new Models.WeightRecord { Date = datePicker, Weight = Double.Parse(newWeight) };
            var newSelectedItem = LogbookListView.SelectedItem as WeightRecord;
            //viewModel.UpdateWeightRecord(newWeight2, newSelectedItem);
            App.Current.Services.GetService<WeightRecordViewModel>().UpdateWeightRecord(newWeight2, newSelectedItem);
            
        }
    }
}
