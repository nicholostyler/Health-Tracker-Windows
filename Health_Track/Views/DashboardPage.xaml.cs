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
        
        public string ProgressMonthDateLabel { get; set; }
        public string Progress3MonthDateLabel { get; set; }
        public string Progress6MonthDateLabel { get; set; }

        public DashboardPage()
        {
            PopulateProgressCard();

            this.InitializeComponent();
            
            this.DataContext = App.Current.Services.GetRequiredService<WeightRecordViewModel>();
            MainDashView.DataContext = App.Current.Services.GetRequiredService<WeightRecordViewModel>();
            MainScrollView.DataContext = App.Current.Services.GetRequiredService<WeightRecordViewModel>().Profile;
            this.NavigationCacheMode = NavigationCacheMode.Required;
       }

        private void ResetViews()
        {
            if (App.Current.Services.GetRequiredService<WeightRecordViewModel>().WeightRecords.Count == 0)
            {
                NoWeightStack.Visibility = Visibility.Visible;
                MainDashView.Visibility = Visibility.Collapsed;
            }
            else
            {
                NoWeightStack.Visibility = Visibility.Collapsed;
                MainDashView.Visibility = Visibility.Visible;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            App.Current.Services.GetRequiredService<WeightRecordViewModel>().ResetProgress();
            this.DataContext = App.Current.Services.GetRequiredService<WeightRecordViewModel>();
            MainDashView.DataContext = App.Current.Services.GetRequiredService<WeightRecordViewModel>();
            MainScrollView.DataContext = App.Current.Services.GetRequiredService<WeightRecordViewModel>().Profile;
            //ResetViews();
            
        }

        public void PopulateProgressCard()
        {
            // date month from now
            DateTimeOffset dateMonth = DateTimeOffset.Now.AddMonths(1);
            DateTimeOffset date3Months = DateTimeOffset.Now.AddMonths(3);
            DateTimeOffset date6Months = DateTimeOffset.Now.AddMonths(6);

            
            ProgressMonthDateLabel = dateMonth.ToString("yyyy-MM-dd");
            Progress3MonthDateLabel = date3Months.ToString("yyyy-MM-dd");
            Progress6MonthDateLabel = date6Months.ToString("yyyy-MM-dd");
        }

        private async void btnAddWeight_Click(object sender, RoutedEventArgs e)
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
                await App.Current.Services.GetService<WeightRecordViewModel>().AddWeightRecord(new Models.WeightRecord { Weight = newWeight, Date = newDate }, this.XamlRoot);
                ResetViews();
            }
        }
    }
}
