using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if (indexPassed < 0 || indexPassed > 1188)
                throw new IndexOutOfRangeException("Index passed in not a bible chapter number");

            // Get H Matrix From Blob Storage
            List<string[]> H = GetH();

            hasRead[indexPassed] = true;
            int k = 5;

            // Get Indices of largest value topics
            Tuple<int[], double[]> tuple = GetTopKTopics(H, indexPassed, k);

            double[,] columnVectors = GetTopicsFromIndexArray(H, tuple.Item1, k, hasRead);

            int tempIndex = GetClosestVectorsIndex(columnVectors, tuple.Item2);

            return tempIndex;
                /*
            // Weight the indices
            double[] probabilities = WeightIndices(tuple.Item2);

            // Return the topic 
            int topicIndex = tuple.Item1[FlipBiasedCoin(probabilities)];

            // Get indices of largest valued books
            Tuple<int[], double[]> tuple2 = GetTopBooks(H, topicIndex, k, hasRead);

            // Weight the indices
            probabilities = WeightIndices(tuple2.Item2);

            int tempIndex = FlipBiasedCoin(probabilities);
                
            // Get book index
            return tuple.Item1[tempIndex];
            */
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

        private Tuple<int[],double[]> GetTopKTopics(List<string[]> H, int index, int k)
        {
            int[] indices = new int[k];
            double[] values = new double[k];

            double value;

            foreach (string[] topic in H)
            {
                value = double.Parse(topic[index]);

                // Find the smallest value in arrays at present
                double smallest = int.MaxValue;
                int smallestIndex = int.MaxValue;
                for (int i = 0; i < indices.Length; ++i)
                {
                    if (smallest > values[i])
                    {
                        smallest = values[i];
                        smallestIndex = i;
                    }
                }

                // Replace smallest value with bigger one if applicable
                if (value > smallest)
                {
                    values[smallestIndex] = value;
                    indices[smallestIndex] = H.IndexOf(topic);
                }
            }

            return new Tuple<int[], double[]>(indices, values);
        }

        private double GetSquaredEuclideanDistance(double[] one, double[] two)
        {
            double distance_squared = 0;
            double diff;
            for (int i = 0; i < one.Length; ++i)
            {
                diff = Math.Pow(one[i] - two[i], 2);
                distance_squared += diff;
            }

            return distance_squared;
        }

        private double[,] GetTopicsFromIndexArray(List<string[]> H, int[] indices, int k_value, bool[] hasRead)
        {
            string[] tempTopic = H[0];
            int length = tempTopic.Length;
            int count = 0;
            for (int k = 0; k < hasRead.Length; ++k)
                if (hasRead[k]) ++count;

            double[,] chapterTopicColumnVectors = new double[k_value, length-count];
            int indexIter = 0;
            for (int i = 0; i < H.Count; ++i)
            {
                if (!hasRead[i])
                {
                    tempTopic = H[i];
                    for (int j = 0; j < indices.Length; ++j)
                    {
                        chapterTopicColumnVectors[j, indexIter++] = double.Parse(tempTopic[j]);
                    }
                }
            }

            return chapterTopicColumnVectors;
        }

        private int GetClosestVectorsIndex(double[,] columnVectors, double[] values)
        {
            int bestIndex = int.MaxValue;
            double smallestDistance = double.MaxValue;
            double tempDist = double.MaxValue;
            double[] temp = new double[values.Length];
            for (int i = 0; i < columnVectors.GetUpperBound(0); ++i)
            {
                for (int j = 0; j < values.Length; ++j)
                {
                    temp[j] = columnVectors[j, i];
                }
                tempDist = GetSquaredEuclideanDistance(values, temp);
                if (smallestDistance > tempDist)
                {
                    smallestDistance = tempDist;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }

        #endregion

        #region Deprecated

        private double[] WeightIndices(double[] weights)
        {
            double sum = 0;
            foreach (double weight in weights)
            {
                sum += weight;
            }
            for (int i = 0; i < weights.Length; ++i)
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

        private Tuple<int[], double[]> GetTopBooks(List<string[]> H, int index, int k, bool[] hasRead)
        {
            int[] indices = new int[k];
            double[] values = new double[k];

            string[] topic = H[index];
            for (int j = 0; j < topic.Length; ++j)
            {
                double value = double.Parse(topic[j]);
                double smallest = int.MaxValue;
                int smallestIndex = int.MaxValue;
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
