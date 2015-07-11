using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Random;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;

namespace BibleAppLibrary
{
    public class BackEnd
    {
        /*
         * 1. Somehow passed a bible chapter
         * 2. Take chapter index, find topic indices of five largest values in that column
         * 3. Weight the five values so that sum = 1, pull number 1-100, figure out which wins.
         * 4. Now with winning topic, pull five biggest indices of books that are not = passed value
         * 5. Weight to 1 and probablistically pick a book
         * 6. Check (if the user is signed in) if the book has been read.
         *      - If so, find top five again not including both books, and so on
         * 7. Once a book is found that has not been read, return it.
         * 8. Load that chapter.
         * */
        public int GetBibleBookIndex(int indexPassed, bool[] hasRead)
        {
            // First check to see indexPassed is valid
            if (indexPassed < 0 || indexPassed > 960)
                throw new IndexOutOfRangeException("Index passed in not a bible chapter number");

            // Get H Matrix From Blob Storage
            List<string[]> H = GetH();

            // Get Indices of largest value topics
            Tuple<int[], double[]> tuple = GetTopFiveTopics(H, indexPassed);

            // Weight the indices
            double[] probabilities = WeightIndices(tuple.Item2);

            // Return the topic 
            int topicIndex = tuple.Item1[FlipBiasedCoin(probabilities)];

            // Get indices of largest valued books
            Tuple<int[], double[]> tuple2 = GetTopFiveBooks(H, topicIndex, hasRead);

            // Weight the indices
            probabilities = WeightIndices(tuple2.Item2);

            int tempIndex = FlipBiasedCoin(probabilities);

            // Get book index
            return tuple.Item1[tempIndex];

        }

        #region Util

        private List<string[]> GetH()
        {
            // ==== Retrieve H matrix ====
            // Connect to the blob
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=c4tk;AccountKey=61vOUO5m1BXi1RsWNCxD/YrlTzq/imAsr65doK4iTL5sey33DErbJsiUIxP6TfYPhpP8N2KIIakqUPzLlK/WHQ==");
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("c4tk-container");
            string blobName = "h_nmf_blob";
            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

            string data = "";
            using (MemoryStream memoryStream = new MemoryStream())
            {
                blob.DownloadToStream(memoryStream);
                data = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
            }

            List<string[]> H = JsonConvert.DeserializeObject<List<string[]>>(data);
            // ==== Got H Marix ====

            return H;
        }

        private Tuple<int[],double[]> GetTopFiveTopics(List<string[]> H, int index)
        {
            int[] indices = new int[5];
            double[] values = new double[5];

            double value;
            foreach (string[] topic in H)
            {
                value = double.Parse(topic[index]);
                double smallest = int.MaxValue;
                int smallestIndex = 6;
                for (int i = 0; i < indices.Length; ++i)
                {
                    if (smallest > values[i])
                    {
                        smallest = values[i];
                        smallestIndex = i;
                    }
                }
                if (value > smallest)
                {
                    values[smallestIndex] = value;
                    indices[smallestIndex] = H.IndexOf(topic);
                }
            }

            return new Tuple<int[], double[]>(indices, values);
        }

        private double[] WeightIndices(double[] weights)
        {
            double sum = 0;
            foreach (double weight in weights)
            {
                sum += weight;
            }
            for(int i = 0; i < weights.Length; ++i)
            {
                weights[i] = weights[i] / sum;
            }

            return weights;
        }

        private int FlipBiasedCoin(double[] probabilities)
        {
            double ceil1 = probabilities[0];
            double ceil2 = probabilities[1];
            double ceil3 = probabilities[2];
            double ceil4 = probabilities[3];
            Random random = new Random();

            double coin = random.NextDouble();

            if (coin < ceil1)
                return 0;
            else if (coin < ceil2)
                return 1;
            else if (coin < ceil3)
                return 2;
            else if (coin < ceil4)
                return 3;
            else
                return 4;

        }

        private Tuple<int[], double[]> GetTopFiveBooks(List<string[]> H, int index, bool[] hasRead)
        {
            int[] indices = new int[5];
            double[] values = new double[5];

            string[] topic = H[index];
            for (int j = 0; j < topic.Length; ++j)
            {
                double value = double.Parse(topic[j]);
                double smallest = int.MaxValue;
                int smallestIndex = 6;
                for (int i = 0; i < indices.Length; ++i)
                {
                    if (smallest > values[i])
                    {
                        smallest = values[i];
                        smallestIndex = i;
                    }
                }
                if (!hasRead[j])
                {
                    if (value > smallest)
                    {
                        values[smallestIndex] = value;
                        indices[smallestIndex] = j;
                    }
                }
            }

            return new Tuple<int[], double[]>(indices, values);
        }

        #endregion
    }
}
