namespace MeteorInkJet.SdCardTestApp {
    partial class ImageFileControl {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.statusStripImage = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusFilename = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusImageDimensions = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pictureBoxPrintData = new System.Windows.Forms.PictureBox();
            this.openFileDialogReadImage = new System.Windows.Forms.OpenFileDialog();
            this.statusStripImage.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPrintData)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStripImage
            // 
            this.statusStripImage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusFilename,
            this.toolStripStatusImageDimensions});
            this.statusStripImage.Location = new System.Drawing.Point(0, 415);
            this.statusStripImage.Name = "statusStripImage";
            this.statusStripImage.Size = new System.Drawing.Size(535, 22);
            this.statusStripImage.SizingGrip = false;
            this.statusStripImage.TabIndex = 9;
            this.statusStripImage.Text = "statusStripImage";
            // 
            // toolStripStatusFilename
            // 
            this.toolStripStatusFilename.Name = "toolStripStatusFilename";
            this.toolStripStatusFilename.Size = new System.Drawing.Size(520, 17);
            this.toolStripStatusFilename.Spring = true;
            this.toolStripStatusFilename.Text = "No image loaded";
            this.toolStripStatusFilename.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusImageDimensions
            // 
            this.toolStripStatusImageDimensions.Name = "toolStripStatusImageDimensions";
            this.toolStripStatusImageDimensions.Size = new System.Drawing.Size(0, 17);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.pictureBoxPrintData);
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(535, 412);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Preview";
            // 
            // pictureBoxPrintData
            // 
            this.pictureBoxPrintData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxPrintData.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.pictureBoxPrintData.Location = new System.Drawing.Point(6, 19);
            this.pictureBoxPrintData.Name = "pictureBoxPrintData";
            this.pictureBoxPrintData.Size = new System.Drawing.Size(523, 387);
            this.pictureBoxPrintData.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxPrintData.TabIndex = 2;
            this.pictureBoxPrintData.TabStop = false;
            // 
            // openFileDialogReadImage
            // 
            this.openFileDialogReadImage.DefaultExt = "bmp";
            this.openFileDialogReadImage.Filter = "Bitmap files|*.bmp|JPEG files|*.jpg|TIFF files|*.tif";
            this.openFileDialogReadImage.Title = "Please select the print image ...";
            // 
            // ImageFileControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.statusStripImage);
            this.Name = "ImageFileControl";
            this.Size = new System.Drawing.Size(535, 437);
            this.statusStripImage.ResumeLayout(false);
            this.statusStripImage.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPrintData)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStripImage;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusFilename;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusImageDimensions;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox pictureBoxPrintData;
        private System.Windows.Forms.OpenFileDialog openFileDialogReadImage;
    }
}
