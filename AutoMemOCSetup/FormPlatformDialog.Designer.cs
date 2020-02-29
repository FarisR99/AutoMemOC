namespace AutoMemOCSetup
{
    partial class FormPlatformDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnZ370 = new System.Windows.Forms.Button();
            this.btnZ390 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnZ370
            // 
            this.btnZ370.Location = new System.Drawing.Point(38, 17);
            this.btnZ370.Name = "btnZ370";
            this.btnZ370.Size = new System.Drawing.Size(70, 40);
            this.btnZ370.TabIndex = 0;
            this.btnZ370.Text = "Z370";
            this.btnZ370.UseVisualStyleBackColor = true;
            this.btnZ370.Click += new System.EventHandler(this.btnZ370_Click);
            // 
            // btnZ390
            // 
            this.btnZ390.Location = new System.Drawing.Point(114, 17);
            this.btnZ390.Name = "btnZ390";
            this.btnZ390.Size = new System.Drawing.Size(70, 40);
            this.btnZ390.TabIndex = 1;
            this.btnZ390.Text = "Z390";
            this.btnZ390.UseVisualStyleBackColor = true;
            this.btnZ390.Click += new System.EventHandler(this.btnZ390_Click);
            // 
            // FormPlatformDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(223, 74);
            this.Controls.Add(this.btnZ390);
            this.Controls.Add(this.btnZ370);
            this.Name = "FormPlatformDialog";
            this.Text = "Platform";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnZ370;
        private System.Windows.Forms.Button btnZ390;
    }
}