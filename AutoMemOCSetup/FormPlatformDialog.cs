using System;
using System.Windows.Forms;

namespace AutoMemOCSetup
{
    public partial class FormPlatformDialog : Form
    {
        private Platform chosenPlatform = null;

        public Platform SelectedPlatform
        {
            get { return chosenPlatform; }
            private set { }
        }

        public FormPlatformDialog()
        {
            InitializeComponent();
        }

        private void btnZ370_Click(object sender, EventArgs e)
        {
            setPlatform(Platform.Z370);
        }

        private void btnZ390_Click(object sender, EventArgs e)
        {
            setPlatform(Platform.Z390);
        }

        private void setPlatform(Platform platform)
        {
            chosenPlatform = platform;
            DialogResult = DialogResult.OK;
            Close();
        }

        public class Platform
        {
            public static readonly Platform Z370 = new Platform("Z370");
            public static readonly Platform Z390 = new Platform("Z390");

            public readonly string Name;

            private Platform(string name)
            {
                Name = name;
            }
        }
    }
}
