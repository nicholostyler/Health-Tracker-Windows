using Health_Track.Helpers;
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
    public sealed partial class NewWeightRecordDialogContent : Page
    {
        public double NewWeight { get; set; }
        public DateTimeOffset NewDate { get; set; }

        public NewWeightRecordDialogContent()
        {
            
            this.InitializeComponent();
            NewDate = DateTime.Now;
            datePicker.SelectedDate = DateTime.Now;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Double tryNewWeight = 0.0;
            if (Double.TryParse(txtNewWeight.Text, out tryNewWeight))
            {
                NewWeight = tryNewWeight;
                NewWeight = Double.Parse(txtNewWeight.Text);
            }
            else
            {

            }
        }

        private void datePicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            if (!FieldVerifier.ValidateDate(datePicker.SelectedDate.Value))
            {
                txtDate.Text = "Date: Invalid Date";
            }
            else
            {
                txtDate.Text = "Date";
                NewDate = datePicker.Date;
            }
        }

        private void txtNewWeight_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            // Cancel the user text that is not a number.
            args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
        }
    }
}
