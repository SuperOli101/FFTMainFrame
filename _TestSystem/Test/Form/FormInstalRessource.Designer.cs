namespace Honeywell.Forms
{
    partial class CFormInstalRessource
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CFormInstalRessource));
			this.LabelMsg = new System.Windows.Forms.Label();
			this.ProgressBarDeviceInstal = new System.Windows.Forms.ProgressBar();
			this.SuspendLayout();
			// 
			// LabelMsg
			// 
			this.LabelMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.LabelMsg.Location = new System.Drawing.Point(20, 9);
			this.LabelMsg.MaximumSize = new System.Drawing.Size(10000, 100);
			this.LabelMsg.MinimumSize = new System.Drawing.Size(100, 0);
			this.LabelMsg.Name = "LabelMsg";
			this.LabelMsg.Size = new System.Drawing.Size(458, 62);
			this.LabelMsg.TabIndex = 0;
			this.LabelMsg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// ProgressBarDeviceInstal
			// 
			this.ProgressBarDeviceInstal.Location = new System.Drawing.Point(23, 87);
			this.ProgressBarDeviceInstal.Name = "ProgressBarDeviceInstal";
			this.ProgressBarDeviceInstal.Size = new System.Drawing.Size(455, 32);
			this.ProgressBarDeviceInstal.TabIndex = 1;
			this.ProgressBarDeviceInstal.Click += new System.EventHandler(this.progressBarDeviceInstal_Click);
			// 
			// CFormInstalRessource
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(503, 134);
			this.ControlBox = false;
			this.Controls.Add(this.ProgressBarDeviceInstal);
			this.Controls.Add(this.LabelMsg);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CFormInstalRessource";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "FormDeviceInstal";
			this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Label LabelMsg;
        public System.Windows.Forms.ProgressBar ProgressBarDeviceInstal;
    }
}