using Amazon.S3.IO;
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
using Amazon.S3.Model;
using Amazon.S3;
using System.Threading;
using Amazon.S3.Transfer;

namespace S3Explorer
{
    public partial class MainForm : Form
    {
        //Load data for treeview s3
        public void LoadTreeViewS3()
        {
            treeViewS3.Nodes.Clear();
            //get list bucket of acount
            List<Bucket> bucketList = acount.GetBucketList();
            //
            treeViewS3.ImageList = imageList1;
            //add list bucket to treeview
            foreach (Bucket bucket in bucketList)
            {
                treeViewS3.Nodes.Add(bucket.rootNode);
            }
        }

        //get full path of treenode form ( BucketName:/***/*** )
        private string GetNodePath(TreeNode node)
        {
            List<string> nodePath = new List<string>();

            // Duyệt từ node hiện tại đến root node
            while (node != null)
            {
                nodePath.Insert(0, node.Text);
                node = node.Parent;
            }

            nodePath[0] = nodePath[0] + ":";

            // Nối các phần tử trong danh sách thành đường dẫn
            return string.Join(@"\", nodePath);
        }

        //Get S3directory info by full path
        private S3DirectoryInfo GetS3DirectoryInfo(string path)
        {
            string bucket = GetBucketNameFromFullPath(path);
            string folder = GetFolderPathFromFullPath(path);
            S3DirectoryInfo result = new S3DirectoryInfo(acount.acountClient, bucket, folder);
            return result;
        }

        //Get bucketName from fullPath
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

        private void LoadListViewData()
        {
            listViewS3.Items.Clear();
            S3DirectoryInfo[] folderArray = currentS3Directory.GetDirectories();
            S3FileInfo[] fileArray = currentS3Directory.GetFiles();

            foreach (var folder in folderArray)
            {
                //set name for item
                ListViewItem item = new ListViewItem(folder.Name);
                //set item type
                item.SubItems.Add("Folder");
                //last change time
                item.SubItems.Add(folder.LastWriteTime.ToString());
                //set image
                item.ImageIndex = 1;
                //set path for item ( hidden)
                item.SubItems.Add(folder.FullName);
                listViewS3.Items.Add(item);
            }

            foreach (var file in fileArray)
            {
                //set name for item
                ListViewItem item = new ListViewItem(file.Name);
                //set type
                item.SubItems.Add(ClassifyFile(file.Name));
                //set last time change
                item.SubItems.Add(file.LastWriteTime.ToString());
                //set path (hidden)
                item.SubItems.Add(file.FullName);
                //set image
                item.ImageIndex = 2;
                listViewS3.Items.Add(item);
            }

            LoadPathToTextBoxS3Path();
        }

        public static string ClassifyFile(string filename)
        {
            /**
             * Phân loại tệp tin dựa trên phần mở rộng của tên tệp.
             * 
             * Tham số:
             * filename (string): Tên tệp tin, bao gồm cả phần mở rộng.
             * 
             * Trả về:
             * string: Loại tệp tin.
             */
            string extension = Path.GetExtension(filename).ToLower().TrimStart('.');

            switch (extension)
            {
                case "doc":
                case "docx":
                    return "Microsoft Word document";
                case "xls":
                case "xlsx":
                    return "Microsoft Excel document";
                case "ppt":
                case "pptx":
                    return "Microsoft PowerPoint presentation";
                case "pdf":
                    return "PDF document";
                case "txt":
                    return "Text file";
                case "jpg":
                case "jpeg":
                case "png":
                case "gif":
                    return "Image file";
                case "mp3":
                case "wav":
                case "ogg":
                    return "Audio file";
                case "mp4":
                case "avi":
                case "mov":
                    return "Video file";
                default:
                    return "Unknown file type";
            }
        }

        private void LoadPathToTextBoxS3Path()
        {
            textBoxS3Path.Text = currentS3Directory.FullName;
        }

        public string GetListViewItemPath(ListViewItem item)
        {
            return item.SubItems[3].Text;
        }
        public string GetListViewItemName(ListViewItem item)
        {
            return item.SubItems[0].Text;
        }


        private void LoadTreeViewLocal()
        {
            treeViewLocal.BeginUpdate();
            treeViewLocal.ImageList = imageList2;
            string[] drives = Directory.GetLogicalDrives();
            foreach (string drive in drives)
            {
                TreeNode driveNode = new TreeNode(drive);
                driveNode.ImageIndex = 0;
                treeViewLocal.Nodes.Add(driveNode);
                LoadChildrenForTreeNode(driveNode);
            }

            treeViewLocal.EndUpdate();
        }

        private void LoadChildrenForTreeNode(TreeNode node)
        {
            string path = node.FullPath;
            DirectoryInfo directory = new DirectoryInfo(path);
            DirectoryInfo[] subDirectories = { };

            try
            {
                if (directory.Exists)
                    subDirectories = directory.GetDirectories();
            }
            catch { return; }

            foreach(DirectoryInfo subDirectory in subDirectories)
            {
                TreeNode childNode = new TreeNode(subDirectory.Name);
                childNode.ImageIndex = 1;
                node.Nodes.Add(childNode);
            }
        }

        private void LoadListViewLocalData()
        {
            DirectoryInfo[] subDirectories = { };
            FileInfo[] files = { };

            try
            {
                if (currentLocalDirectory != null && currentLocalDirectory.Exists )
                {
                    subDirectories = currentLocalDirectory.GetDirectories();
                    files = currentLocalDirectory.GetFiles();
                }
            }
            catch { return; }

            listViewLocal.BeginUpdate();
            listViewLocal.Items.Clear();
            foreach(DirectoryInfo subDirectory in subDirectories)
            {
                //set name for item
                ListViewItem item = new ListViewItem(subDirectory.Name);
                //set item type
                item.SubItems.Add("Folder");
                //last change time
                item.SubItems.Add(subDirectory.LastWriteTime.ToString());
                //set image
                item.ImageIndex = 1;
                //set path for item ( hidden)
                item.SubItems.Add(subDirectory.FullName);
                listViewLocal.Items.Add(item);
            }

            foreach (var file in files)
            {
                //set name for item
                ListViewItem item = new ListViewItem(file.Name);
                //set type
                item.SubItems.Add(ClassifyFile(file.Name));
                //set last time change
                item.SubItems.Add(file.LastWriteTime.ToString());
                //set path (hidden)
                item.SubItems.Add(file.FullName);
                //set image
                item.ImageIndex = 2;
                listViewLocal.Items.Add(item);
            }
            listViewLocal.EndUpdate();

            LoadPathToTextBoxLocalPath();
        }

        private void LoadPathToTextBoxLocalPath()
        {
            textBoxLocalPath.Text = currentLocalDirectory.FullName;
        }

        private void UploadObject(string localPath, string s3Path)
        {
            string fileName = "CounterSide_Setup.1.1.22-5.bin";
          //  string localPath = "C:\\Users\\ACER\\Desktop";
            string uploadPath = "folder1";
            string bucketName = "phamdatbucket1";

            var transferUtil = new TransferUtility(acount.acountClient);

          //  await UploadSingleFileAsync(transferUtil, bucketName, uploadPath, fileName, localPath);
            MessageBox.Show("");
        }

        public static async Task<bool> UploadSingleFileAsync(
            TransferUtility transferUtil,
            string bucketName,
            string uploadPath,
            string fileName,
            string localPath)
        {
            if (System.IO.File.Exists($"{localPath}\\{fileName}"))
            {
                try
                {
                    await transferUtil.UploadAsync(new TransferUtilityUploadRequest
                    {
                        BucketName = bucketName,
                        Key = $"{uploadPath}/{fileName}",
                        FilePath = $"{localPath}\\{fileName}",
                    });

                    return true;
                }
                catch (AmazonS3Exception s3Ex)
                {
                    Console.WriteLine($"Could not upload {fileName} from {localPath} because:");
                    Console.WriteLine(s3Ex.Message);
                    return false;
                }
            }
            else
            {
                Console.WriteLine($"{fileName} does not exist in {localPath}");
                return false;
            }
        }




    }
}
