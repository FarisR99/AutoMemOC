namespace AutoMemOCSetup
{
    partial class FormSetup
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
            this.components = new System.ComponentModel.Container();
            this.btnLocateTM5 = new System.Windows.Forms.Button();
            this.btnInstallTM5 = new System.Windows.Forms.Button();
            this.btnSetupTM5 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlTestMem5 = new System.Windows.Forms.Panel();
            this.cbTM5 = new System.Windows.Forms.CheckBox();
            this.pnlMTI = new System.Windows.Forms.Panel();
            this.cbMTI = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnLocateMTI = new System.Windows.Forms.Button();
            this.btnInstallMTI = new System.Windows.Forms.Button();
            this.tipLocate = new System.Windows.Forms.ToolTip(this.components);
            this.pnlTestMem5.SuspendLayout();
            this.pnlMTI.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnLocateTM5
            // 
            this.btnLocateTM5.Location = new System.Drawing.Point(24, 70);
            this.btnLocateTM5.Name = "btnLocateTM5";
            this.btnLocateTM5.Size = new System.Drawing.Size(95, 25);
            this.btnLocateTM5.TabIndex = 0;
            this.btnLocateTM5.Text = "Locate TM5";
            this.tipLocate.SetToolTip(this.btnLocateTM5, "Locate TM5 if you already have it downloaded.");
            this.btnLocateTM5.UseVisualStyleBackColor = true;
            this.btnLocateTM5.Click += new System.EventHandler(this.btnLocateTM5_Click);
            // 
            // btnInstallTM5
            // 
            this.btnInstallTM5.Location = new System.Drawing.Point(24, 99);
            this.btnInstallTM5.Name = "btnInstallTM5";
            this.btnInstallTM5.Size = new System.Drawing.Size(95, 25);
            this.btnInstallTM5.TabIndex = 1;
            this.btnInstallTM5.Text = "Install TM5";
            this.btnInstallTM5.UseVisualStyleBackColor = true;
            this.btnInstallTM5.Click += new System.EventHandler(this.btnInstallTM5_Click);
            // 
            // btnSetupTM5
            // 
            this.btnSetupTM5.Location = new System.Drawing.Point(24, 128);
            this.btnSetupTM5.Name = "btnSetupTM5";
            this.btnSetupTM5.Size = new System.Drawing.Size(95, 25);
            this.btnSetupTM5.TabIndex = 2;
            this.btnSetupTM5.Text = "Set up TM5";
            this.btnSetupTM5.UseVisualStyleBackColor = true;
            this.btnSetupTM5.Click += new System.EventHandler(this.btnSetupTM5_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(28, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "TestMem5";
            // 
            // pnlTestMem5
            // 
            this.pnlTestMem5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlTestMem5.Controls.Add(this.cbTM5);
            this.pnlTestMem5.Controls.Add(this.label1);
            this.pnlTestMem5.Controls.Add(this.btnLocateTM5);
            this.pnlTestMem5.Controls.Add(this.btnSetupTM5);
            this.pnlTestMem5.Controls.Add(this.btnInstallTM5);
            this.pnlTestMem5.Location = new System.Drawing.Point(15, 18);
            this.pnlTestMem5.Name = "pnlTestMem5";
            this.pnlTestMem5.Size = new System.Drawing.Size(145, 198);
            this.pnlTestMem5.TabIndex = 4;
            // 
            // cbTM5
            // 
            this.cbTM5.AutoSize = true;
            this.cbTM5.Enabled = false;
            this.cbTM5.Location = new System.Drawing.Point(99, 21);
            this.cbTM5.Name = "cbTM5";
            this.cbTM5.Size = new System.Drawing.Size(15, 14);
            this.cbTM5.TabIndex = 4;
            this.cbTM5.UseVisualStyleBackColor = true;
            // 
            // pnlMTI
            // 
            this.pnlMTI.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlMTI.Controls.Add(this.cbMTI);
            this.pnlMTI.Controls.Add(this.label2);
            this.pnlMTI.Controls.Add(this.btnLocateMTI);
            this.pnlMTI.Controls.Add(this.btnInstallMTI);
            this.pnlMTI.Location = new System.Drawing.Point(166, 18);
            this.pnlMTI.Name = "pnlMTI";
            this.pnlMTI.Size = new System.Drawing.Size(145, 198);
            this.pnlMTI.TabIndex = 5;
            // 
            // cbMTI
            // 
            this.cbMTI.AutoSize = true;
            this.cbMTI.Enabled = false;
            this.cbMTI.Location = new System.Drawing.Point(106, 21);
            this.cbMTI.Name = "cbMTI";
            this.cbMTI.Size = new System.Drawing.Size(15, 14);
            this.cbMTI.TabIndex = 5;
            this.cbMTI.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(21, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "MemTweakIt";
            // 
            // btnLocateMTI
            // 
            this.btnLocateMTI.Location = new System.Drawing.Point(24, 57);
            this.btnLocateMTI.Name = "btnLocateMTI";
            this.btnLocateMTI.Size = new System.Drawing.Size(95, 50);
            this.btnLocateMTI.TabIndex = 0;
            this.btnLocateMTI.Text = "Locate MemTweakIt";
            this.tipLocate.SetToolTip(this.btnLocateMTI, "Locate MemTweakIt if you already have it downloaded.");
            this.btnLocateMTI.UseVisualStyleBackColor = true;
            this.btnLocateMTI.Click += new System.EventHandler(this.btnLocateMTI_Click);
            // 
            // btnInstallMTI
            // 
            this.btnInstallMTI.Location = new System.Drawing.Point(24, 115);
            this.btnInstallMTI.Name = "btnInstallMTI";
            this.btnInstallMTI.Size = new System.Drawing.Size(95, 50);
            this.btnInstallMTI.TabIndex = 1;
            this.btnInstallMTI.Text = "Install MemTweakIt";
            this.btnInstallMTI.UseVisualStyleBackColor = true;
            this.btnInstallMTI.Click += new System.EventHandler(this.btnInstallMTI_Click);
            // 
            // tipLocate
            // 
            this.tipLocate.ToolTipTitle = "Locate";
            // 
            // FormSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(326, 234);
            this.Controls.Add(this.pnlMTI);
            this.Controls.Add(this.pnlTestMem5);
            this.Name = "FormSetup";
            this.Text = "AutoMemOC Setup";
            this.Load += new System.EventHandler(this.FormSetup_Load);
            this.pnlTestMem5.ResumeLayout(false);
            this.pnlTestMem5.PerformLayout();
            this.pnlMTI.ResumeLayout(false);
            this.pnlMTI.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnLocateTM5;
        private System.Windows.Forms.Button btnInstallTM5;
        private System.Windows.Forms.Button btnSetupTM5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnlTestMem5;
        private System.Windows.Forms.Panel pnlMTI;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnLocateMTI;
        private System.Windows.Forms.Button btnInstallMTI;
        private System.Windows.Forms.ToolTip tipLocate;
        private System.Windows.Forms.CheckBox cbTM5;
        private System.Windows.Forms.CheckBox cbMTI;
    }
}