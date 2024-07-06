using Amazon;
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
using System.Diagnostics;

namespace S3Explorer
{
    public partial class NewDirectory : Form
    {
        string bucketName;
        string key;
        ListViewItem[] items; 
        AmazonS3Client client;
        public NewDirectory(AmazonS3Client client,string bucketName, string key, ListViewItem[] items)
        {
            InitializeComponent();
            this.bucketName = bucketName;
            this.key = key;
            this.items = items;
            this.client = client;
        }

        private async void iconButton1_Click(object sender, EventArgs e)
        {
            string directoryName = textBoxDirectoryName.Text;
            if(directoryName == "")
            {
                MessageBox.Show("Ten thu muc khong hop le");
                return;
            }

            foreach(ListViewItem item in items)
            {
                if(GetListViewItemName(item) == directoryName)
                {
                    MessageBox.Show("Ten thu muc da ton tai");
                    return;
                }
            }
            string fullPath = key + directoryName + "/";
            // Tạo một PutObjectRequest để tạo directory
            PutObjectRequest request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = fullPath
            };

            // Tạo directory
            await client.PutObjectAsync(request);

            MessageBox.Show("Tao moi thanh cong","",MessageBoxButtons.OK);
            this.Close();

        }
        public string GetListViewItemName(ListViewItem item)
        {
            return item.SubItems[0].Text;
        }

        private void textBoxDirectoryName_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
