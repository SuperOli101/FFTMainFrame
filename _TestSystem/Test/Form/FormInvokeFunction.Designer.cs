namespace Honeywell.Forms
{
    partial class FormInvokeFunction
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
            this.Button2 = new System.Windows.Forms.Button();
            this.Button1 = new System.Windows.Forms.Button();
            this.TextBoxFunctionName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Button2
            // 
            this.Button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Button2.Location = new System.Drawing.Point(15, 222);
            this.Button2.Name = "Button2";
            this.Button2.Size = new System.Drawing.Size(165, 29);
            this.Button2.TabIndex = 1;
            this.Button2.Text = "button2";
            this.Button2.UseVisualStyleBackColor = true;
            this.Button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Button1
            // 
            this.Button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Button1.Location = new System.Drawing.Point(195, 222);
            this.Button1.Name = "Button1";
            this.Button1.Size = new System.Drawing.Size(165, 29);
            this.Button1.TabIndex = 0;
            this.Button1.Text = "button1";
            this.Button1.UseVisualStyleBackColor = true;
            this.Button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // TextBoxFunctionName
            // 
            this.TextBoxFunctionName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBoxFunctionName.Location = new System.Drawing.Point(15, 17);
            this.TextBoxFunctionName.Name = "TextBoxFunctionName";
            this.TextBoxFunctionName.ReadOnly = true;
            this.TextBoxFunctionName.Size = new System.Drawing.Size(345, 20);
            this.TextBoxFunctionName.TabIndex = 4;
            this.TextBoxFunctionName.TabStop = false;
            this.TextBoxFunctionName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TextBoxFunctionName.WordWrap = false;
            // 
            // FormInvokeFunction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(380, 276);
            this.Controls.Add(this.TextBoxFunctionName);
            this.Controls.Add(this.Button1);
            this.Controls.Add(this.Button2);
            this.MinimumSize = new System.Drawing.Size(388, 310);
            this.Name = "FormInvokeFunction";
            this.Text = "FormInvokeFunction";
            this.Load += new System.EventHandler(this.FormInvokeFunction_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Button Button2;
        public System.Windows.Forms.Button Button1;
        public System.Windows.Forms.TextBox TextBoxFunctionName;
    }
}