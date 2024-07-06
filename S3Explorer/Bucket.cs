using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.IO;
using Amazon.S3.Model;
using System.Windows.Forms;

namespace S3Explorer
{
    internal class Bucket
    {
        BasicAWSCredentials credentials;

        string name;                            //bucket name

        Amazon.RegionEndpoint region;           //region of bucket

        DateTime creationDate;

        //directory tree of bucket
        public TreeNode rootNode;
        S3DirectoryInfo directoryInfo;

        //client to processing data of bucket
        AmazonS3Client client;

        //constructor
        public Bucket(string name, BasicAWSCredentials credentials)
        {
            this.credentials = credentials;
            //
            this.name = name;
            //region 
            region = this.GetBucketRegion();

            //initializing bucket client 
            client = new AmazonS3Client(credentials, region);
            //self-load directory tree of this bucket

            directoryInfo = new S3DirectoryInfo(client, name);

            rootNode = new TreeNode();
            rootNode.Text = name;
            rootNode.ImageIndex = 0;
            rootNode.SelectedImageIndex = 0;

            S3DirectoryToTreeNode(directoryInfo, rootNode);
        }
        public Amazon.RegionEndpoint GetBucketRegion()
        {

            using (var s3Client = new AmazonS3Client(credentials, Amazon.RegionEndpoint.APSoutheast1))
            {
                // Tạo một request để lấy thông tin vùng của bucket
                var request = new GetBucketLocationRequest
                {
                    BucketName = this.name
                };

                // Gửi request và nhận về response
                var response = s3Client.GetBucketLocation(request);

                // Vùng sẽ được trả về dưới dạng mã quốc gia
                string region = response.Location.ToString();

                // Chuyển đổi mã quốc gia thành tên vùng
                Amazon.RegionEndpoint regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(region);

                return regionEndpoint;
            }
        }

        public static void S3DirectoryToTreeNode(S3DirectoryInfo DirectoryRoot, TreeNode treeRoot)
        {
            // Lặp qua danh sách các thư mục con của thư mục gốc
            foreach (var directory in DirectoryRoot.GetDirectories())
            {
                // Lấy tên của thư mục
                string directoryName = directory.Name;

                // Tạo TreeNode mới với tên thư mục
                TreeNode directoryNode = new TreeNode();
                //
                directoryNode.Text = directoryName;
                // Thêm TreeNode vào TreeNode gốc
                treeRoot.Nodes.Add(directoryNode);

                //set image
                directoryNode.ImageIndex = 1;
                directoryNode.SelectedImageIndex = 1;

                // Đệ quy gọi hàm này để sao chép thông tin của các thư mục con
                S3DirectoryToTreeNode(directory, directoryNode);


               

                
     
            }
        }

        //private S3DirectoryInfo GetDirectoryInfo(string path)
        //{
        //    //S3DirectoryInfo result;


        //    //return result;
        //}
    }
}
