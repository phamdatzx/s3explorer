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
    public partial class LoginForm : Form
    {
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public LoginForm()
        {
            InitializeComponent();
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.AccessKey = acessKeyTextBox.Text;
            this.SecretKey = secretKeyTextBox.Text;
            Close(); // Close the dialog
        }
    }
}
