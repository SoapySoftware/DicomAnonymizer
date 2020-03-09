namespace DicomAnon
{
    partial class FormMain
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
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.buttonSelectFolder = new System.Windows.Forms.Button();
            this.labelFolder = new System.Windows.Forms.Label();
            this.checkBoxIncludeSubfolders = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelWarnings = new System.Windows.Forms.Label();
            this.labelImages = new System.Windows.Forms.Label();
            this.buttonAnon = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.richTextBoxOut = new System.Windows.Forms.RichTextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textCropTop = new System.Windows.Forms.TextBox();
            this.textCropLeft = new System.Windows.Forms.TextBox();
            this.textCropRight = new System.Windows.Forms.TextBox();
            this.textCropBottom = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonSelectFolder
            // 
            this.buttonSelectFolder.Location = new System.Drawing.Point(12, 37);
            this.buttonSelectFolder.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.buttonSelectFolder.Name = "buttonSelectFolder";
            this.buttonSelectFolder.Size = new System.Drawing.Size(404, 44);
            this.buttonSelectFolder.TabIndex = 0;
            this.buttonSelectFolder.Text = "Select Folder...";
            this.buttonSelectFolder.UseVisualStyleBackColor = true;
            this.buttonSelectFolder.Click += new System.EventHandler(this.ButtonSelectFolder_Click);
            // 
            // labelFolder
            // 
            this.labelFolder.AutoSize = true;
            this.labelFolder.Location = new System.Drawing.Point(12, 87);
            this.labelFolder.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelFolder.Name = "labelFolder";
            this.labelFolder.Size = new System.Drawing.Size(223, 25);
            this.labelFolder.TabIndex = 1;
            this.labelFolder.Text = "Folder selected: None";
            // 
            // checkBoxIncludeSubfolders
            // 
            this.checkBoxIncludeSubfolders.AutoSize = true;
            this.checkBoxIncludeSubfolders.Checked = true;
            this.checkBoxIncludeSubfolders.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxIncludeSubfolders.Location = new System.Drawing.Point(428, 44);
            this.checkBoxIncludeSubfolders.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.checkBoxIncludeSubfolders.Name = "checkBoxIncludeSubfolders";
            this.checkBoxIncludeSubfolders.Size = new System.Drawing.Size(222, 29);
            this.checkBoxIncludeSubfolders.TabIndex = 2;
            this.checkBoxIncludeSubfolders.Text = "Include Subfolders";
            this.checkBoxIncludeSubfolders.UseVisualStyleBackColor = true;
            this.checkBoxIncludeSubfolders.CheckedChanged += new System.EventHandler(this.CheckBoxIncludeSubfolders_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelWarnings);
            this.groupBox1.Controls.Add(this.labelImages);
            this.groupBox1.Controls.Add(this.checkBoxIncludeSubfolders);
            this.groupBox1.Controls.Add(this.buttonSelectFolder);
            this.groupBox1.Controls.Add(this.labelFolder);
            this.groupBox1.Location = new System.Drawing.Point(24, 10);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox1.Size = new System.Drawing.Size(670, 388);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "File Details";
            // 
            // labelWarnings
            // 
            this.labelWarnings.AutoSize = true;
            this.labelWarnings.Location = new System.Drawing.Point(12, 171);
            this.labelWarnings.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelWarnings.Name = "labelWarnings";
            this.labelWarnings.Size = new System.Drawing.Size(109, 25);
            this.labelWarnings.TabIndex = 4;
            this.labelWarnings.Text = "Warnings:";
            // 
            // labelImages
            // 
            this.labelImages.AutoSize = true;
            this.labelImages.Location = new System.Drawing.Point(12, 129);
            this.labelImages.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelImages.Name = "labelImages";
            this.labelImages.Size = new System.Drawing.Size(278, 25);
            this.labelImages.TabIndex = 3;
            this.labelImages.Text = "Images (*.dcm) found: None";
            // 
            // buttonAnon
            // 
            this.buttonAnon.Enabled = false;
            this.buttonAnon.Location = new System.Drawing.Point(24, 435);
            this.buttonAnon.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.buttonAnon.Name = "buttonAnon";
            this.buttonAnon.Size = new System.Drawing.Size(960, 71);
            this.buttonAnon.TabIndex = 4;
            this.buttonAnon.Text = "Anonymise";
            this.buttonAnon.UseVisualStyleBackColor = true;
            this.buttonAnon.Click += new System.EventHandler(this.ButtonAnon_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(24, 517);
            this.progressBar.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(960, 44);
            this.progressBar.TabIndex = 5;
            // 
            // richTextBoxOut
            // 
            this.richTextBoxOut.Location = new System.Drawing.Point(24, 575);
            this.richTextBoxOut.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.richTextBoxOut.Name = "richTextBoxOut";
            this.richTextBoxOut.ReadOnly = true;
            this.richTextBoxOut.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.richTextBoxOut.Size = new System.Drawing.Size(954, 264);
            this.richTextBoxOut.TabIndex = 6;
            this.richTextBoxOut.Text = "";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.textCropBottom);
            this.groupBox2.Controls.Add(this.textCropRight);
            this.groupBox2.Controls.Add(this.textCropLeft);
            this.groupBox2.Controls.Add(this.textCropTop);
            this.groupBox2.Location = new System.Drawing.Point(704, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(290, 385);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Crop";
            // 
            // textCropTop
            // 
            this.textCropTop.Location = new System.Drawing.Point(94, 83);
            this.textCropTop.Name = "textCropTop";
            this.textCropTop.Size = new System.Drawing.Size(100, 31);
            this.textCropTop.TabIndex = 0;
            this.textCropTop.Text = "0";
            // 
            // textCropLeft
            // 
            this.textCropLeft.Location = new System.Drawing.Point(22, 162);
            this.textCropLeft.Name = "textCropLeft";
            this.textCropLeft.Size = new System.Drawing.Size(100, 31);
            this.textCropLeft.TabIndex = 1;
            this.textCropLeft.Text = "0";
            // 
            // textCropRight
            // 
            this.textCropRight.Location = new System.Drawing.Point(162, 162);
            this.textCropRight.Name = "textCropRight";
            this.textCropRight.Size = new System.Drawing.Size(100, 31);
            this.textCropRight.TabIndex = 2;
            this.textCropRight.Text = "0";
            // 
            // textCropBottom
            // 
            this.textCropBottom.Location = new System.Drawing.Point(94, 246);
            this.textCropBottom.Name = "textCropBottom";
            this.textCropBottom.Size = new System.Drawing.Size(100, 31);
            this.textCropBottom.TabIndex = 3;
            this.textCropBottom.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(94, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 25);
            this.label1.TabIndex = 4;
            this.label1.Text = "Top (px)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 126);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 25);
            this.label2.TabIndex = 5;
            this.label2.Text = "Left (px)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(162, 126);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 25);
            this.label3.TabIndex = 6;
            this.label3.Text = "Right (px)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(94, 215);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(122, 25);
            this.label4.TabIndex = 7;
            this.label4.Text = "Bottom (px)";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1006, 865);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.richTextBoxOut);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.buttonAnon);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "FormMain";
            this.Text = "Dicom Anonymiser";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button buttonSelectFolder;
        private System.Windows.Forms.Label labelFolder;
        private System.Windows.Forms.CheckBox checkBoxIncludeSubfolders;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelWarnings;
        private System.Windows.Forms.Label labelImages;
        private System.Windows.Forms.Button buttonAnon;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.RichTextBox richTextBoxOut;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textCropBottom;
        private System.Windows.Forms.TextBox textCropRight;
        private System.Windows.Forms.TextBox textCropLeft;
        private System.Windows.Forms.TextBox textCropTop;
    }
}

