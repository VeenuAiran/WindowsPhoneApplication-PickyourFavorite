using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Phone.System.UserProfile;

namespace PickFavPic
{
    class LockScreenHelper
    {
    
    private const string iconRoot = "Shared/ShellContent/";
        private const string BackgroundRoot = "Images/";
        private const string LockScreenImage = "LockScreenImage.json";

        public static void clearImages()
        {

            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                DeleteImages(storage, BackgroundRoot);
                DeleteImages(storage, iconRoot);


            }
        }

        private static void DeleteImages(IsolatedStorageFile storage, string directory)
        {
            if (storage.DirectoryExists(directory))
            {
                try
                {
                    string[] imageFiles = storage.GetFileNames(directory);
                    foreach (string f in imageFiles)
                    {

                        storage.DeleteFile(directory + f);
                    }

                }
                catch (Exception)
                {


                }


            }
        }


        public static void SaveSelectedImages(List<ApiImages> img)
        {
            var imgData = JsonConvert.SerializeObject(img);

            using (var storage = IsolatedStorageFile.GetUserStoreForApplication())
            {

                using (var stream = storage.CreateFile(LockScreenImage))
                {

                    using (StreamWriter w = new StreamWriter(stream))
                    {

                        w.Write(imgData);

                    }

                }
            }

        }

        public static async Task setRandomImageToLockScreen()
        {
            string filedata;
            using (IsolatedStorageFile storageFolder = IsolatedStorageFile.GetUserStoreForApplication())
            {

                if (!storageFolder.FileExists(LockScreenImage))

                    return;

                using (IsolatedStorageFileStream stream = storageFolder.OpenFile(LockScreenImage, FileMode.Open))
                {


                    using (StreamReader reader = new StreamReader(stream))
                    {
                        filedata = reader.ReadToEnd();
                    }
                }
            }

            List<ApiImages> images = JsonConvert.DeserializeObject<List<ApiImages>>(filedata);

            if (images != null)
            {

                Random rnd = new Random();
                int index = rnd.Next(images.Count);
                Debug.WriteLine(index + "::" + images[index].BigImage);
                await SetImageToLockScreen(images[index].BigImage);
            }
        }

        public static async Task SetImageToLockScreen(Uri uri)
        {
            String filename = uri.Segments[uri.Segments.Length - 1];
            string imageName = BackgroundRoot + filename;
            string iconName = iconRoot + filename;
            using (IsolatedStorageFile storageFolder = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!storageFolder.DirectoryExists(BackgroundRoot))
                    storageFolder.CreateDirectory(BackgroundRoot);
                if (!storageFolder.FileExists(imageName))
                {
                    using (IsolatedStorageFileStream stream = storageFolder.CreateFile(imageName))
                    {
                        HttpClient client = new HttpClient();
                        byte[] result = await client.GetByteArrayAsync(uri);
                        await stream.WriteAsync(result, 0, result.Length);

                    }

                    storageFolder.CopyFile(imageName, iconName);

                }

            }
            await SetLockScreen(filename);
        }

        private static async Task SetLockScreen(string filename)
        {
            bool LockScreenAccess = LockScreenManager.IsProvidedByCurrentApplication;

            if (!LockScreenAccess)
            {
                var requestAccess = await LockScreenManager.RequestAccessAsync();

                LockScreenAccess = (requestAccess == LockScreenRequestResult.Granted);

            }
            if (LockScreenAccess)
            {

                Uri uri = new Uri("ms-appdata:///local/" + BackgroundRoot + filename, UriKind.Absolute);
                LockScreen.SetImageUri(uri);
            }

       /**     var mainPageTile = ShellTile.ActiveTiles.FirstOrDefault();
            if (null!=mainPageTile)
            {
                Uri iconUri = new Uri("isostore:///" + iconRoot + filename, UriKind.Absolute);
                var imgs = new List<Uri>();
                imgs.Add(iconUri);

                CycleTileData tileImage = new CycleTileData();
                tileImage.CycleImages = imgs;
                mainPageTile.Update(tileImage);

            }**/

        }

        


    }
    
    
}
