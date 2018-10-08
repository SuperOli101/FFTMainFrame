namespace Honeywell.Forms
{
    partial class CFormMenuSelect
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CFormMenuSelect));
			this.listViewMenu = new System.Windows.Forms.ListView();
			this.columnHeaderOSNumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeaderDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.buttonOK = new System.Windows.Forms.Button();
			this.textBoxOSNumber = new System.Windows.Forms.TextBox();
			this.comboBoxTestMode = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// listViewMenu
			// 
			this.listViewMenu.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderOSNumber,
            this.columnHeaderDescription});
			this.listViewMenu.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.listViewMenu.HideSelection = false;
			this.listViewMenu.Location = new System.Drawing.Point(23, 31);
			this.listViewMenu.MultiSelect = false;
			this.listViewMenu.Name = "listViewMenu";
			this.listViewMenu.Size = new System.Drawing.Size(456, 543);
			this.listViewMenu.TabIndex = 2;
			this.listViewMenu.UseCompatibleStateImageBehavior = false;
			this.listViewMenu.View = System.Windows.Forms.View.Details;
			this.listViewMenu.SelectedIndexChanged += new System.EventHandler(this.listViewMenu_SelectedIndexChanged);
			this.listViewMenu.DoubleClick += new System.EventHandler(this.listViewMenu_DoubleClick);
			this.listViewMenu.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listViewMenu_KeyDown);
			// 
			// columnHeaderOSNumber
			// 
			this.columnHeaderOSNumber.Text = "OS-Number";
			this.columnHeaderOSNumber.Width = 67;
			// 
			// columnHeaderDescription
			// 
			this.columnHeaderDescription.Text = "Description";
			this.columnHeaderDescription.Width = 346;
			// 
			// buttonOK
			// 
			this.buttonOK.BackColor = System.Drawing.SystemColors.Control;
			this.buttonOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonOK.Location = new System.Drawing.Point(23, 622);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(455, 34);
			this.buttonOK.TabIndex = 1;
			this.buttonOK.TabStop = false;
			this.buttonOK.Text = "O.K.";
			this.buttonOK.UseVisualStyleBackColor = false;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// textBoxOSNumber
			// 
			this.textBoxOSNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxOSNumber.Location = new System.Drawing.Point(256, 580);
			this.textBoxOSNumber.Name = "textBoxOSNumber";
			this.textBoxOSNumber.Size = new System.Drawing.Size(222, 22);
			this.textBoxOSNumber.TabIndex = 0;
			this.textBoxOSNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBoxOSNumber.TextChanged += new System.EventHandler(this.textBoxOSNumber_TextChanged);
			this.textBoxOSNumber.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxOSNumber_KeyDown);
			// 
			// comboBoxTestMode
			// 
			this.comboBoxTestMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxTestMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.comboBoxTestMode.FormattingEnabled = true;
			this.comboBoxTestMode.Location = new System.Drawing.Point(100, 580);
			this.comboBoxTestMode.Name = "comboBoxTestMode";
			this.comboBoxTestMode.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.comboBoxTestMode.Size = new System.Drawing.Size(135, 24);
			this.comboBoxTestMode.TabIndex = 3;
			this.comboBoxTestMode.TabStop = false;
			// 
			// labelMsg
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(20, 583);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(75, 16);
			this.label1.TabIndex = 4;
			this.label1.Text = "FFT-Mode:";
			// 
			// CFormMenuSelect
			// 
            
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(505, 668);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboBoxTestMode);
			this.Controls.Add(this.textBoxOSNumber);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.listViewMenu);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CFormMenuSelect";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Variant";
			this.Load += new System.EventHandler(this.FormMenuSelect_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormMenuSelect_KeyDown);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listViewMenu;
        private System.Windows.Forms.ColumnHeader columnHeaderOSNumber;
        private System.Windows.Forms.ColumnHeader columnHeaderDescription;
        

        private System.Windows.Forms.Button buttonOK;
        private Honeywell.Test.CTest test;
        private System.Windows.Forms.TextBox textBoxOSNumber;
        private System.Windows.Forms.ComboBox comboBoxTestMode;
        private System.Windows.Forms.Label label1;

    }
}