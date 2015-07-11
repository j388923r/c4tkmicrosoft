using BibleAppLibrary;
using CforTK.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace CforTK.Controllers
{
    public class NextController : ApiController
    {
        // GET api/next
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/next/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/next
        public void Post([FromBody]string value)
        {
        }

        // PUT api/next/5
        public int Put(int id, [FromBody]Text value)
        {
            bool[] hasRead = new bool[1189];
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=c4tk;AccountKey=61vOUO5m1BXi1RsWNCxD/YrlTzq/imAsr65doK4iTL5sey33DErbJsiUIxP6TfYPhpP8N2KIIakqUPzLlK/WHQ==");
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("c4tk-container");
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
