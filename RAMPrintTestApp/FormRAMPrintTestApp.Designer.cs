namespace Ttp.Meteor.RAMPrintTestApp
{
    partial class FormRAMPrintTestApp
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
            if (disposing && (components != null)) {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRAMPrintTestApp));
            this.buttonStartJob = new System.Windows.Forms.Button();
            this.buttonStopJob = new System.Windows.Forms.Button();
            this.numericUpDownFrequency = new System.Windows.Forms.NumericUpDown();
            this.labelkHz = new System.Windows.Forms.Label();
            this.textBoxStatus = new System.Windows.Forms.TextBox();
            this.groupBoxStatus = new System.Windows.Forms.GroupBox();
            this.groupBoxControl = new System.Windows.Forms.GroupBox();
            this.comboBoxJobType = new System.Windows.Forms.ComboBox();
            this.labelJobType = new System.Windows.Forms.Label();
            this.numericUpDownDocWidth = new System.Windows.Forms.NumericUpDown();
            this.labelDocWidth = new System.Windows.Forms.Label();
            this.checkBoxEnableHeadPower = new System.Windows.Forms.CheckBox();
            this.groupBoxResolution = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanelBpp = new System.Windows.Forms.FlowLayoutPanel();
            this.radioButton1bpp = new System.Windows.Forms.RadioButton();
            this.radioButton2bpp = new System.Windows.Forms.RadioButton();
            this.radioButton4bpp = new System.Windows.Forms.RadioButton();
            this.timerMeteorStatus = new System.Windows.Forms.Timer(this.components);
            this.groupBoxPrintClock = new System.Windows.Forms.GroupBox();
            this.radioButtonInternalEncoder = new System.Windows.Forms.RadioButton();
            this.radioButtonExternalEncoder = new System.Windows.Forms.RadioButton();
            this.tabControlImages = new System.Windows.Forms.TabControl();
            this.tabPageImage0 = new System.Windows.Forms.TabPage();
            this.imageControl1 = new Ttp.Meteor.RAMPrintTestApp.ImageControl();
            this.tabPageImage1 = new System.Windows.Forms.TabPage();
            this.imageControl2 = new Ttp.Meteor.RAMPrintTestApp.ImageControl();
            this.tabPageImage2 = new System.Windows.Forms.TabPage();
            this.imageControl3 = new Ttp.Meteor.RAMPrintTestApp.ImageControl();
            this.tabPageImage3 = new System.Windows.Forms.TabPage();
            this.imageControl4 = new Ttp.Meteor.RAMPrintTestApp.ImageControl();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFrequency)).BeginInit();
            this.groupBoxStatus.SuspendLayout();
            this.groupBoxControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDocWidth)).BeginInit();
            this.groupBoxResolution.SuspendLayout();
            this.flowLayoutPanelBpp.SuspendLayout();
            this.groupBoxPrintClock.SuspendLayout();
            this.tabControlImages.SuspendLayout();
            this.tabPageImage0.SuspendLayout();
            this.tabPageImage1.SuspendLayout();
            this.tabPageImage2.SuspendLayout();
            this.tabPageImage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonStartJob
            // 
            this.buttonStartJob.Enabled = false;
            this.buttonStartJob.Location = new System.Drawing.Point(14, 109);
            this.buttonStartJob.Name = "buttonStartJob";
            this.buttonStartJob.Size = new System.Drawing.Size(80, 23);
            this.buttonStartJob.TabIndex = 3;
            this.buttonStartJob.Text = "Start Job";
            this.buttonStartJob.UseVisualStyleBackColor = true;
            this.buttonStartJob.Click += new System.EventHandler(this.ButtonStartJob_Click);
            // 
            // buttonStopJob
            // 
            this.buttonStopJob.Enabled = false;
            this.buttonStopJob.Location = new System.Drawing.Point(112, 109);
            this.buttonStopJob.Name = "buttonStopJob";
            this.buttonStopJob.Size = new System.Drawing.Size(80, 23);
            this.buttonStopJob.TabIndex = 4;
            this.buttonStopJob.Text = "Stop Job";
            this.buttonStopJob.UseVisualStyleBackColor = true;
            this.buttonStopJob.Click += new System.EventHandler(this.ButtonStopPrint_Click);
            // 
            // numericUpDownFrequency
            // 
            this.numericUpDownFrequency.DecimalPlaces = 1;
            this.numericUpDownFrequency.Location = new System.Drawing.Point(121, 42);
            this.numericUpDownFrequency.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numericUpDownFrequency.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDownFrequency.Name = "numericUpDownFrequency";
            this.numericUpDownFrequency.Size = new System.Drawing.Size(50, 20);
            this.numericUpDownFrequency.TabIndex = 2;
            this.numericUpDownFrequency.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // labelkHz
            // 
            this.labelkHz.AutoSize = true;
            this.labelkHz.Location = new System.Drawing.Point(171, 44);
            this.labelkHz.Name = "labelkHz";
            this.labelkHz.Size = new System.Drawing.Size(26, 13);
            this.labelkHz.TabIndex = 4;
            this.labelkHz.Text = "kHz";
            // 
            // textBoxStatus
            // 
            this.textBoxStatus.Location = new System.Drawing.Point(7, 19);
            this.textBoxStatus.Name = "textBoxStatus";
            this.textBoxStatus.ReadOnly = true;
            this.textBoxStatus.Size = new System.Drawing.Size(185, 20);
            this.textBoxStatus.TabIndex = 1;
            this.textBoxStatus.TabStop = false;
            this.textBoxStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // groupBoxStatus
            // 
            this.groupBoxStatus.Controls.Add(this.textBoxStatus);
            this.groupBoxStatus.Location = new System.Drawing.Point(5, 3);
            this.groupBoxStatus.Name = "groupBoxStatus";
            this.groupBoxStatus.Size = new System.Drawing.Size(206, 51);
            this.groupBoxStatus.TabIndex = 1;
            this.groupBoxStatus.TabStop = false;
            this.groupBoxStatus.Text = "Status:";
            // 
            // groupBoxControl
            // 
            this.groupBoxControl.Controls.Add(this.comboBoxJobType);
            this.groupBoxControl.Controls.Add(this.labelJobType);
            this.groupBoxControl.Controls.Add(this.numericUpDownDocWidth);
            this.groupBoxControl.Controls.Add(this.labelDocWidth);
            this.groupBoxControl.Controls.Add(this.checkBoxEnableHeadPower);
            this.groupBoxControl.Controls.Add(this.buttonStartJob);
            this.groupBoxControl.Controls.Add(this.buttonStopJob);
            this.groupBoxControl.Location = new System.Drawing.Point(5, 61);
            this.groupBoxControl.Name = "groupBoxControl";
            this.groupBoxControl.Size = new System.Drawing.Size(206, 143);
            this.groupBoxControl.TabIndex = 2;
            this.groupBoxControl.TabStop = false;
            this.groupBoxControl.Text = "Print Control";
            // 
            // comboBoxJobType
            // 
            this.comboBoxJobType.FormattingEnabled = true;
            this.comboBoxJobType.Items.AddRange(new object[] {
            "Preload",
            "FIFO",
            "Mixed",
            "Scan",
            "Dual FIFO"});
            this.comboBoxJobType.Location = new System.Drawing.Point(87, 47);
            this.comboBoxJobType.Name = "comboBoxJobType";
            this.comboBoxJobType.Size = new System.Drawing.Size(105, 21);
            this.comboBoxJobType.TabIndex = 8;
            // 
            // labelJobType
            // 
            this.labelJobType.AutoSize = true;
            this.labelJobType.Location = new System.Drawing.Point(11, 50);
            this.labelJobType.Name = "labelJobType";
            this.labelJobType.Size = new System.Drawing.Size(51, 13);
            this.labelJobType.TabIndex = 9;
            this.labelJobType.Text = "JobType:";
            // 
            // numericUpDownDocWidth
            // 
            this.numericUpDownDocWidth.Location = new System.Drawing.Point(87, 74);
            this.numericUpDownDocWidth.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericUpDownDocWidth.Name = "numericUpDownDocWidth";
            this.numericUpDownDocWidth.Size = new System.Drawing.Size(105, 20);
            this.numericUpDownDocWidth.TabIndex = 6;
            // 
            // labelDocWidth
            // 
            this.labelDocWidth.AutoSize = true;
            this.labelDocWidth.Location = new System.Drawing.Point(11, 76);
            this.labelDocWidth.Name = "labelDocWidth";
            this.labelDocWidth.Size = new System.Drawing.Size(61, 13);
            this.labelDocWidth.TabIndex = 5;
            this.labelDocWidth.Text = "Doc Width:";
            // 
            // checkBoxEnableHeadPower
            // 
            this.checkBoxEnableHeadPower.AutoSize = true;
            this.checkBoxEnableHeadPower.Location = new System.Drawing.Point(14, 21);
            this.checkBoxEnableHeadPower.Name = "checkBoxEnableHeadPower";
            this.checkBoxEnableHeadPower.Size = new System.Drawing.Size(121, 17);
            this.checkBoxEnableHeadPower.TabIndex = 0;
            this.checkBoxEnableHeadPower.Text = "Enable Head Power";
            this.checkBoxEnableHeadPower.UseVisualStyleBackColor = true;
            this.checkBoxEnableHeadPower.CheckedChanged += new System.EventHandler(this.CheckBoxEnableHeadPower_CheckedChanged);
            // 
            // groupBoxResolution
            // 
            this.groupBoxResolution.Controls.Add(this.flowLayoutPanelBpp);
            this.groupBoxResolution.Location = new System.Drawing.Point(5, 288);
            this.groupBoxResolution.Name = "groupBoxResolution";
            this.groupBoxResolution.Size = new System.Drawing.Size(206, 51);
            this.groupBoxResolution.TabIndex = 5;
            this.groupBoxResolution.TabStop = false;
            this.groupBoxResolution.Text = "Resolution";
            // 
            // flowLayoutPanelBpp
            // 
            this.flowLayoutPanelBpp.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.flowLayoutPanelBpp.AutoSize = true;
            this.flowLayoutPanelBpp.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanelBpp.Controls.Add(this.radioButton1bpp);
            this.flowLayoutPanelBpp.Controls.Add(this.radioButton2bpp);
            this.flowLayoutPanelBpp.Controls.Add(this.radioButton4bpp);
            this.flowLayoutPanelBpp.Location = new System.Drawing.Point(18, 16);
            this.flowLayoutPanelBpp.Name = "flowLayoutPanelBpp";
            this.flowLayoutPanelBpp.Size = new System.Drawing.Size(165, 23);
            this.flowLayoutPanelBpp.TabIndex = 1;
            // 
            // radioButton1bpp
            // 
            this.radioButton1bpp.AutoSize = true;
            this.radioButton1bpp.Checked = true;
            this.radioButton1bpp.Location = new System.Drawing.Point(3, 3);
            this.radioButton1bpp.Name = "radioButton1bpp";
            this.radioButton1bpp.Size = new System.Drawing.Size(49, 17);
            this.radioButton1bpp.TabIndex = 0;
            this.radioButton1bpp.TabStop = true;
            this.radioButton1bpp.Text = "1bpp";
            this.radioButton1bpp.UseVisualStyleBackColor = true;
            // 
            // radioButton2bpp
            // 
            this.radioButton2bpp.AutoSize = true;
            this.radioButton2bpp.Location = new System.Drawing.Point(58, 3);
            this.radioButton2bpp.Name = "radioButton2bpp";
            this.radioButton2bpp.Size = new System.Drawing.Size(49, 17);
            this.radioButton2bpp.TabIndex = 0;
            this.radioButton2bpp.Text = "2bpp";
            this.radioButton2bpp.UseVisualStyleBackColor = true;
            // 
            // radioButton4bpp
            // 
            this.radioButton4bpp.AutoSize = true;
            this.radioButton4bpp.Location = new System.Drawing.Point(113, 3);
            this.radioButton4bpp.Name = "radioButton4bpp";
            this.radioButton4bpp.Size = new System.Drawing.Size(49, 17);
            this.radioButton4bpp.TabIndex = 0;
            this.radioButton4bpp.Text = "4bpp";
            this.radioButton4bpp.UseVisualStyleBackColor = true;
            // 
            // timerMeteorStatus
            // 
            this.timerMeteorStatus.Enabled = true;
            this.timerMeteorStatus.Interval = 250;
            this.timerMeteorStatus.Tick += new System.EventHandler(this.TimerMeteorStatus_Tick);
            // 
            // groupBoxPrintClock
            // 
            this.groupBoxPrintClock.Controls.Add(this.numericUpDownFrequency);
            this.groupBoxPrintClock.Controls.Add(this.radioButtonInternalEncoder);
            this.groupBoxPrintClock.Controls.Add(this.radioButtonExternalEncoder);
            this.groupBoxPrintClock.Controls.Add(this.labelkHz);
            this.groupBoxPrintClock.Location = new System.Drawing.Point(5, 210);
            this.groupBoxPrintClock.Name = "groupBoxPrintClock";
            this.groupBoxPrintClock.Size = new System.Drawing.Size(206, 72);
            this.groupBoxPrintClock.TabIndex = 4;
            this.groupBoxPrintClock.TabStop = false;
            this.groupBoxPrintClock.Text = "Print Clock";
            // 
            // radioButtonInternalEncoder
            // 
            this.radioButtonInternalEncoder.AutoSize = true;
            this.radioButtonInternalEncoder.Location = new System.Drawing.Point(21, 42);
            this.radioButtonInternalEncoder.Name = "radioButtonInternalEncoder";
            this.radioButtonInternalEncoder.Size = new System.Drawing.Size(102, 17);
            this.radioButtonInternalEncoder.TabIndex = 1;
            this.radioButtonInternalEncoder.TabStop = true;
            this.radioButtonInternalEncoder.Text = "Internal encoder";
            this.radioButtonInternalEncoder.UseVisualStyleBackColor = true;
            // 
            // radioButtonExternalEncoder
            // 
            this.radioButtonExternalEncoder.AutoSize = true;
            this.radioButtonExternalEncoder.Location = new System.Drawing.Point(21, 19);
            this.radioButtonExternalEncoder.Name = "radioButtonExternalEncoder";
            this.radioButtonExternalEncoder.Size = new System.Drawing.Size(105, 17);
            this.radioButtonExternalEncoder.TabIndex = 1;
            this.radioButtonExternalEncoder.TabStop = true;
            this.radioButtonExternalEncoder.Text = "External encoder";
            this.radioButtonExternalEncoder.UseVisualStyleBackColor = true;
            // 
            // tabControlImages
            // 
            this.tabControlImages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlImages.Controls.Add(this.tabPageImage0);
            this.tabControlImages.Controls.Add(this.tabPageImage1);
            this.tabControlImages.Controls.Add(this.tabPageImage2);
            this.tabControlImages.Controls.Add(this.tabPageImage3);
            this.tabControlImages.Location = new System.Drawing.Point(217, 13);
            this.tabControlImages.Name = "tabControlImages";
            this.tabControlImages.SelectedIndex = 0;
            this.tabControlImages.Size = new System.Drawing.Size(442, 505);
            this.tabControlImages.TabIndex = 7;
            // 
            // tabPageImage0
            // 
            this.tabPageImage0.Controls.Add(this.imageControl1);
            this.tabPageImage0.Location = new System.Drawing.Point(4, 22);
            this.tabPageImage0.Name = "tabPageImage0";
            this.tabPageImage0.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageImage0.Size = new System.Drawing.Size(434, 479);
            this.tabPageImage0.TabIndex = 0;
            this.tabPageImage0.Text = "Image #1";
            this.tabPageImage0.UseVisualStyleBackColor = true;
            // 
            // imageControl1
            // 
            this.imageControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.imageControl1.ImageIndex = 0;
            this.imageControl1.Location = new System.Drawing.Point(7, 7);
            this.imageControl1.Name = "imageControl1";
            this.imageControl1.Size = new System.Drawing.Size(420, 463);
            this.imageControl1.TabIndex = 0;
            // 
            // tabPageImage1
            // 
            this.tabPageImage1.Controls.Add(this.imageControl2);
            this.tabPageImage1.Location = new System.Drawing.Point(4, 22);
            this.tabPageImage1.Name = "tabPageImage1";
            this.tabPageImage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageImage1.Size = new System.Drawing.Size(434, 479);
            this.tabPageImage1.TabIndex = 1;
            this.tabPageImage1.Text = "Image #2";
            this.tabPageImage1.UseVisualStyleBackColor = true;
            // 
            // imageControl2
            // 
            this.imageControl2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.imageControl2.ImageIndex = 1;
            this.imageControl2.Location = new System.Drawing.Point(7, 7);
            this.imageControl2.Name = "imageControl2";
            this.imageControl2.Size = new System.Drawing.Size(420, 463);
            this.imageControl2.TabIndex = 0;
            // 
            // tabPageImage2
            // 
            this.tabPageImage2.Controls.Add(this.imageControl3);
            this.tabPageImage2.Location = new System.Drawing.Point(4, 22);
            this.tabPageImage2.Name = "tabPageImage2";
            this.tabPageImage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageImage2.Size = new System.Drawing.Size(434, 479);
            this.tabPageImage2.TabIndex = 2;
            this.tabPageImage2.Text = "Image #3";
            this.tabPageImage2.UseVisualStyleBackColor = true;
            // 
            // imageControl3
            // 
            this.imageControl3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.imageControl3.ImageIndex = 2;
            this.imageControl3.Location = new System.Drawing.Point(7, 7);
            this.imageControl3.Name = "imageControl3";
            this.imageControl3.Size = new System.Drawing.Size(420, 463);
            this.imageControl3.TabIndex = 0;
            // 
            // tabPageImage3
            // 
            this.tabPageImage3.Controls.Add(this.imageControl4);
            this.tabPageImage3.Location = new System.Drawing.Point(4, 22);
            this.tabPageImage3.Name = "tabPageImage3";
            this.tabPageImage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageImage3.Size = new System.Drawing.Size(434, 479);
            this.tabPageImage3.TabIndex = 3;
            this.tabPageImage3.Text = "Image #4";
            this.tabPageImage3.UseVisualStyleBackColor = true;
            // 
            // imageControl4
            // 
            this.imageControl4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.imageControl4.ImageIndex = 3;
            this.imageControl4.Location = new System.Drawing.Point(7, 7);
            this.imageControl4.Name = "imageControl4";
            this.imageControl4.Size = new System.Drawing.Size(420, 463);
            this.imageControl4.TabIndex = 0;
            // 
            // FormRAMPrintTestApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(661, 521);
            this.Controls.Add(this.tabControlImages);
            this.Controls.Add(this.groupBoxPrintClock);
            this.Controls.Add(this.groupBoxControl);
            this.Controls.Add(this.groupBoxResolution);
            this.Controls.Add(this.groupBoxStatus);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(677, 559);
            this.Name = "FormRAMPrintTestApp";
            this.Text = "RAMPrintTestApp";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormRAMPrintTestApp_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFrequency)).EndInit();
            this.groupBoxStatus.ResumeLayout(false);
            this.groupBoxStatus.PerformLayout();
            this.groupBoxControl.ResumeLayout(false);
            this.groupBoxControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDocWidth)).EndInit();
            this.groupBoxResolution.ResumeLayout(false);
            this.groupBoxResolution.PerformLayout();
            this.flowLayoutPanelBpp.ResumeLayout(false);
            this.flowLayoutPanelBpp.PerformLayout();
            this.groupBoxPrintClock.ResumeLayout(false);
            this.groupBoxPrintClock.PerformLayout();
            this.tabControlImages.ResumeLayout(false);
            this.tabPageImage0.ResumeLayout(false);
            this.tabPageImage1.ResumeLayout(false);
            this.tabPageImage2.ResumeLayout(false);
            this.tabPageImage3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonStartJob;
        private System.Windows.Forms.Button buttonStopJob;
        private System.Windows.Forms.NumericUpDown numericUpDownFrequency;
        private System.Windows.Forms.Label labelkHz;
        private System.Windows.Forms.TextBox textBoxStatus;
        private System.Windows.Forms.GroupBox groupBoxStatus;
        private System.Windows.Forms.GroupBox groupBoxControl;
        private System.Windows.Forms.GroupBox groupBoxResolution;
        private System.Windows.Forms.Timer timerMeteorStatus;
        private System.Windows.Forms.CheckBox checkBoxEnableHeadPower;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelBpp;
        private System.Windows.Forms.RadioButton radioButton1bpp;
        private System.Windows.Forms.RadioButton radioButton2bpp;
        private System.Windows.Forms.RadioButton radioButton4bpp;
        private System.Windows.Forms.GroupBox groupBoxPrintClock;
        private System.Windows.Forms.RadioButton radioButtonInternalEncoder;
        private System.Windows.Forms.RadioButton radioButtonExternalEncoder;
        private System.Windows.Forms.TabControl tabControlImages;
        private System.Windows.Forms.TabPage tabPageImage0;
        private System.Windows.Forms.TabPage tabPageImage1;
        private ImageControl imageControl1;
        private ImageControl imageControl2;
        private System.Windows.Forms.NumericUpDown numericUpDownDocWidth;
        private System.Windows.Forms.Label labelDocWidth;
        private System.Windows.Forms.TabPage tabPageImage2;
        private ImageControl imageControl3;
        private System.Windows.Forms.TabPage tabPageImage3;
        private ImageControl imageControl4;
        private System.Windows.Forms.ComboBox comboBoxJobType;
        private System.Windows.Forms.Label labelJobType;
    }
}

