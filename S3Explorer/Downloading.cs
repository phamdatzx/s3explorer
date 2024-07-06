using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Animation;
 using System.Text;
 using Amazon.S3;
 using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System.IO;

namespace S3Explorer
{

    internal partial class Downloading : Form
    {
        TransferUtility transferUtil;
        string downloadPath;
        AmazonS3Client s3Client;
        ListViewItem[] downloadItems;
        public Downloading(ListViewItem[] downloadItems, AmazonS3Client client, string localPath)
        {
            InitializeComponent();
            
            this.downloadPath = localPath;
            this.s3Client = client;
            transferUtil = new TransferUtility(client);
            this.downloadItems = downloadItems;
            StartDownload();
        }
        public async void StartDownload()
        {
            int itemsCount = downloadItems.Count();
            int i = 1;
            foreach (ListViewItem item in downloadItems)
            {
                label2.Text = i.ToString() + "/" + itemsCount.ToString();
                i++;
                string fullPath = GetListViewItemPath(item);
                string itemName = GetListViewItemName(item);
                label1.Text = "Downloading " + itemName;
                string itemType = item.SubItems[1].Text;
                // Định nghĩa tên bucket và key của file
                string bucketName = GetBucketNameFromFullPath(fullPath);
                string key = GetFolderPathFromFullPath(fullPath);
                if(itemType == "Folder")
                {
                   await DownloadS3DirectoryAsync(transferUtil, bucketName, key , downloadPath +"\\"+itemName);
                }
                else
                {
                    string key2 = ConvertBackslashToForwardslash(key);
                    await DownloadSingleFileAsync(transferUtil, bucketName, key2, downloadPath,itemName);
                }
            }   
            
            MessageBox.Show("Tai xuong thanh cong");
            this.Close();

        }
        public static string ConvertBackslashToForwardslash(string input)
        {
            return input.Replace("\\", "/");
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

        public static async Task<bool> DownloadSingleFileAsync(
        TransferUtility transferUtil,
            string bucketName,
            string keyName,
            string localPath,
            string itemName)
        {
            await transferUtil.DownloadAsync(new TransferUtilityDownloadRequest
            {
                BucketName = bucketName,
                Key = keyName,
                FilePath = $"{localPath}\\{itemName}",
            });

            return (System.IO.File.Exists($"{localPath}\\{keyName}"));
        }


        public static async Task<bool> DownloadS3DirectoryAsync(
            TransferUtility transferUtil,
            string bucketName,
            string s3Path,
            string localPath)
        {
            int fileCount = 0;

            // If the directory doesn't exist, it will be created.
            if (Directory.Exists(s3Path))
            {
                var files = Directory.GetFiles(localPath);
                fileCount = files.Length;
            }

            await transferUtil.DownloadDirectoryAsync(new TransferUtilityDownloadDirectoryRequest
            {
                BucketName = bucketName,
                LocalDirectory = localPath,
                S3Directory = s3Path,
            });

            if (Directory.Exists(localPath))
            {
                var files = Directory.GetFiles(localPath);
                if (files.Length > fileCount)
                {
                    return true;
                }

                // No change in the number of files. Assume
                // the download failed.
                return false;
            }

            // The local directory doesn't exist. No files
            // were downloaded.
            return false;
        }

    }
}
