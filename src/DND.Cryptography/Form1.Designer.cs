namespace DND.Cryptography
{
    partial class Form1
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
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnGenerateRSAKeys = new System.Windows.Forms.Button();
            this.txtPrivateKey = new System.Windows.Forms.TextBox();
            this.txtPublicKey = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.btnSaveRSAPublicKey = new System.Windows.Forms.Button();
            this.btnSaveRSAPrivateKey = new System.Windows.Forms.Button();
            this.tabPage1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnSaveRSAPrivateKey);
            this.tabPage1.Controls.Add(this.btnSaveRSAPublicKey);
            this.tabPage1.Controls.Add(this.btnGenerateRSAKeys);
            this.tabPage1.Controls.Add(this.txtPrivateKey);
            this.tabPage1.Controls.Add(this.txtPublicKey);
            this.tabPage1.Location = new System.Drawing.Point(8, 39);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1559, 1061);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "RSA Keys";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnGenerateRSAKeys
            // 
            this.btnGenerateRSAKeys.Location = new System.Drawing.Point(49, 36);
            this.btnGenerateRSAKeys.Name = "btnGenerateRSAKeys";
            this.btnGenerateRSAKeys.Size = new System.Drawing.Size(257, 68);
            this.btnGenerateRSAKeys.TabIndex = 2;
            this.btnGenerateRSAKeys.Text = "Generate";
            this.btnGenerateRSAKeys.UseVisualStyleBackColor = true;
            this.btnGenerateRSAKeys.Click += new System.EventHandler(this.btnGenerateRSAKeys_Click);
            // 
            // txtPrivateKey
            // 
            this.txtPrivateKey.Location = new System.Drawing.Point(796, 181);
            this.txtPrivateKey.Multiline = true;
            this.txtPrivateKey.Name = "txtPrivateKey";
            this.txtPrivateKey.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtPrivateKey.Size = new System.Drawing.Size(688, 723);
            this.txtPrivateKey.TabIndex = 1;
            // 
            // txtPublicKey
            // 
            this.txtPublicKey.Location = new System.Drawing.Point(49, 181);
            this.txtPublicKey.Multiline = true;
            this.txtPublicKey.Name = "txtPublicKey";
            this.txtPublicKey.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtPublicKey.Size = new System.Drawing.Size(704, 723);
            this.txtPublicKey.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1575, 1108);
            this.tabControl1.TabIndex = 0;
            // 
            // btnSaveRSAPublicKey
            // 
            this.btnSaveRSAPublicKey.Location = new System.Drawing.Point(262, 938);
            this.btnSaveRSAPublicKey.Name = "btnSaveRSAPublicKey";
            this.btnSaveRSAPublicKey.Size = new System.Drawing.Size(159, 48);
            this.btnSaveRSAPublicKey.TabIndex = 3;
            this.btnSaveRSAPublicKey.Text = "Save As...";
            this.btnSaveRSAPublicKey.UseVisualStyleBackColor = true;
            this.btnSaveRSAPublicKey.Click += new System.EventHandler(this.btnSaveRSAPublicKey_Click);
            // 
            // btnSaveRSAPrivateKey
            // 
            this.btnSaveRSAPrivateKey.Location = new System.Drawing.Point(1032, 938);
            this.btnSaveRSAPrivateKey.Name = "btnSaveRSAPrivateKey";
            this.btnSaveRSAPrivateKey.Size = new System.Drawing.Size(159, 48);
            this.btnSaveRSAPrivateKey.TabIndex = 4;
            this.btnSaveRSAPrivateKey.Text = "Save As...";
            this.btnSaveRSAPrivateKey.UseVisualStyleBackColor = true;
            this.btnSaveRSAPrivateKey.Click += new System.EventHandler(this.btnSaveRSAPrivateKey_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1575, 1108);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "DND Cryptography";
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TextBox txtPrivateKey;
        private System.Windows.Forms.TextBox txtPublicKey;
        private System.Windows.Forms.Button btnGenerateRSAKeys;
        private System.Windows.Forms.Button btnSaveRSAPrivateKey;
        private System.Windows.Forms.Button btnSaveRSAPublicKey;
    }
}

