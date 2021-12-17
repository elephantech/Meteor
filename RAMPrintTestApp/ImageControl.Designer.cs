namespace Ttp.Meteor.RAMPrintTestApp
{
    partial class ImageControl
    {
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
            this.pictureBoxPrintData = new System.Windows.Forms.PictureBox();
            this.buttonSelectFile = new System.Windows.Forms.Button();
            this.buttonLoadImage = new System.Windows.Forms.Button();
            this.buttonPrint = new System.Windows.Forms.Button();
            this.numericUpDownCopies = new System.Windows.Forms.NumericUpDown();
            this.labelCopies = new System.Windows.Forms.Label();
            this.openFileDialogReadImage = new System.Windows.Forms.OpenFileDialog();
            this.statusStripImage = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusFilename = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusImageDimensions = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.radioButtonForward = new System.Windows.Forms.RadioButton();
            this.radioButtonReverse = new System.Windows.Forms.RadioButton();
            this.groupBoxPrint = new System.Windows.Forms.GroupBox();
            this.labelDocType = new System.Windows.Forms.Label();
            this.comboBoxDocType = new System.Windows.Forms.ComboBox();
            this.numericUpDownXStart = new System.Windows.Forms.NumericUpDown();
            this.labelXStart = new System.Windows.Forms.Label();
            this.labelXOffset = new System.Windows.Forms.Label();
            this.numericUpDownXOffset = new System.Windows.Forms.NumericUpDown();
            this.groupBoxLoad = new System.Windows.Forms.GroupBox();
            this.numericUpDownYTop = new System.Windows.Forms.NumericUpDown();
            this.labelYTop = new System.Windows.Forms.Label();
            this.checkBoxCircular = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.numericUpDownPcc = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPrintData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCopies)).BeginInit();
            this.statusStripImage.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBoxPrint.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownXStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownXOffset)).BeginInit();
            this.groupBoxLoad.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownYTop)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPcc)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxPrintData
            // 
            this.pictureBoxPrintData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxPrintData.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.pictureBoxPrintData.Location = new System.Drawing.Point(6, 19);
            this.pictureBoxPrintData.Name = "pictureBoxPrintData";
            this.pictureBoxPrintData.Size = new System.Drawing.Size(540, 269);
            this.pictureBoxPrintData.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxPrintData.TabIndex = 2;
            this.pictureBoxPrintData.TabStop = false;
            // 
            // buttonSelectFile
            // 
            this.buttonSelectFile.Location = new System.Drawing.Point(6, 19);
            this.buttonSelectFile.Name = "buttonSelectFile";
            this.buttonSelectFile.Size = new System.Drawing.Size(97, 23);
            this.buttonSelectFile.TabIndex = 3;
            this.buttonSelectFile.Text = "Select File";
            this.buttonSelectFile.UseVisualStyleBackColor = true;
            this.buttonSelectFile.Click += new System.EventHandler(this.ButtonSelectFile_Click);
            // 
            // buttonLoadImage
            // 
            this.buttonLoadImage.Location = new System.Drawing.Point(5, 18);
            this.buttonLoadImage.Name = "buttonLoadImage";
            this.buttonLoadImage.Size = new System.Drawing.Size(130, 23);
            this.buttonLoadImage.TabIndex = 4;
            this.buttonLoadImage.Text = "Load to RAM";
            this.buttonLoadImage.UseVisualStyleBackColor = true;
            this.buttonLoadImage.Click += new System.EventHandler(this.ButtonLoadImage_Click);
            // 
            // buttonPrint
            // 
            this.buttonPrint.Location = new System.Drawing.Point(6, 18);
            this.buttonPrint.Name = "buttonPrint";
            this.buttonPrint.Size = new System.Drawing.Size(133, 23);
            this.buttonPrint.TabIndex = 5;
            this.buttonPrint.Text = "Print";
            this.buttonPrint.UseVisualStyleBackColor = true;
            this.buttonPrint.Click += new System.EventHandler(this.ButtonPrint_Click);
            // 
            // numericUpDownCopies
            // 
            this.numericUpDownCopies.Location = new System.Drawing.Point(63, 75);
            this.numericUpDownCopies.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericUpDownCopies.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownCopies.Name = "numericUpDownCopies";
            this.numericUpDownCopies.Size = new System.Drawing.Size(78, 20);
            this.numericUpDownCopies.TabIndex = 6;
            this.numericUpDownCopies.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // labelCopies
            // 
            this.labelCopies.AutoSize = true;
            this.labelCopies.Location = new System.Drawing.Point(6, 77);
            this.labelCopies.Name = "labelCopies";
            this.labelCopies.Size = new System.Drawing.Size(42, 13);
            this.labelCopies.TabIndex = 7;
            this.labelCopies.Text = "Copies:";
            // 
            // openFileDialogReadImage
            // 
            this.openFileDialogReadImage.DefaultExt = "bmp";
            this.openFileDialogReadImage.Filter = "Bitmap files|*.bmp|JPEG files|*.jpg|TIFF files|*.tif";
            this.openFileDialogReadImage.Title = "Please select the print image ...";
            // 
            // statusStripImage
            // 
            this.statusStripImage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusFilename,
            this.toolStripStatusImageDimensions});
            this.statusStripImage.Location = new System.Drawing.Point(0, 484);
            this.statusStripImage.Name = "statusStripImage";
            this.statusStripImage.Size = new System.Drawing.Size(552, 22);
            this.statusStripImage.SizingGrip = false;
            this.statusStripImage.TabIndex = 8;
            this.statusStripImage.Text = "statusStripImage";
            // 
            // toolStripStatusFilename
            // 
            this.toolStripStatusFilename.Name = "toolStripStatusFilename";
            this.toolStripStatusFilename.Size = new System.Drawing.Size(537, 17);
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
            this.groupBox1.Location = new System.Drawing.Point(0, 187);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(552, 294);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Preview";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.groupBox5);
            this.groupBox2.Controls.Add(this.groupBox4);
            this.groupBox2.Controls.Add(this.groupBoxPrint);
            this.groupBox2.Controls.Add(this.groupBoxLoad);
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Location = new System.Drawing.Point(0, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(552, 178);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Print Control";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.radioButtonForward);
            this.groupBox4.Controls.Add(this.radioButtonReverse);
            this.groupBox4.Location = new System.Drawing.Point(309, 11);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(109, 69);
            this.groupBox4.TabIndex = 17;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Scan Direction";
            // 
            // radioButtonForward
            // 
            this.radioButtonForward.AutoSize = true;
            this.radioButtonForward.Checked = true;
            this.radioButtonForward.Location = new System.Drawing.Point(6, 18);
            this.radioButtonForward.Name = "radioButtonForward";
            this.radioButtonForward.Size = new System.Drawing.Size(63, 17);
            this.radioButtonForward.TabIndex = 14;
            this.radioButtonForward.TabStop = true;
            this.radioButtonForward.Text = "Forward";
            this.radioButtonForward.UseVisualStyleBackColor = true;
            // 
            // radioButtonReverse
            // 
            this.radioButtonReverse.AutoSize = true;
            this.radioButtonReverse.Location = new System.Drawing.Point(6, 41);
            this.radioButtonReverse.Name = "radioButtonReverse";
            this.radioButtonReverse.Size = new System.Drawing.Size(65, 17);
            this.radioButtonReverse.TabIndex = 13;
            this.radioButtonReverse.Text = "Reverse";
            this.radioButtonReverse.UseVisualStyleBackColor = true;
            // 
            // groupBoxPrint
            // 
            this.groupBoxPrint.Controls.Add(this.labelDocType);
            this.groupBoxPrint.Controls.Add(this.comboBoxDocType);
            this.groupBoxPrint.Controls.Add(this.numericUpDownXStart);
            this.groupBoxPrint.Controls.Add(this.buttonPrint);
            this.groupBoxPrint.Controls.Add(this.numericUpDownCopies);
            this.groupBoxPrint.Controls.Add(this.labelCopies);
            this.groupBoxPrint.Controls.Add(this.labelXStart);
            this.groupBoxPrint.Controls.Add(this.labelXOffset);
            this.groupBoxPrint.Controls.Add(this.numericUpDownXOffset);
            this.groupBoxPrint.Location = new System.Drawing.Point(154, 11);
            this.groupBoxPrint.Name = "groupBoxPrint";
            this.groupBoxPrint.Size = new System.Drawing.Size(149, 161);
            this.groupBoxPrint.TabIndex = 16;
            this.groupBoxPrint.TabStop = false;
            // 
            // labelDocType
            // 
            this.labelDocType.AutoSize = true;
            this.labelDocType.Location = new System.Drawing.Point(6, 51);
            this.labelDocType.Name = "labelDocType";
            this.labelDocType.Size = new System.Drawing.Size(54, 13);
            this.labelDocType.TabIndex = 16;
            this.labelDocType.Text = "DocType:";
            // 
            // comboBoxDocType
            // 
            this.comboBoxDocType.FormattingEnabled = true;
            this.comboBoxDocType.Items.AddRange(new object[] {
            "FIFO",
            "Preload",
            "Scan"});
            this.comboBoxDocType.Location = new System.Drawing.Point(63, 48);
            this.comboBoxDocType.Name = "comboBoxDocType";
            this.comboBoxDocType.Size = new System.Drawing.Size(76, 21);
            this.comboBoxDocType.TabIndex = 15;
            this.comboBoxDocType.Text = "FIFO";
            // 
            // numericUpDownXStart
            // 
            this.numericUpDownXStart.Location = new System.Drawing.Point(63, 101);
            this.numericUpDownXStart.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDownXStart.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownXStart.Name = "numericUpDownXStart";
            this.numericUpDownXStart.Size = new System.Drawing.Size(78, 20);
            this.numericUpDownXStart.TabIndex = 14;
            this.numericUpDownXStart.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // labelXStart
            // 
            this.labelXStart.AutoSize = true;
            this.labelXStart.Location = new System.Drawing.Point(6, 103);
            this.labelXStart.Name = "labelXStart";
            this.labelXStart.Size = new System.Drawing.Size(42, 13);
            this.labelXStart.TabIndex = 9;
            this.labelXStart.Text = "X Start:";
            // 
            // labelXOffset
            // 
            this.labelXOffset.AutoSize = true;
            this.labelXOffset.Location = new System.Drawing.Point(6, 129);
            this.labelXOffset.Name = "labelXOffset";
            this.labelXOffset.Size = new System.Drawing.Size(48, 13);
            this.labelXOffset.TabIndex = 9;
            this.labelXOffset.Text = "X Offset:";
            // 
            // numericUpDownXOffset
            // 
            this.numericUpDownXOffset.Location = new System.Drawing.Point(63, 127);
            this.numericUpDownXOffset.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownXOffset.Name = "numericUpDownXOffset";
            this.numericUpDownXOffset.Size = new System.Drawing.Size(78, 20);
            this.numericUpDownXOffset.TabIndex = 11;
            // 
            // groupBoxLoad
            // 
            this.groupBoxLoad.Controls.Add(this.buttonLoadImage);
            this.groupBoxLoad.Controls.Add(this.numericUpDownYTop);
            this.groupBoxLoad.Controls.Add(this.labelYTop);
            this.groupBoxLoad.Controls.Add(this.checkBoxCircular);
            this.groupBoxLoad.Location = new System.Drawing.Point(6, 11);
            this.groupBoxLoad.Name = "groupBoxLoad";
            this.groupBoxLoad.Size = new System.Drawing.Size(140, 140);
            this.groupBoxLoad.TabIndex = 15;
            this.groupBoxLoad.TabStop = false;
            // 
            // numericUpDownYTop
            // 
            this.numericUpDownYTop.Location = new System.Drawing.Point(68, 49);
            this.numericUpDownYTop.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownYTop.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.numericUpDownYTop.Name = "numericUpDownYTop";
            this.numericUpDownYTop.Size = new System.Drawing.Size(67, 20);
            this.numericUpDownYTop.TabIndex = 8;
            // 
            // labelYTop
            // 
            this.labelYTop.AutoSize = true;
            this.labelYTop.Location = new System.Drawing.Point(5, 51);
            this.labelYTop.Name = "labelYTop";
            this.labelYTop.Size = new System.Drawing.Size(57, 13);
            this.labelYTop.TabIndex = 9;
            this.labelYTop.Text = "Y Position:";
            // 
            // checkBoxCircular
            // 
            this.checkBoxCircular.AutoSize = true;
            this.checkBoxCircular.Location = new System.Drawing.Point(9, 75);
            this.checkBoxCircular.Name = "checkBoxCircular";
            this.checkBoxCircular.Size = new System.Drawing.Size(99, 17);
            this.checkBoxCircular.TabIndex = 10;
            this.checkBoxCircular.Text = "Circular Printing";
            this.checkBoxCircular.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.buttonSelectFile);
            this.groupBox3.Location = new System.Drawing.Point(309, 117);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(109, 55);
            this.groupBox3.TabIndex = 14;
            this.groupBox3.TabStop = false;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.numericUpDownPcc);
            this.groupBox5.Controls.Add(this.label1);
            this.groupBox5.Location = new System.Drawing.Point(309, 80);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(109, 35);
            this.groupBox5.TabIndex = 18;
            this.groupBox5.TabStop = false;
            // 
            // numericUpDownPcc
            // 
            this.numericUpDownPcc.Location = new System.Drawing.Point(42, 11);
            this.numericUpDownPcc.Name = "numericUpDownPcc";
            this.numericUpDownPcc.Size = new System.Drawing.Size(61, 20);
            this.numericUpDownPcc.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "PCC:";
            // 
            // ImageControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.statusStripImage);
            this.Name = "ImageControl";
            this.Size = new System.Drawing.Size(552, 506);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPrintData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCopies)).EndInit();
            this.statusStripImage.ResumeLayout(false);
            this.statusStripImage.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBoxPrint.ResumeLayout(false);
            this.groupBoxPrint.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownXStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownXOffset)).EndInit();
            this.groupBoxLoad.ResumeLayout(false);
            this.groupBoxLoad.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownYTop)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPcc)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxPrintData;
        private System.Windows.Forms.Button buttonSelectFile;
        private System.Windows.Forms.Button buttonLoadImage;
        private System.Windows.Forms.Button buttonPrint;
        private System.Windows.Forms.NumericUpDown numericUpDownCopies;
        private System.Windows.Forms.Label labelCopies;
        private System.Windows.Forms.OpenFileDialog openFileDialogReadImage;
        private System.Windows.Forms.StatusStrip statusStripImage;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusFilename;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusImageDimensions;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label labelYTop;
        private System.Windows.Forms.NumericUpDown numericUpDownYTop;
        private System.Windows.Forms.NumericUpDown numericUpDownXOffset;
        private System.Windows.Forms.CheckBox checkBoxCircular;
        private System.Windows.Forms.Label labelXOffset;
        private System.Windows.Forms.GroupBox groupBoxPrint;
        private System.Windows.Forms.GroupBox groupBoxLoad;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.NumericUpDown numericUpDownXStart;
        private System.Windows.Forms.Label labelXStart;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton radioButtonForward;
        private System.Windows.Forms.RadioButton radioButtonReverse;
        private System.Windows.Forms.ComboBox comboBoxDocType;
        private System.Windows.Forms.Label labelDocType;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.NumericUpDown numericUpDownPcc;
        private System.Windows.Forms.Label label1;
    }
}
