namespace MeteorInkJet.SdCardTestApp
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
            this.buttonSelectFile = new System.Windows.Forms.Button();
            this.buttonLoadImage = new System.Windows.Forms.Button();
            this.buttonPrint = new System.Windows.Forms.Button();
            this.numericUpDownCopies = new System.Windows.Forms.NumericUpDown();
            this.labelCopies = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBoxPrint = new System.Windows.Forms.GroupBox();
            this.numericUpDownXStart = new System.Windows.Forms.NumericUpDown();
            this.labelXStart = new System.Windows.Forms.Label();
            this.groupBoxLoad = new System.Windows.Forms.GroupBox();
            this.numericUpDownYTop = new System.Windows.Forms.NumericUpDown();
            this.labelYTop = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.imageFileControl = new MeteorInkJet.SdCardTestApp.ImageFileControl();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCopies)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBoxPrint.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownXStart)).BeginInit();
            this.groupBoxLoad.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownYTop)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
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
            this.buttonLoadImage.Text = "Load to SD Card";
            this.buttonLoadImage.UseVisualStyleBackColor = true;
            this.buttonLoadImage.Click += new System.EventHandler(this.ButtonLoadImage_Click);
            // 
            // buttonPrint
            // 
            this.buttonPrint.Location = new System.Drawing.Point(4, 18);
            this.buttonPrint.Name = "buttonPrint";
            this.buttonPrint.Size = new System.Drawing.Size(133, 23);
            this.buttonPrint.TabIndex = 5;
            this.buttonPrint.Text = "Print";
            this.buttonPrint.UseVisualStyleBackColor = true;
            this.buttonPrint.Click += new System.EventHandler(this.ButtonPrint_Click);
            // 
            // numericUpDownCopies
            // 
            this.numericUpDownCopies.Location = new System.Drawing.Point(56, 49);
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
            this.labelCopies.Location = new System.Drawing.Point(7, 51);
            this.labelCopies.Name = "labelCopies";
            this.labelCopies.Size = new System.Drawing.Size(42, 13);
            this.labelCopies.TabIndex = 7;
            this.labelCopies.Text = "Copies:";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.groupBoxPrint);
            this.groupBox2.Controls.Add(this.groupBoxLoad);
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Location = new System.Drawing.Point(0, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(552, 123);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Print Control";
            // 
            // groupBoxPrint
            // 
            this.groupBoxPrint.Controls.Add(this.numericUpDownXStart);
            this.groupBoxPrint.Controls.Add(this.buttonPrint);
            this.groupBoxPrint.Controls.Add(this.numericUpDownCopies);
            this.groupBoxPrint.Controls.Add(this.labelCopies);
            this.groupBoxPrint.Controls.Add(this.labelXStart);
            this.groupBoxPrint.Location = new System.Drawing.Point(154, 11);
            this.groupBoxPrint.Name = "groupBoxPrint";
            this.groupBoxPrint.Size = new System.Drawing.Size(140, 107);
            this.groupBoxPrint.TabIndex = 16;
            this.groupBoxPrint.TabStop = false;
            // 
            // numericUpDownXStart
            // 
            this.numericUpDownXStart.Location = new System.Drawing.Point(56, 74);
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
            this.labelXStart.Location = new System.Drawing.Point(7, 76);
            this.labelXStart.Name = "labelXStart";
            this.labelXStart.Size = new System.Drawing.Size(42, 13);
            this.labelXStart.TabIndex = 9;
            this.labelXStart.Text = "X Start:";
            // 
            // groupBoxLoad
            // 
            this.groupBoxLoad.Controls.Add(this.buttonLoadImage);
            this.groupBoxLoad.Controls.Add(this.numericUpDownYTop);
            this.groupBoxLoad.Controls.Add(this.labelYTop);
            this.groupBoxLoad.Location = new System.Drawing.Point(6, 11);
            this.groupBoxLoad.Name = "groupBoxLoad";
            this.groupBoxLoad.Size = new System.Drawing.Size(140, 107);
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
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.buttonSelectFile);
            this.groupBox3.Location = new System.Drawing.Point(302, 63);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(109, 55);
            this.groupBox3.TabIndex = 14;
            this.groupBox3.TabStop = false;
            // 
            // imageFileControl
            // 
            this.imageFileControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.imageFileControl.Location = new System.Drawing.Point(0, 132);
            this.imageFileControl.Name = "imageFileControl";
            this.imageFileControl.Size = new System.Drawing.Size(552, 371);
            this.imageFileControl.TabIndex = 11;
            // 
            // ImageControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.imageFileControl);
            this.Controls.Add(this.groupBox2);
            this.Name = "ImageControl";
            this.Size = new System.Drawing.Size(552, 506);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCopies)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBoxPrint.ResumeLayout(false);
            this.groupBoxPrint.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownXStart)).EndInit();
            this.groupBoxLoad.ResumeLayout(false);
            this.groupBoxLoad.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownYTop)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button buttonSelectFile;
        private System.Windows.Forms.Button buttonLoadImage;
        private System.Windows.Forms.Button buttonPrint;
        private System.Windows.Forms.NumericUpDown numericUpDownCopies;
        private System.Windows.Forms.Label labelCopies;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label labelYTop;
        private System.Windows.Forms.NumericUpDown numericUpDownYTop;
        private System.Windows.Forms.GroupBox groupBoxPrint;
        private System.Windows.Forms.GroupBox groupBoxLoad;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.NumericUpDown numericUpDownXStart;
        private System.Windows.Forms.Label labelXStart;
        private ImageFileControl imageFileControl;
    }
}
