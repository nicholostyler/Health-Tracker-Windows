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
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        private async void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();
            StackPanel contentMain = new StackPanel();
            TextBlock dialogDescription = new TextBlock();
            HyperlinkButton licenseHyperlink = new HyperlinkButton();
            licenseHyperlink.Content = "MIT License";
            licenseHyperlink.NavigateUri = new Uri("https://opensource.org/licenses/MIT");
            dialogDescription.Text = "Health Tracker 1.0.2";
            contentMain.Children.Add(dialogDescription);
            contentMain.Children.Add(licenseHyperlink);

            // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
            dialog.XamlRoot = this.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = "About Health Tracker";
            dialog.CloseButtonText = "Ok";
            dialog.DefaultButton = ContentDialogButton.Close;
            dialog.Content = contentMain;

            var result = await dialog.ShowAsync();

        }
    }
}
