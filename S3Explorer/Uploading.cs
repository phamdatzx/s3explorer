using Amazon.S3.Transfer;
using Amazon.S3;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace S3Explorer
{
    public partial class Uploading : Form
    {
        TransferUtility transferUtil;
        string uploadPath;
        AmazonS3Client s3Client;
        ListViewItem[] uploadItems;
        public Uploading(ListViewItem[] uploadItems, AmazonS3Client client, string uploadPath)
        {
            InitializeComponent();
            this.s3Client = client;
            this.uploadItems = uploadItems;
            this.uploadPath = uploadPath;
            transferUtil = new TransferUtility(client);
            StartUpload();
        }
        public string GetListViewItemPath(ListViewItem item)
        {
            return item.SubItems[3].Text;
        }
        public string GetListViewItemName(ListViewItem item)
        {
            return item.SubItems[0].Text;
        }

        private static string GetBucketNameFromFullPath(string path)
        {
            // Tách chuỗi bằng dấu :
            string[] parts = path.Split(':');

            // Lấy phần tử đầu tiên và loại bỏ dấu :
            string bucket = parts[0];
            return bucket;
        }

        private static string GetFolderPathFromFullPath(string path)
        {
            string bucket = GetBucketNameFromFullPath(path);
            int index = bucket.Length + 2;

            string folderPath = "";
            if (index < path.Length)
            {
                folderPath = path.Substring(index);
            }
            return folderPath;
        }

        public async void StartUpload()
        {
            int itemsCount = uploadItems.Count();
            int i = 1;
            foreach (ListViewItem item in uploadItems)
            {
                label2.Text = i.ToString() + "/" + itemsCount.ToString();
                i++;
                string localPath = GetListViewItemPath(item);
                string itemName = GetListViewItemName(item);
                label1.Text = "Uploading " + itemName;
                string itemType = item.SubItems[1].Text;

                // Định nghĩa tên bucket và key của file
                string bucketName = GetBucketNameFromFullPath(uploadPath);

                string key = GetFolderPathFromFullPath(uploadPath);

                if (itemType == "Folder")
                {
                    await UploadFullDirectoryAsync(transferUtil, bucketName, ConvertBackslashToForwardslash(key) + itemName +"/", localPath);
                }
                else
                {
                    await UploadSingleFileAsync(transferUtil, bucketName,ConvertBackslashToForwardslash(key) + itemName, localPath);
                }
            }

            MessageBox.Show("Tai len thanh cong");
            this.Close();

        }

        public static string ConvertBackslashToForwardslash(string input)
        {
            return input.Replace("\\", "/");
        }

        public static async Task<bool> UploadSingleFileAsync(
            TransferUtility transferUtil,
            string bucketName,
            string key,
            string localPath)
        {
            if (System.IO.File.Exists(localPath))
            {
                try
                {
                    await transferUtil.UploadAsync(new TransferUtilityUploadRequest
                    {
                        BucketName = bucketName,
                        Key = key,
                        FilePath = localPath
                    });

                    return true;
                }
                catch (AmazonS3Exception s3Ex)
                {
                    Console.WriteLine($"Could not upload {key} from {localPath} because:");
                    Console.WriteLine(s3Ex.Message);
                    return false;
                }
            }
            else
            {
                Console.WriteLine($"{key} does not exist in {localPath}");
                return false;
            }
        }

        public static async Task<bool> UploadFullDirectoryAsync(
           TransferUtility transferUtil,
           string bucketName,
           string keyPrefix,
           string localPath)
        {
            if (Directory.Exists(localPath))
            {
                try
                {
                    await transferUtil.UploadDirectoryAsync(new TransferUtilityUploadDirectoryRequest
                    {
                        BucketName = bucketName,
                        KeyPrefix = keyPrefix,
                        Directory = localPath,
                    });

                    return true;
                }
                catch (AmazonS3Exception s3Ex)
                {
                    Console.WriteLine($"Can't upload the contents of {localPath} because:");
                    Console.WriteLine(s3Ex?.Message);
                    return false;
                }
            }
            else
            {
                Console.WriteLine($"The directory {localPath} does not exist.");
                return false;
            }
        }

        public static string RemoveLastTwoBackslashes(string inputString)
        {
            if (inputString.EndsWith("\\\\"))
            {
                return inputString.Substring(0, inputString.Length - 2);
            }
            else
            {
                return inputString;
            }
        }


    }
}
