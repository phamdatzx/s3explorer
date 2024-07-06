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
using System.Windows.Media.Animation;
using System.Runtime.InteropServices;
using S3Explorer.Properties;

namespace S3Explorer
{
    public partial class MainForm : Form
    {
        string accessKey;
        string secretKey;
        S3Acount acount;

        S3DirectoryInfo currentS3Directory;
        DirectoryInfo currentLocalDirectory;
        ListViewItem[] pasteItems;
        bool cutFlag = false;

        public MainForm()
        {
            InitializeComponent();
            listViewS3.SmallImageList = imageList1;
            listViewLocal.SmallImageList = imageList1;

            var loginForm = new LoginForm();
            var result = loginForm.ShowDialog();

            accessKey = loginForm.AccessKey;
            secretKey = loginForm.SecretKey;

            acount = new S3Acount(accessKey, secretKey);
            LoadTreeViewS3();
            LoadTreeViewLocal();
        }

        private void fileTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Lấy đường dẫn của node được chọn
            string path = GetNodePath(e.Node);

            //set path for currentDirectory
            currentS3Directory = GetS3DirectoryInfo(path);

            LoadListViewData();

        }

        

        private void toolStripTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                ToolStripTextBox textBox = (ToolStripTextBox)sender;
                string path = textBox.Text;
                var directory = GetS3DirectoryInfo(path);

                if (directory.Exists)
                {
                    textBox.Text = directory.FullName;
                    currentS3Directory = directory;
                    LoadListViewData();
                }
                else
                {
                    MessageBox.Show("Duong dan sai", "Thong Bao", MessageBoxButtons.OK);
                }
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            ListViewItem clickItem = listViewS3.FocusedItem as ListViewItem;

            string itemType = clickItem.SubItems[1].Text;

            if (itemType == "Folder")
            {
                currentS3Directory = GetS3DirectoryInfo(GetListViewItemPath(clickItem));
                LoadListViewData();
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            currentS3Directory = currentS3Directory.Parent;
            LoadListViewData();
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            ToolStripTextBox textBox = (ToolStripTextBox)sender;
            string path = textBox.Text;
            var directory = GetS3DirectoryInfo(path);

            if (directory.Exists)
            {
                
            }
            else
            {
                MessageBox.Show("Duong dan sai", "Thong Bao", MessageBoxButtons.OK);
            }
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            string downloadPath;
            if(currentLocalDirectory is null)
            {
                downloadPath = "";
            }
            else
            {
                downloadPath = currentLocalDirectory.FullName;
            }

            ListViewItem[] selectedItems;
            selectedItems = new ListViewItem[listViewS3.SelectedItems.Count];
            if(listViewS3.SelectedItems.Count == 0)
            {
                MessageBox.Show("Hay chon folder, file can tai xuong","Thong bao",MessageBoxButtons.OK);
                return;
            }
            listViewS3.SelectedItems.CopyTo(selectedItems, 0);
            DownLoadForm downLoad = new DownLoadForm(selectedItems, acount,downloadPath);
            downLoad.Show();
            LoadListViewLocalData();
        }

        

        private void listView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;    
        }


        private void listView2_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            
        }

        private void listView2_DragDrop(object sender, DragEventArgs e)
        {
            

        }

        private void treeViewLocal_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {

        }

        private void treeViewLocal_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Lấy đường dẫn của node được chọn
            string path = e.Node.FullPath;

            //set path for currentDirectory
            currentLocalDirectory = new DirectoryInfo(path);

            //load data for listviewlocal
            LoadListViewLocalData();
        }

        private void listViewLocal_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem clickItem = listViewLocal.FocusedItem as ListViewItem;

            string itemType = clickItem.SubItems[1].Text;

            if (itemType == "Folder")
            {
                currentLocalDirectory = new DirectoryInfo(GetListViewItemPath(clickItem));
                LoadListViewLocalData();
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (currentLocalDirectory != null && currentLocalDirectory.Parent != null)
            {
                currentLocalDirectory = currentLocalDirectory.Parent;
                LoadListViewLocalData();
            }
        }

        private void textBoxLocalPath_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    ToolStripTextBox textBox = (ToolStripTextBox)sender;
                    string path = textBox.Text;
                    var directory = new DirectoryInfo(path);

                    if (directory.Exists)
                    {
                        textBox.Text = directory.FullName;
                        currentLocalDirectory = directory;
                        LoadListViewLocalData();
                    }
                    else
                    {
                        MessageBox.Show("Duong dan sai", "Thong Bao", MessageBoxButtons.OK);
                    }
                }
            }
            catch { MessageBox.Show("Duong dan sai", "Thong Bao", MessageBoxButtons.OK); }
            
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            string uploadPath;
            if (currentS3Directory is null)
            {
                uploadPath = "";
            }
            else
            {
                uploadPath = currentS3Directory.FullName;
            }

            ListViewItem[] selectedItems;
            selectedItems = new ListViewItem[listViewLocal.SelectedItems.Count];
            if (listViewLocal.SelectedItems.Count == 0) {
                MessageBox.Show("Hay chon folder, file can tai len", "Thong bao", MessageBoxButtons.OK);
                return;
            }
            listViewLocal.SelectedItems.CopyTo(selectedItems, 0);
            UploadForm upload = new UploadForm(selectedItems, acount, uploadPath);
            upload.Show();

            LoadListViewData();
        }

        private async void toolStripButton7_Click(object sender, EventArgs e)
        {
            string deletePath;
            if (currentS3Directory is null)
            {
                deletePath = "";
            }
            else
            {
                deletePath = currentS3Directory.FullName;
            }

            ListViewItem[] selectedItems;
            selectedItems = new ListViewItem[listViewS3.SelectedItems.Count];
            listViewS3.SelectedItems.CopyTo(selectedItems, 0);
            if (listViewS3.SelectedItems.Count == 0)
            {
                MessageBox.Show("Hay chon folder, file can xoa", "Thong bao", MessageBoxButtons.OK);
                return;
            }

            foreach (ListViewItem item in selectedItems)
            {
                
                string fullPath = GetListViewItemPath(item);

                string itemName = GetListViewItemName(item);
                
                string itemType = item.SubItems[1].Text;
                // Định nghĩa tên bucket và key của file
                string bucketName = GetBucketNameFromFullPath(fullPath);
                string key = GetFolderPathFromFullPath(fullPath);

                if (itemType == "Folder")
                {
                    string key2 = ConvertBackslashToForwardslash(key);

                    DeleteS3Directory(bucketName, key2);  
                }
                else
                {
                    string key2 = ConvertBackslashToForwardslash(key);
                    try 
                    { 
                        await DeleteObject(acount.acountClient, bucketName, key2);
                        MessageBox.Show($"Đã xóa '{key}' trong bucket '{bucketName}'.");
                    }
                    catch { }
                }
            }

            LoadListViewData();
        }

        public static void DeleteBucketDirectory(AmazonS3Client client, string bucketName, string fullPath)
        {
            S3DirectoryInfo directoryToDelete = new S3DirectoryInfo(client, bucketName, fullPath);
            directoryToDelete.Delete(true);
        }

        public static async Task DeleteObject(AmazonS3Client client, string bucketName, string key)
        {
            var deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = key
            };
           
            // Gửi yêu cầu xóa file
            try
            {
                await client.DeleteObjectAsync(deleteObjectRequest);
            }
            catch (AmazonS3Exception s3Exception)
            {
                MessageBox.Show($"Lỗi khi xóa file: {s3Exception.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }

        }

        public static string ConvertBackslashToForwardslash(string input)
        {
            return input.Replace("\\", "/");
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            if (listViewS3.SelectedItems.Count == 0)
            {
                MessageBox.Show("Hay chon folder, file can sao chep", "Thong bao", MessageBoxButtons.OK);
                return;
            }
            else
            {
                cutFlag = false;
                pasteItems = new ListViewItem[listViewS3.SelectedItems.Count];
                listViewS3.SelectedItems.CopyTo(pasteItems, 0);
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (listViewS3.SelectedItems.Count == 0)
            {
                MessageBox.Show("Hay chon folder, file can di chuyen", "Thong bao", MessageBoxButtons.OK);
                return;
            }
            else
            {
                cutFlag = true;
                pasteItems = new ListViewItem[listViewS3.SelectedItems.Count];
                listViewS3.SelectedItems.CopyTo(pasteItems, 0);
            }
        }

        private async void  toolStripButton2_Click(object sender, EventArgs e)
        {
            string destPath = currentS3Directory.FullName;
            string destBucket = GetBucketNameFromFullPath(destPath);
            string destKey = ConvertBackslashToForwardslash(GetFolderPathFromFullPath(destPath));
            if(destKey == null)
            {
                MessageBox.Show("Noi dan khong hop le");
                return;
            }

            //copy
            foreach (ListViewItem item in pasteItems)
            {
                string itemPath = GetListViewItemPath(item);
                string sourceBucket = GetBucketNameFromFullPath(itemPath);
                string sourceKey = ConvertBackslashToForwardslash(GetFolderPathFromFullPath(itemPath));
                if (item.SubItems[1].Text !="Folder")
                {
                    string destinationKey = destKey + GetListViewItemName(item);
                    CopyObjectRequest copyRequest = new CopyObjectRequest
                    {
                        SourceBucket = sourceBucket,
                        SourceKey = sourceKey,
                        DestinationBucket = destBucket,
                        DestinationKey = destinationKey
                    };

                    await acount.acountClient.CopyObjectAsync(copyRequest);
                }
                else
                {
                    CopyFolderInsideS3Bucket(sourceKey, destKey ,sourceBucket,destBucket);
                }
                
            }

            if (cutFlag)
            {
                //delete
                foreach (ListViewItem item in pasteItems)
                {

                    string fullPath = GetListViewItemPath(item);

                    string itemName = GetListViewItemName(item);

                    string itemType = item.SubItems[1].Text;
                    // Định nghĩa tên bucket và key của file
                    string bucketName = GetBucketNameFromFullPath(fullPath);
                    string key = GetFolderPathFromFullPath(fullPath);

                    if (itemType == "Folder")
                    {
                        string key2 = ConvertBackslashToForwardslash(key);

                        DeleteS3Directory(bucketName, key2);
                    }
                    else
                    {
                        string key2 = ConvertBackslashToForwardslash(key);
                        await DeleteObject(acount.acountClient, bucketName, key2);
                    }
                }
                MessageBox.Show("Di chuyen thanh cong","Thong bao",MessageBoxButtons.OK);

            }
            else
            {
                MessageBox.Show("Sao chep thanh cong", "Thong bao", MessageBoxButtons.OK);

            }
            LoadListViewData();
        }

        private void MovePasteItems()
        {
                
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            LoadListViewData();
            LoadTreeViewS3();
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            string path = currentS3Directory.FullName;
            string bucketName = GetBucketNameFromFullPath(path);
            string key = ConvertBackslashToForwardslash(GetFolderPathFromFullPath(path));
            ListViewItem[] items = new ListViewItem[listViewS3.Items.Count];
            listViewS3.Items.CopyTo(items, 0);
            NewDirectory createDirectoryForm = new NewDirectory(acount.acountClient,bucketName, key,items);
            createDirectoryForm.ShowDialog();
            LoadListViewData();
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            CreateBucket createBucket = new CreateBucket(acount.acountClient);
            createBucket.ShowDialog();
            LoadTreeViewS3();
        }

        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            List<TreeNode> listNode = new List<TreeNode>();

            foreach(TreeNode node in treeViewS3.Nodes)
            {
                if (node.IsSelected)
                {
                    listNode.Add(node);
                }
            }

            if(listNode.Count !=1)
            {
                MessageBox.Show("Hay chon 1 bucket de xoa");
                return;
            }
            else
            {
                TreeNode deleteNode = listNode[0];
                string bucketName = deleteNode.Text;

                DeleteBucket(bucketName); 
            }


            

        }

        private async void DeleteBucket(string bucketName)
        {
            var request = new ListObjectsV2Request
            {
                BucketName = bucketName,
                MaxKeys = 1000 // Bạn có thể điều chỉnh số lượng object trả về tối đa
            };

            var response = await acount.acountClient.ListObjectsV2Async(request);

            foreach(S3Object obj in response.S3Objects)
            {
                if (obj.Key.EndsWith("/"))
                {                  
                        DeleteS3Directory(bucketName, obj.Key);
                }
                else
                {
                    // Delete file
                    acount.acountClient.DeleteObject(new DeleteObjectRequest { BucketName = bucketName, Key = obj.Key });
                }
            }

            //delete bucket 
            var deleteRequest = new DeleteBucketRequest{ BucketName = bucketName };
            try
            {
                await acount.acountClient.DeleteBucketAsync(deleteRequest);
                MessageBox.Show($"Da xoa bucket {bucketName}","Thong bao",MessageBoxButtons.OK);
                LoadTreeViewS3();
            }
            catch (AmazonS3Exception ex)
            {
                MessageBox.Show($"Error deleting object '{bucketName}': {ex.Message}");
            }


        }

        public void DeleteS3Directory(string bucketName, string directoryPath)
        {
            AmazonS3Client s3Client = acount.acountClient;

            ListObjectsV2Request request = new ListObjectsV2Request
            {
                BucketName = bucketName,
                Prefix = directoryPath,
                // Delimiter = "/"
            };

            ListObjectsV2Response response;
            do
            {
                response = s3Client.ListObjectsV2(request);

                foreach (S3Object obj in response.S3Objects)
                {
                    if (obj.Key.EndsWith("/"))
                    {
                        // Recursively delete subdirectory
                        if (obj.Key != directoryPath)
                        {
                            DeleteS3Directory(bucketName, obj.Key);

                        }
                        

                    }
                    else
                    {
                        // Delete file
                        s3Client.DeleteObject(new DeleteObjectRequest { BucketName = bucketName, Key = obj.Key });
                    }
                }

                request.ContinuationToken = response.NextContinuationToken;
            }
            while (response.IsTruncated);

            // Delete the directory itself
            s3Client.DeleteObject(new DeleteObjectRequest { BucketName = bucketName, Key = directoryPath });
        }

        private void toolStripButton13_Click(object sender, EventArgs e)
        {
            
        }

        public async void CopyFolderInsideS3Bucket(string source, string destination, string sourceBucketName ,string  desBucketName)
        {
            AmazonS3Client s3Client = acount.acountClient;
            //name of source folder
            string sourceFolderName = GetLastPart(source);
            //create source folder in des folder
            string fullPath = destination + sourceFolderName;
            // Tạo một PutObjectRequest để tạo directory
            PutObjectRequest putRequest = new PutObjectRequest
            {
                BucketName = desBucketName,
                Key = fullPath
            };

            // Tạo directory
            await acount.acountClient.PutObjectAsync(putRequest);

            //
            ListObjectsV2Request listRequest = new ListObjectsV2Request
            {
                BucketName = sourceBucketName,
                Prefix = source,
                // Delimiter = "/"
            };

            ListObjectsV2Response response;

            response = s3Client.ListObjectsV2(listRequest);

            foreach (S3Object obj in response.S3Objects)
            {
                if (obj.Key.EndsWith("/"))
                {
                    // Recursively copy subdirectory
                    if (obj.Key != source)
                    {
                        CopyFolderInsideS3Bucket(obj.Key, destination + sourceFolderName, sourceBucketName,desBucketName);

                    }
                }
                else
                {
                    // copy File
                    string fileName = GetLastPart2(obj.Key);
                    string destinationToCopyFile = destination + sourceFolderName + fileName ;

                    CopyObjectRequest copyRequest = new CopyObjectRequest
                    {
                        SourceBucket = sourceBucketName,
                        SourceKey = source + fileName,
                        DestinationBucket = desBucketName,
                        DestinationKey = destinationToCopyFile
                    };
                    await s3Client.CopyObjectAsync(copyRequest);
                }
            }



        }

        public string GetLastPart(string path)
        {
            // Chia tách đường dẫn thành các phần
            string[] parts = path.Split('/');

            // Lấy phần cuối cùng của đường dẫn
            string lastPart = parts[parts.Length - 2];

            // Trả về phần cuối cùng dạng "d/"
            return $"{lastPart}/";
        }

        public string GetLastPart2(string path)
        {
            // Chia tách đường dẫn thành các phần
            string[] parts = path.Split('/');

            // Lấy phần cuối cùng của đường dẫn
            string lastPart = parts[parts.Length - 1];

            // Trả về phần cuối cùng dạng "d/"
            return lastPart;
        }
    }
}
