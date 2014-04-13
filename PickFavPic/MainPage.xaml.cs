using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PickFavPic.Resources;
using Windows.Devices.Geolocation;
using System.Device.Location;
using System.Net.Http;
using Newtonsoft.Json;
using System.Windows.Media.Imaging;

namespace PickFavPic
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;

            // Sample code to localize the ApplicationBar
            BuildLocalizedApplicationBar();
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBarIconButton SearchButton = new ApplicationBarIconButton();
            SearchButton.IconUri = new Uri("/Assets/search.png", UriKind.Relative);
            SearchButton.Text = "Search";
            SearchButton.Click += SearchButton_Click;
            ApplicationBar.Buttons.Add(SearchButton);

            //Create a new menu item with localized string from App Resources
           // ApplicationBarMenuItem appbarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
            //ApplicationBar.MenuItems.Add(appbarMenuItem);

        }

      

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            SystemTray.ProgressIndicator = new ProgressIndicator();

            updateMap();
        }

        private static void SetProgressIndicator(bool isVisible) 
        {

            SystemTray.ProgressIndicator.IsIndeterminate = isVisible;
            SystemTray.ProgressIndicator.IsVisible = isVisible;
        }
        private async void updateMap()
        {
            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;
            //Acquiring position on geolocation abso

            SetProgressIndicator(true);
            SystemTray.ProgressIndicator.Text = "Retrieving GPS Location";

            try
            {
                Geoposition geoposition = await geolocator.GetGeopositionAsync(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(30));

                SystemTray.ProgressIndicator.Text = "Acquired";
                GeoCoordinate GPSCoordinator = new GeoCoordinate(geoposition.Coordinate.Latitude, geoposition.Coordinate.Longitude);
                myMap.Center = GPSCoordinator;
                myMap.ZoomLevel = 15;

                SetProgressIndicator(false);
            }
            catch (UnauthorizedAccessException) {
                MessageBox.Show("Location setting is disabled");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

          //  ResultTextBlock.Text = string.Format("{0}-{1}", myMap.Center.Latitude, myMap.Center.Longitude);


        }

        void SearchButton_Click(object sender, EventArgs e)
        {
            string topic = HttpUtility.UrlEncode(SearchTextBox.Text);
            string searchPage = string.Format("/PhotoResults.xaml?latitude={0}&longitude={1}&topic={2}",
                myMap.Center.Latitude, myMap.Center.Longitude,topic);
            NavigationService.Navigate(new Uri(searchPage, UriKind.RelativeOrAbsolute));
        }


    }
 
}