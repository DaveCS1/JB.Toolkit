namespace JBToolkit.WinForms
{
    partial class CustomMessageBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomMessageBox));
            this.btnOk = new JBToolkit.WinForms.RoundButton();
            this.btnCancel = new JBToolkit.WinForms.RoundButton();
            this.pbIcon = new System.Windows.Forms.PictureBox();
            this.lblMessage = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Active1 = System.Drawing.Color.SkyBlue;
            this.btnOk.Active2 = System.Drawing.SystemColors.GradientInactiveCaption;
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.BackColor = System.Drawing.Color.Transparent;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.btnOk.ForeColor = System.Drawing.Color.Black;
            this.btnOk.Image = null;
            this.btnOk.Inactive1 = System.Drawing.SystemColors.GradientInactiveCaption;
            this.btnOk.Inactive2 = System.Drawing.SystemColors.GradientActiveCaption;
            this.btnOk.Location = new System.Drawing.Point(395, 159);
            this.btnOk.Name = "btnOk";
            this.btnOk.Radius = 4;
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.Stroke = true;
            this.btnOk.StrokeColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "OK";
            this.btnOk.Transparency = false;
            this.btnOk.Click += new System.EventHandler(this.BtnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Active1 = System.Drawing.Color.SkyBlue;
            this.btnCancel.Active2 = System.Drawing.SystemColors.GradientInactiveCaption;
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.Black;
            this.btnCancel.Image = null;
            this.btnCancel.Inactive1 = System.Drawing.SystemColors.GradientInactiveCaption;
            this.btnCancel.Inactive2 = System.Drawing.SystemColors.GradientActiveCaption;
            this.btnCancel.Location = new System.Drawing.Point(476, 159);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Radius = 4;
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.Stroke = true;
            this.btnCancel.StrokeColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Transparency = false;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // pbIcon
            // 
            this.pbIcon.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.pbIcon.Location = new System.Drawing.Point(26, 89);
            this.pbIcon.Name = "pbIcon";
            this.pbIcon.Size = new System.Drawing.Size(48, 48);
            this.pbIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbIcon.TabIndex = 4;
            this.pbIcon.TabStop = false;
            // 
            // lblMessage
            // 
            this.lblMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMessage.Location = new System.Drawing.Point(93, 76);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(460, 73);
            this.lblMessage.TabIndex = 5;
            this.lblMessage.Text = resources.GetString("lblMessage.Text");
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CustomMessageBox
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(568, 195);
            this.ControlBox = false;
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.pbIcon);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CustomMessageBox";
            this.Resizable = false;
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.AeroShadow;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Style = MetroFramework.MetroColorStyle.Orange;
            this.Text = "Custom Message Box Title";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.CustomMessageBox_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private JBToolkit.WinForms.RoundButton btnOk;
        private JBToolkit.WinForms.RoundButton btnCancel;
        private System.Windows.Forms.PictureBox pbIcon;
        private System.Windows.Forms.Label lblMessage;
    }
}