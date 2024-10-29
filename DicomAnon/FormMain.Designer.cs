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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.buttonSelectFolder = new System.Windows.Forms.Button();
            this.labelFolder = new System.Windows.Forms.Label();
            this.checkBoxIncludeSubfolders = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkKeepFilenames = new System.Windows.Forms.CheckBox();
            this.labelWarnings = new System.Windows.Forms.Label();
            this.labelImages = new System.Windows.Forms.Label();
            this.buttonAnon = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.richTextBoxOut = new System.Windows.Forms.RichTextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textCropBottom = new System.Windows.Forms.TextBox();
            this.textCropRight = new System.Windows.Forms.TextBox();
            this.textCropLeft = new System.Windows.Forms.TextBox();
            this.textCropTop = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonSelectFolder
            // 
            this.buttonSelectFolder.Location = new System.Drawing.Point(6, 19);
            this.buttonSelectFolder.Name = "buttonSelectFolder";
            this.buttonSelectFolder.Size = new System.Drawing.Size(202, 23);
            this.buttonSelectFolder.TabIndex = 0;
            this.buttonSelectFolder.Text = "Select Folder...";
            this.buttonSelectFolder.UseVisualStyleBackColor = true;
            this.buttonSelectFolder.Click += new System.EventHandler(this.ButtonSelectFolder_Click);
            // 
            // labelFolder
            // 
            this.labelFolder.AutoSize = true;
            this.labelFolder.Location = new System.Drawing.Point(6, 69);
            this.labelFolder.Name = "labelFolder";
            this.labelFolder.Size = new System.Drawing.Size(111, 13);
            this.labelFolder.TabIndex = 1;
            this.labelFolder.Text = "Folder selected: None";
            // 
            // checkBoxIncludeSubfolders
            // 
            this.checkBoxIncludeSubfolders.AutoSize = true;
            this.checkBoxIncludeSubfolders.Checked = true;
            this.checkBoxIncludeSubfolders.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxIncludeSubfolders.Location = new System.Drawing.Point(214, 23);
            this.checkBoxIncludeSubfolders.Name = "checkBoxIncludeSubfolders";
            this.checkBoxIncludeSubfolders.Size = new System.Drawing.Size(114, 17);
            this.checkBoxIncludeSubfolders.TabIndex = 2;
            this.checkBoxIncludeSubfolders.Text = "Include Subfolders";
            this.checkBoxIncludeSubfolders.UseVisualStyleBackColor = true;
            this.checkBoxIncludeSubfolders.CheckedChanged += new System.EventHandler(this.CheckBoxIncludeSubfolders_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkKeepFilenames);
            this.groupBox1.Controls.Add(this.labelWarnings);
            this.groupBox1.Controls.Add(this.labelImages);
            this.groupBox1.Controls.Add(this.checkBoxIncludeSubfolders);
            this.groupBox1.Controls.Add(this.buttonSelectFolder);
            this.groupBox1.Controls.Add(this.labelFolder);
            this.groupBox1.Location = new System.Drawing.Point(12, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(335, 202);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "File Details";
            // 
            // chkKeepFilenames
            // 
            this.chkKeepFilenames.AutoSize = true;
            this.chkKeepFilenames.Location = new System.Drawing.Point(214, 44);
            this.chkKeepFilenames.Name = "chkKeepFilenames";
            this.chkKeepFilenames.Size = new System.Drawing.Size(101, 17);
            this.chkKeepFilenames.TabIndex = 5;
            this.chkKeepFilenames.Text = "Keep Filenames";
            this.chkKeepFilenames.UseVisualStyleBackColor = true;
            // 
            // labelWarnings
            // 
            this.labelWarnings.AutoSize = true;
            this.labelWarnings.Location = new System.Drawing.Point(6, 113);
            this.labelWarnings.Name = "labelWarnings";
            this.labelWarnings.Size = new System.Drawing.Size(55, 13);
            this.labelWarnings.TabIndex = 4;
            this.labelWarnings.Text = "Warnings:";
            // 
            // labelImages
            // 
            this.labelImages.AutoSize = true;
            this.labelImages.Location = new System.Drawing.Point(6, 91);
            this.labelImages.Name = "labelImages";
            this.labelImages.Size = new System.Drawing.Size(139, 13);
            this.labelImages.TabIndex = 3;
            this.labelImages.Text = "Images (*.dcm) found: None";
            // 
            // buttonAnon
            // 
            this.buttonAnon.Enabled = false;
            this.buttonAnon.Location = new System.Drawing.Point(12, 226);
            this.buttonAnon.Name = "buttonAnon";
            this.buttonAnon.Size = new System.Drawing.Size(194, 37);
            this.buttonAnon.TabIndex = 4;
            this.buttonAnon.Text = "Anonymise";
            this.buttonAnon.UseVisualStyleBackColor = true;
            this.buttonAnon.Click += new System.EventHandler(this.ButtonAnon_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 269);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(480, 23);
            this.progressBar.TabIndex = 5;
            // 
            // richTextBoxOut
            // 
            this.richTextBoxOut.Location = new System.Drawing.Point(12, 299);
            this.richTextBoxOut.Name = "richTextBoxOut";
            this.richTextBoxOut.ReadOnly = true;
            this.richTextBoxOut.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.richTextBoxOut.Size = new System.Drawing.Size(479, 139);
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
            this.groupBox2.Location = new System.Drawing.Point(352, 7);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(145, 200);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Crop";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(47, 112);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Bottom (px)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(81, 66);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Right (px)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 66);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Left (px)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(47, 27);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Top (px)";
            // 
            // textCropBottom
            // 
            this.textCropBottom.Location = new System.Drawing.Point(47, 128);
            this.textCropBottom.Margin = new System.Windows.Forms.Padding(2);
            this.textCropBottom.Name = "textCropBottom";
            this.textCropBottom.Size = new System.Drawing.Size(52, 20);
            this.textCropBottom.TabIndex = 3;
            this.textCropBottom.Text = "0";
            // 
            // textCropRight
            // 
            this.textCropRight.Location = new System.Drawing.Point(81, 84);
            this.textCropRight.Margin = new System.Windows.Forms.Padding(2);
            this.textCropRight.Name = "textCropRight";
            this.textCropRight.Size = new System.Drawing.Size(52, 20);
            this.textCropRight.TabIndex = 2;
            this.textCropRight.Text = "0";
            // 
            // textCropLeft
            // 
            this.textCropLeft.Location = new System.Drawing.Point(11, 84);
            this.textCropLeft.Margin = new System.Windows.Forms.Padding(2);
            this.textCropLeft.Name = "textCropLeft";
            this.textCropLeft.Size = new System.Drawing.Size(52, 20);
            this.textCropLeft.TabIndex = 1;
            this.textCropLeft.Text = "0";
            // 
            // textCropTop
            // 
            this.textCropTop.Location = new System.Drawing.Point(47, 43);
            this.textCropTop.Margin = new System.Windows.Forms.Padding(2);
            this.textCropTop.Name = "textCropTop";
            this.textCropTop.Size = new System.Drawing.Size(52, 20);
            this.textCropTop.TabIndex = 0;
            this.textCropTop.Text = "0";
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(212, 226);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(131, 37);
            this.button1.TabIndex = 8;
            this.button1.Text = "Get Data";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(349, 226);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(142, 37);
            this.button2.TabIndex = 9;
            this.button2.Text = "Recode PatientID";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(503, 450);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.richTextBoxOut);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.buttonAnon);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
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
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox chkKeepFilenames;
        private System.Windows.Forms.Button button2;
    }
}

