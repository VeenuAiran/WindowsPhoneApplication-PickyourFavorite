using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PickFavPic
{
    public class ApiImages
    {

        public Uri SmallImage { get; set; }
        public Uri BigImage { get; set; }


        public async static Task<List<ApiImages>> GetImages(string baseUrl)
        {

            try
            {
                HttpClient client = new HttpClient();

                string result = await client.GetStringAsync(baseUrl);


                PhotoData data = JsonConvert.DeserializeObject<PhotoData>(result);
                List<ApiImages> images = new List<ApiImages>();

                if (data.stat == "ok")
                {
                    foreach (Photo photodata in data.photos.photo)
                    {
                        ApiImages img = new ApiImages();
                        //To retrive one photo, URL is http://farm{farmid}.staticflickr.com/{server-id}/{id}_{secret}{size}.jpg
                        string photourl = "http://farm{0}.staticflickr.com/{1}/{2}_{3}";
                        string basePhotourl = string.Format(photourl,
                            photodata.farm,
                            photodata.server,
                            photodata.id,
                            photodata.secret);
                        img.SmallImage = new Uri(basePhotourl + "_n.jpg");
                        img.BigImage = new Uri(basePhotourl + "_b.jpg");

                        images.Add(img);

                    }


                }
                return images;
            }
            catch (Exception e)
            {

                throw e.GetBaseException();
            }


        }
    
    }
}
