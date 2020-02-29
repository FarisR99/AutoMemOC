
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoMemOCSetup
{
    public partial class FormSetup : Form
    {
        private readonly string appPath;

        private string testPath = null;
        private string tweakerPath = null;

        private string internalTM5Path = null;
        private string internalMTIPath = null;

        public string TM5Path
        {
            get
            {
                return internalTM5Path;
            }
            set
            {
                internalTM5Path = value;
                cbTM5.Checked = internalTM5Path != null;
            }
        }
        public string MemTweakItPath
        {
            get
            {
                return internalMTIPath;
            }
            set
            {
                internalMTIPath = value;
                cbMTI.Checked = internalMTIPath != null;
            }
        }

        public FormSetup(string path)
        {
            appPath = path;

            InitializeComponent();
        }

        private void FormSetup_Load(object sender, EventArgs e)
        {
            try
            {
                SimpleProperties simpleProperties = new SimpleProperties(appPath + "/config.txt");
                testPath = simpleProperties.get("MemTestPath");
                tweakerPath = simpleProperties.get("TweakerPath");
            }
            catch (Exception ex) {
                MessageBox.Show("Failed to load '" + appPath + "/config.txt'", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.Error.WriteLine(ex);
                Application.Exit();
                return;
            }
            Console.Out.WriteLine("Current memory test path: " + (testPath != null ? testPath : "Unknown"));
            Console.Out.WriteLine("Current tweaker path: " + (tweakerPath != null ? tweakerPath : "Unknown"));

            cbTM5.Checked = false;
            cbMTI.Checked = false;

            if (testPath != null)
            {
                if (File.Exists(testPath + "/TM5.exe"))
                {
                    TM5Path = testPath;
                }
            }
            if (tweakerPath != null)
            {
                if (File.Exists(tweakerPath + "/MemTweakIt.exe"))
                {
                    MemTweakItPath = testPath;
                }
            }
        }

        private void btnLocateTM5_Click(object sender, EventArgs e)
        {
            string locatedPath = locate("TM5.exe", "memory test", "MemTestPath");
            if (locatedPath != null)
            {
                testPath = locatedPath;
                TM5Path = locatedPath;
            }
        }

        private void btnInstallTM5_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog destinationDialog = new FolderBrowserDialog();
            destinationDialog.Description = "Destination directory";
            DialogResult result = destinationDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string destinationPath = destinationDialog.SelectedPath + "/TM5";
                Directory.CreateDirectory(destinationPath);
                FormDownload formDownload = new FormDownload("https://kingfaris.co.uk/downloads/tm5", "tm5.zip", destinationPath);
                formDownload.OnDownloadCompleted(() =>
                {
                    ZipFile.ExtractToDirectory($"{destinationPath}/tm5.zip", destinationPath);
                    try
                    {
                        File.Delete($"{destinationPath}/tm5.zip");
                    }
                    catch (Exception) { }
                    MessageBox.Show("Downloaded!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    internalLocate("memory tester", "MemTestPath", destinationPath);
                    TM5Path = destinationPath;
                });
                formDownload.ShowDialog();
            }
        }

        private void btnSetupTM5_Click(object sender, EventArgs e)
        {
            if (TM5Path == null)
            {
                MessageBox.Show("Please download or locate TM5 before setting it up.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (File.Exists($"{TM5Path}/bin/MT.cfg"))
            {
                File.Delete($"{TM5Path}/bin/MT.cfg");
            }
            File.Copy("Resources/extreme@anta777.cfg", $"{TM5Path}/bin/MT.cfg");

            MessageBox.Show("Successfully set up TM5!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnLocateMTI_Click(object sender, EventArgs e)
        {
            string locatedPath = locate("MemTweakIt.exe", "tweaker", "TweakerPath");
            if (locatedPath != null)
            {
                tweakerPath = locatedPath;
                MemTweakItPath = locatedPath;
            }
        }

        private void btnInstallMTI_Click(object sender, EventArgs e)
        {
            FormPlatformDialog platformDialog = new FormPlatformDialog();
            DialogResult result = platformDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                FormPlatformDialog.Platform platform = platformDialog.SelectedPlatform;
                if (platform == null)
                {
                    MessageBox.Show("Invalid platform selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                FolderBrowserDialog destinationDialog = new FolderBrowserDialog();
                destinationDialog.Description = "Setup destination directory";
                result = destinationDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string destinationPath = destinationDialog.SelectedPath + "/MemTweakIt_Setup";
                    Directory.CreateDirectory(destinationPath);
                    FormDownload formDownload = new FormDownload("https://kingfaris.co.uk/downloads/memtweakit/" + platform.Name.ToLower(), "memtweakit.zip", destinationPath);
                    formDownload.OnDownloadCompleted(() =>
                    {
                        ZipFile.ExtractToDirectory($"{destinationPath}/memtweakit.zip", destinationPath);
                        try
                        {
                            File.Delete($"{destinationPath}/memtweakit.zip");
                        }
                        catch (Exception) { }
                        
                        if (File.Exists($"{destinationPath}/Setup.exe"))
                        {
                            Process setupProcess = Process.Start($"{destinationPath}/Setup.exe");
                            setupProcess.WaitForExit();

                            string locatedPath = locate("MemTweakIt.exe", "tweaker", "MemTweakPath");
                            if (locatedPath != null)
                            {
                                tweakerPath = locatedPath;
                                MemTweakItPath = locatedPath;
                            }
                        } else
                        {
                            MessageBox.Show("Failed to locate Setup.exe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        Directory.Delete(destinationPath, true);
                    });
                    formDownload.ShowDialog();
                }
            }
        }

        private string locate(string exeName, string description, string configKey)
        {
            FolderBrowserDialog browserDialog = new FolderBrowserDialog();
            browserDialog.Description = $"Select the directory containing {exeName}";
            DialogResult result = browserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string selectedPath = browserDialog.SelectedPath;
                if (!File.Exists($"{selectedPath}/{exeName}"))
                {
                    MessageBox.Show($"Could not locate {exeName}", "Invalid directory", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }

                if (internalLocate(description, configKey, selectedPath))
                {
                    return selectedPath;
                } else
                {
                    return null;
                }
            }
            return null;
        }

        private bool internalLocate(string description, string configKey, string selectedPath)
        {
            try
            {
                SimpleProperties simpleProperties = new SimpleProperties(appPath + "/config.txt");
                simpleProperties.set(configKey, selectedPath);
                simpleProperties.save();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save '" + appPath + "/config.txt'", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.Error.WriteLine(ex);
                return false;
            }

            MessageBox.Show($"Successfully updated the {description} location!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return true;
        }
    }
}
