using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows;
using System.Windows.Media.Animation;
using PickFavPic.Resources;

namespace PickFavPic
{
    public partial class PhotoResults : PhoneApplicationPage
    {
        public Uri SmallImage { get; set; }
        public Uri BigImage { get; set; }
        public static string result;

        private double lat;
        private double lng;
        private string _topic;
        private const string flickerAPiKey = "2f8bb794a1472233742e5077537bb4c6";
        public PhotoResults()
        {  
            InitializeComponent();
            Loaded += PhotoResults_Loaded;
            BuildLocalizedApplicationBar();

        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.IsVisible = false;

            //Create a new button and set the value to the localized string from AppResource

            ApplicationBarIconButton appButton = new ApplicationBarIconButton(new Uri("/Toolkit.Content/ApplicationBar.Check.png", UriKind.RelativeOrAbsolute));
            appButton.Text = AppResources.AppBarSet;
            appButton.Click += appButton_Click;
            ApplicationBar.Buttons.Add(appButton);
            //Create a new menu item with the localized string from AppResource
            //ApplicationBarMenuItem appbarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
           // ApplicationBar.MenuItems.Add(appbarMenuItem);
        
        }

        private async void appButton_Click(object sender, EventArgs e)
        {
            List<ApiImages> imgs = new List<ApiImages>();
            foreach (object item in PhotosForLockScreen.SelectedItems)
            {

                ApiImages img = item as ApiImages;
                if (img != null)
                {

                    imgs.Add(img);

                }

            }
                //Clean out all images in Isolated Storage currently
            LockScreenHelper.clearImages();

                //Save this new list of selected images to Isolated Storage

                LockScreenHelper.SaveSelectedImages(imgs);
                //Select one image at random and set it as a lock screen 
               await LockScreenHelper.setRandomImageToLockScreen();
                MessageBox.Show("You have a new Background Image", "Set!", MessageBoxButton.OK);

            
        }


       protected async void PhotoResults_Loaded(object sender, RoutedEventArgs e)
        {
          

            string baseUrl = getBaseUrl(flickerAPiKey, lat, lng,_topic);
            var images = await ApiImages.GetImages(baseUrl);
            DataContext = images;


        }

      

       private string getBaseUrl(string flickerAPiKey, double lat, double lng,string topic)
       {
          string[] licenses = { "4", "5", "6", "7" };
                 string license = String.Join(",", licenses);
                 license = license.Replace(",", "%2C");

                 if (!double.IsNaN(lat))
                     lat = Math.Round(lat, 5);

                 if (!double.IsNaN(lng))
                     lng = Math.Round(lng, 5);

            

                 string url = "http://api.flickr.com/services/rest/" +
                     "?method=flickr.photos.search" +
                     "&license={0}" +
                     "&api_key={1}" +
                     "&lat={2}" +
                     "&lon={3}" +
                     "&text={4}"+
                     "&radius=2"+
                     "&format=json&nojsoncallback=1";

                 var baseUrl = string.Format(url, license, flickerAPiKey, lat, lng,topic);

                 return baseUrl;
       }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            lat = Convert.ToDouble(NavigationContext.QueryString["latitude"]);
            lng = Convert.ToDouble(NavigationContext.QueryString["longitude"]);
                _topic = NavigationContext.QueryString["topic"];
        }

        
       

        private void PhotosForLockScreen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PhotosForLockScreen.SelectedItems.Count == 0)
                ApplicationBar.IsVisible = false;
            else
                ApplicationBar.IsVisible = true;

        } 

    }
}