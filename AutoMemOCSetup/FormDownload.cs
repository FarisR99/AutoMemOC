using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Windows.Forms;

namespace AutoMemOCSetup
{
    public partial class FormDownload : Form
    {
        private readonly string url;
        private readonly string fileName;
        private readonly string filePath;

        private WebClient webClient = null;
        private List<Action> onDownloadCompleted = new List<Action>();

        public FormDownload(string url, string fileName, string filePath)
        {
            this.url = url;
            this.fileName = fileName;
            this.filePath = filePath;

            InitializeComponent();
        }

        private void FormDownload_Load(object sender, EventArgs e)
        {
            pbDownload.Value = 0;
            lblProgress.Text = "0%";
            lblFileName.Text = fileName;

            webClient = new WebClient();
            webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
            webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;

            webClient.DownloadFileAsync(new Uri(url), $"{filePath}/{fileName}");
        }

        private void FormDownload_FormClosing(object sender, FormClosingEventArgs e)
        {
            webClient.CancelAsync();
            webClient.Dispose();
        }

        private void FormDownload_FormClosed(object sender, FormClosedEventArgs e)
        {
            webClient = null;
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            pbDownload.Value = e.ProgressPercentage;
            lblProgress.Text = e.ProgressPercentage + "%";
        }

        private void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                Close();
                return;
            }
            Close();
            onDownloadCompleted.ForEach(a => a.Invoke());
        }

        public void OnDownloadCompleted(Action action)
        {
            onDownloadCompleted.Add(action);
        }
    }
}
