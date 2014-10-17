namespace Ashp.AuthService.Harness
{
    partial class FormMain
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
            this.cmdIPAuthenticate = new System.Windows.Forms.Button();
            this.cmdAuthenticate = new System.Windows.Forms.Button();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtXML = new System.Windows.Forms.TextBox();
            this.txtAuthResult = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // cmdIPAuthenticate
            // 
            this.cmdIPAuthenticate.Location = new System.Drawing.Point(679, 32);
            this.cmdIPAuthenticate.Name = "cmdIPAuthenticate";
            this.cmdIPAuthenticate.Size = new System.Drawing.Size(75, 23);
            this.cmdIPAuthenticate.TabIndex = 21;
            this.cmdIPAuthenticate.Text = "IP Auth";
            this.cmdIPAuthenticate.UseVisualStyleBackColor = true;
            // 
            // cmdAuthenticate
            // 
            this.cmdAuthenticate.Location = new System.Drawing.Point(679, 6);
            this.cmdAuthenticate.Name = "cmdAuthenticate";
            this.cmdAuthenticate.Size = new System.Drawing.Size(75, 23);
            this.cmdAuthenticate.TabIndex = 20;
            this.cmdAuthenticate.Text = "Authenticate";
            this.cmdAuthenticate.UseVisualStyleBackColor = true;
            this.cmdAuthenticate.Click += new System.EventHandler(this.cmdAuthenticate_Click);
            // 
            // txtPass
            // 
            this.txtPassword.Location = new System.Drawing.Point(570, 32);
            this.txtPassword.Name = "txtPass";
            this.txtPassword.Size = new System.Drawing.Size(100, 20);
            this.txtPassword.TabIndex = 18;
            this.txtPassword.Text = "aw3se4";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(532, 35);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(33, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Pass:";
            // 
            // txtLogon
            // 
            this.txtUsername.Location = new System.Drawing.Point(570, 6);
            this.txtUsername.Name = "txtLogon";
            this.txtUsername.Size = new System.Drawing.Size(100, 20);
            this.txtUsername.TabIndex = 19;
            this.txtUsername.Text = "clebioda@ashp.org.test";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(529, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "Login:";
            // 
            // txtXML
            // 
            this.txtXML.Location = new System.Drawing.Point(535, 237);
            this.txtXML.Multiline = true;
            this.txtXML.Name = "txtXML";
            this.txtXML.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtXML.Size = new System.Drawing.Size(219, 103);
            this.txtXML.TabIndex = 22;
            this.txtXML.Visible = false;
            // 
            // txtAuthResult
            // 
            this.txtAuthResult.Location = new System.Drawing.Point(535, 71);
            this.txtAuthResult.Multiline = true;
            this.txtAuthResult.Name = "txtAuthResult";
            this.txtAuthResult.Size = new System.Drawing.Size(219, 141);
            this.txtAuthResult.TabIndex = 23;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(758, 465);
            this.Controls.Add(this.txtAuthResult);
            this.Controls.Add(this.txtXML);
            this.Controls.Add(this.cmdIPAuthenticate);
            this.Controls.Add(this.cmdAuthenticate);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.label5);
            this.Name = "FormMain";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdIPAuthenticate;
        private System.Windows.Forms.Button cmdAuthenticate;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtXML;
        private System.Windows.Forms.TextBox txtAuthResult;
    }
}

