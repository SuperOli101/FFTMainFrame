namespace Honeywell.Forms
{
	partial class CFormErrorList
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
			if(disposing && (components != null))
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
			this.buttonOK = new System.Windows.Forms.Button();
			this.listViewErrorList = new System.Windows.Forms.ListView();
			this.columnHeaderErrorCode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeaderErrorMsg = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(17, 485);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(1006, 33);
			this.buttonOK.TabIndex = 0;
			this.buttonOK.Text = "O.K.";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// listViewErrorList
			// 
			this.listViewErrorList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listViewErrorList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderErrorCode,
            this.columnHeaderErrorMsg});
			this.listViewErrorList.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.listViewErrorList.HideSelection = false;
			this.listViewErrorList.Location = new System.Drawing.Point(17, 12);
			this.listViewErrorList.MultiSelect = false;
			this.listViewErrorList.Name = "listViewErrorList";
			this.listViewErrorList.Size = new System.Drawing.Size(1006, 448);
			this.listViewErrorList.TabIndex = 3;
			this.listViewErrorList.UseCompatibleStateImageBehavior = false;
			this.listViewErrorList.View = System.Windows.Forms.View.Details;
			// 
			// columnHeaderErrorCode
			// 
			this.columnHeaderErrorCode.Text = "Error code";
			this.columnHeaderErrorCode.Width = 141;
			// 
			// columnHeaderErrorMsg
			// 
			this.columnHeaderErrorMsg.Text = "Error message";
			this.columnHeaderErrorMsg.Width = 902;
			// 
			// CFormErrorList
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1035, 536);
			this.Controls.Add(this.listViewErrorList);
			this.Controls.Add(this.buttonOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "CFormErrorList";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Error";
			this.Load += new System.EventHandler(this.CFormErrorList_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.ListView listViewErrorList;
		private System.Windows.Forms.ColumnHeader columnHeaderErrorCode;
		private System.Windows.Forms.ColumnHeader columnHeaderErrorMsg;
	}
}