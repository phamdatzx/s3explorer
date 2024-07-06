using Amazon.Runtime;
using Amazon.S3.IO;
using Amazon.S3.Model;
using Amazon.S3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace S3Explorer
{
    internal class S3Acount
    {
        BasicAWSCredentials credentials;

        public AmazonS3Client acountClient;

        public List<Bucket> buckets;
        public S3Acount(string awsAccessKey, string awsSecretAccessKey)
        {
            credentials = new BasicAWSCredentials(awsAccessKey, awsSecretAccessKey);
            acountClient = new AmazonS3Client(credentials, Amazon.RegionEndpoint.APSoutheast1);

            buckets = GetBucketList();

        }

        public List<Bucket> GetBucketList()
        {
            //issue call
            ListBucketsResponse response = acountClient.ListBuckets();

            //bucket list from response
            List<Bucket> bucketList = new List<Bucket>();
            foreach (S3Bucket s3bucket in response.Buckets)
            {
                Bucket bucket = new Bucket(s3bucket.BucketName, credentials);
                bucketList.Add(bucket);
            }

            return bucketList;
        }
    }
}
