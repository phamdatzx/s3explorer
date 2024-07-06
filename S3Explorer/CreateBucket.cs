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
    public partial class CreateBucket : Form
    {
        AmazonS3Client client;
        public CreateBucket(AmazonS3Client client)
        {
            InitializeComponent();
            this.client = client;
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            // Tên của bucket mới
            string bucketName = textBoxBucketName.Text;
            try
            {
                

                PutBucketRequest request = new PutBucketRequest
                {
                    BucketName = bucketName,
                    BucketRegion = S3Region.APSoutheast1,         // set region
                   
                };

                // Issue call
                PutBucketResponse response = client.PutBucket(request);
                MessageBox.Show("Tao bucket moi thanh cong");
                this.Close();
            }
            catch (AmazonS3Exception ex)
            {
                // Xử lý các exception liên quan đến Amazon S3
                if (ex.ErrorCode.Equals("BucketAlreadyOwnedByYou", StringComparison.InvariantCultureIgnoreCase))
                {
                    MessageBox.Show($"Bucket '{bucketName}' đã tồn tại và được sở hữu bởi bạn.");
                }
                else if (ex.ErrorCode.Equals("BucketAlreadyExists", StringComparison.InvariantCultureIgnoreCase))
                {
                    MessageBox.Show($"Bucket '{bucketName}' đã tồn tại và được sở hữu bởi người khác.");
                }
                else
                {
                    MessageBox.Show($"Đã xảy ra lỗi khi tạo bucket: {ex.ErrorCode}");
                }
            }
            catch (Exception ex)
            {
                // Xử lý các exception khác
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}");
            }


        }
    }
}
