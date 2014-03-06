using System;
using System.Diagnostics;
using System.IO;
using Amazon.S3.Transfer;
using NUnit.Framework;

namespace AmazonS3
{
    public class BasicTests
    {
        [Test]
        public void Test()
        {
            // http://docs.aws.amazon.com/AmazonS3/latest/dev/HLuploadFileDotNet.html

            string[] credentials = File.ReadAllLines(@"..\..\..\s3-credentials.txt");

            string accessKeyID = credentials[0];
            string secretAccessKey = credentials[1];
            string bucketName = credentials[2];

            TransferUtilityConfig config = new TransferUtilityConfig ();
            config.DefaultTimeout = 5 * 60 * 1000;

            using (TransferUtility utility = new TransferUtility(accessKeyID, secretAccessKey, config))
            {
                //Stopwatch stopwatch = new Stopwatch();
                //stopwatch.Start();

                //string fileName = @"D:\MyStuff\Google Drive\ScalableMaps\map-products\china-natural-bg-low.pdf";

                ////TransferUtilityUploadRequest request = new TransferUtilityUploadRequest();
                //utility.Upload(fileName, bucketName);

                //Console.WriteLine(
                //    "Uploaded in {0} ms ({1} KB/s)", 
                //    stopwatch.ElapsedMilliseconds,
                //    new FileInfo(fileName).Length / 1024.0 / (stopwatch.ElapsedMilliseconds / 1000));

                utility.Download (@"d:\brisi\china-natural-bg-low.pdf", bucketName, "china-natural-bg-low.pdf");
            }

            // http://stackoverflow.com/questions/9920804/how-to-list-all-objects-in-amazon-s3-bucket
            // http://stackoverflow.com/questions/1161022/storing-my-amazon-credentials-in-c-sharp-desktop-app?rq=1
            // http://stackoverflow.com/questions/1856988/how-to-restrict-amazon-s3-api-access
        }
    }
}
