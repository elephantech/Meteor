namespace MeteorInkJet.SdCardTestApp {
    partial class FifoImageControl {
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.buttonSelectFile = new System.Windows.Forms.Button();
            this.checkBoxEnableMixedMode = new System.Windows.Forms.CheckBox();
            this.imageFileControl = new MeteorInkJet.SdCardTestApp.ImageFileControl();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numericUpDownYTop = new System.Windows.Forms.NumericUpDown();
            this.labelYTop = new System.Windows.Forms.Label();
            this.numericUpDownXStart = new System.Windows.Forms.NumericUpDown();
            this.labelXStart = new System.Windows.Forms.Label();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownYTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownXStart)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.buttonSelectFile);
            this.groupBox3.Location = new System.Drawing.Point(302, 66);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(109, 55);
            this.groupBox3.TabIndex = 15;
            this.groupBox3.TabStop = false;
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
            // checkBoxEnableMixedMode
            // 
            this.checkBoxEnableMixedMode.AutoSize = true;
            this.checkBoxEnableMixedMode.Location = new System.Drawing.Point(12, 23);
            this.checkBoxEnableMixedMode.Name = "checkBoxEnableMixedMode";
            this.checkBoxEnableMixedMode.Size = new System.Drawing.Size(83, 17);
            this.checkBoxEnableMixedMode.TabIndex = 16;
            this.checkBoxEnableMixedMode.Text = "Mixed mode";
            this.checkBoxEnableMixedMode.UseVisualStyleBackColor = true;
            // 
            // imageFileControl
            // 
            this.imageFileControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.imageFileControl.Location = new System.Drawing.Point(0, 132);
            this.imageFileControl.Name = "imageFileControl";
            this.imageFileControl.Size = new System.Drawing.Size(552, 371);
            this.imageFileControl.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxEnableMixedMode);
            this.groupBox1.Location = new System.Drawing.Point(302, 11);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(109, 55);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            // 
            // numericUpDownYTop
            // 
            this.numericUpDownYTop.Location = new System.Drawing.Point(74, 60);
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
            this.numericUpDownYTop.TabIndex = 18;
            // 
            // labelYTop
            // 
            this.labelYTop.AutoSize = true;
            this.labelYTop.Location = new System.Drawing.Point(11, 61);
            this.labelYTop.Name = "labelYTop";
            this.labelYTop.Size = new System.Drawing.Size(57, 13);
            this.labelYTop.TabIndex = 19;
            this.labelYTop.Text = "Y Position:";
            // 
            // numericUpDownXStart
            // 
            this.numericUpDownXStart.Location = new System.Drawing.Point(210, 85);
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
            this.numericUpDownXStart.TabIndex = 21;
            this.numericUpDownXStart.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // labelXStart
            // 
            this.labelXStart.AutoSize = true;
            this.labelXStart.Location = new System.Drawing.Point(161, 87);
            this.labelXStart.Name = "labelXStart";
            this.labelXStart.Size = new System.Drawing.Size(42, 13);
            this.labelXStart.TabIndex = 20;
            this.labelXStart.Text = "X Start:";
            // 
            // FifoImageControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.numericUpDownXStart);
            this.Controls.Add(this.labelXStart);
            this.Controls.Add(this.numericUpDownYTop);
            this.Controls.Add(this.labelYTop);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.imageFileControl);
            this.Name = "FifoImageControl";
            this.Size = new System.Drawing.Size(552, 506);
            this.groupBox3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownYTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownXStart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ImageFileControl imageFileControl;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button buttonSelectFile;
        private System.Windows.Forms.CheckBox checkBoxEnableMixedMode;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown numericUpDownYTop;
        private System.Windows.Forms.Label labelYTop;
        private System.Windows.Forms.NumericUpDown numericUpDownXStart;
        private System.Windows.Forms.Label labelXStart;
    }
}
