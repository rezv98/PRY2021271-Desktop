namespace ActMon.Forms
{
    partial class FormRegisterActivity
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRegisterActivity));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gBUser = new System.Windows.Forms.GroupBox();
            this.emailLabel = new System.Windows.Forms.Label();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.emailText = new System.Windows.Forms.TextBox();
            this.passwordText = new System.Windows.Forms.TextBox();
            this.gBUser.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // gBUser
            // 
            this.gBUser.Controls.Add(this.passwordText);
            this.gBUser.Controls.Add(this.emailText);
            this.gBUser.Controls.Add(this.passwordLabel);
            this.gBUser.Controls.Add(this.emailLabel);
            resources.ApplyResources(this.gBUser, "gBUser");
            this.gBUser.Name = "gBUser";
            this.gBUser.TabStop = false;
            // 
            // emailLabel
            // 
            resources.ApplyResources(this.emailLabel, "emailLabel");
            this.emailLabel.Name = "emailLabel";
            // 
            // passwordLabel
            // 
            resources.ApplyResources(this.passwordLabel, "passwordLabel");
            this.passwordLabel.Name = "passwordLabel";
            // 
            // emailText
            // 
            resources.ApplyResources(this.emailText, "emailText");
            this.emailText.Name = "emailText";
            // 
            // passwordText
            // 
            resources.ApplyResources(this.passwordText, "passwordText");
            this.passwordText.Name = "passwordText";
            // 
            // FormRegisterActivity
            // 
            this.AcceptButton = this.btnOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.gBUser);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormRegisterActivity";
            this.gBUser.ResumeLayout(false);
            this.gBUser.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox gBUser;
        private System.Windows.Forms.Label emailLabel;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.TextBox passwordText;
        private System.Windows.Forms.TextBox emailText;
    }
}