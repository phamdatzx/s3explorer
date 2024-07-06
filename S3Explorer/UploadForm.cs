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

namespace S3Explorer
{
    internal partial class UploadForm : Form
    {
        string uploadPath;
        AmazonS3Client s3Client;
        ListViewItem[] uploadItems;
        public UploadForm(ListViewItem[] uploadItems,S3Acount acount,string uploadPath)
        {
            InitializeComponent();
            this.s3Client = acount.acountClient;
            this.uploadPath = uploadPath;
            this.uploadItems = uploadItems;
            textBoxLocalPath.Text = uploadPath;
            int numberOfObject = uploadItems.Length;
            label1.Text = "Upload " + numberOfObject + " objects to s3 :";
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.Close();
            Uploading uploading = new Uploading(uploadItems,s3Client,uploadPath);
            uploading.Show();
        }
    }
}
