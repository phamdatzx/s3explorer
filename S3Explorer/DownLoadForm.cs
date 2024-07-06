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
    internal partial class DownLoadForm : Form
    {
        string downloadPath;
        AmazonS3Client s3Client;
        ListViewItem[] downloadItems;
        public DownLoadForm(ListViewItem[] downloadItems,S3Acount acount,string localPath)
        {
            InitializeComponent();
            this.downloadItems = downloadItems;
            downloadPath = localPath;
            textBoxLocalPath.Text = localPath;
            int numberOfObject = downloadItems.Length;
            label1.Text = "Download "  + numberOfObject + " objects to local :";
            s3Client = acount.acountClient;
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                downloadPath = dialog.SelectedPath;
                textBoxLocalPath.Text = downloadPath;
            }
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            this.Close();
            Downloading downloading = new Downloading(downloadItems,s3Client,downloadPath);
            downloading.Show();
        }
    }
}
