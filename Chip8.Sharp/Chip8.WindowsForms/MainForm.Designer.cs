namespace Chip8.WindowsForms
{
    partial class MainForm
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
            this.cbPrograms = new System.Windows.Forms.ComboBox();
            this.pbScreen = new Chip8.WindowsForms.PictureBoxWithInterpolationMode();
            this.frameRateStatusLabel = new System.Windows.Forms.Label();
            this.clockRateStatusLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbScreen)).BeginInit();
            this.SuspendLayout();
            // 
            // cbPrograms
            // 
            this.cbPrograms.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbPrograms.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPrograms.FormattingEnabled = true;
            this.cbPrograms.Location = new System.Drawing.Point(0, 0);
            this.cbPrograms.Margin = new System.Windows.Forms.Padding(6);
            this.cbPrograms.Name = "cbPrograms";
            this.cbPrograms.Size = new System.Drawing.Size(1280, 33);
            this.cbPrograms.TabIndex = 1;
            this.cbPrograms.TabStop = false;
            this.cbPrograms.SelectedIndexChanged += new System.EventHandler(this.cbPrograms_SelectedIndexChanged);
            // 
            // pbScreen
            // 
            this.pbScreen.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbScreen.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.pbScreen.Location = new System.Drawing.Point(0, 52);
            this.pbScreen.Margin = new System.Windows.Forms.Padding(6);
            this.pbScreen.Name = "pbScreen";
            this.pbScreen.Size = new System.Drawing.Size(1280, 923);
            this.pbScreen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbScreen.TabIndex = 0;
            this.pbScreen.TabStop = false;
            // 
            // frameRateStatusLabel
            // 
            this.frameRateStatusLabel.AutoSize = true;
            this.frameRateStatusLabel.ForeColor = System.Drawing.Color.Red;
            this.frameRateStatusLabel.Location = new System.Drawing.Point(12, 63);
            this.frameRateStatusLabel.Name = "frameRateStatusLabel";
            this.frameRateStatusLabel.Size = new System.Drawing.Size(71, 25);
            this.frameRateStatusLabel.TabIndex = 2;
            this.frameRateStatusLabel.Text = "0 FPS";
            // 
            // clockRateStatusLabel
            // 
            this.clockRateStatusLabel.AutoSize = true;
            this.clockRateStatusLabel.ForeColor = System.Drawing.Color.Red;
            this.clockRateStatusLabel.Location = new System.Drawing.Point(12, 102);
            this.clockRateStatusLabel.Name = "clockRateStatusLabel";
            this.clockRateStatusLabel.Size = new System.Drawing.Size(56, 25);
            this.clockRateStatusLabel.TabIndex = 3;
            this.clockRateStatusLabel.Text = "0 Hz";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1280, 977);
            this.Controls.Add(this.clockRateStatusLabel);
            this.Controls.Add(this.frameRateStatusLabel);
            this.Controls.Add(this.cbPrograms);
            this.Controls.Add(this.pbScreen);
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "CHIP-8 Emulator (WinForms)";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.pbScreen)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PictureBoxWithInterpolationMode pbScreen;
        private System.Windows.Forms.ComboBox cbPrograms;
        private System.Windows.Forms.Label frameRateStatusLabel;
        private System.Windows.Forms.Label clockRateStatusLabel;
    }
}

