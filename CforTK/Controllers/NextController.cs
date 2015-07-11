using BibleAppLibrary;
using CforTK.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;

namespace CforTK.Controllers
{
    public class NextController : ApiController
    {
        CloudStorageAccount storageAccount;
        CloudBlobClient blobClient;
        CloudBlobContainer container;

        public NextController()
        {
            storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=c4tk;AccountKey=61vOUO5m1BXi1RsWNCxD/YrlTzq/imAsr65doK4iTL5sey33DErbJsiUIxP6TfYPhpP8N2KIIakqUPzLlK/WHQ==");
            blobClient = storageAccount.CreateCloudBlobClient();
            container = blobClient.GetContainerReference("c4tk-container");
        }

        // GET api/next
        public JToken Get()
        {
            string blobName = "bibleBooks";
            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);
            string a = blob.DownloadText();
            JToken json = JObject.Parse(a);
            return json;
        }

        // GET api/next/5
        public async Task<string> Get(string id)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage message = await client.GetAsync("http://www.esvapi.org/v2/rest/passageQuery?key=IP&passage=" + id.Replace('q', '+'));
            string value = await message.Content.ReadAsStringAsync();
            return value;
        }

        // PUT api/next/5
        public int Put(int id, [FromBody]Text value)
        {
            bool[] hasRead = new bool[1189];
            string blobName = "user/j388923r";
            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

            blob.UploadText(value.value);

            for (int k = 0; k < Math.Min(value.value.Length, 1189); k++)
            {
                hasRead[k] = value.value.ElementAt(k) == '1';
            }
            return BackEnd.GetBibleBookIndex(id, hasRead);
        }
    }
}
