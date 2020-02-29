using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoMemOCSetup
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnLocate_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog directoryBrowser = new FolderBrowserDialog();
            DialogResult result = directoryBrowser.ShowDialog();
            if (result == DialogResult.OK)
            {
                string selectedDirectory = directoryBrowser.SelectedPath;
                if (!File.Exists(selectedDirectory + "/AutoMemOC.exe"))
                {
                    DialogResult msgBoxResult = MessageBox.Show("Could not locate AutoMemOC.exe", "Invalid directory", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                    if (msgBoxResult == DialogResult.Retry)
                    {
                        btnLocate_Click(sender, null);
                    }
                    return;
                }

                Hide();
                FormSetup setupForm = new FormSetup(selectedDirectory);
                setupForm.FormClosed += (s, args) => Close();
                setupForm.Show();
            }
        }
    }
}
