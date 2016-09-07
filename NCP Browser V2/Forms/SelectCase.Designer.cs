using System.Windows.Forms;
namespace NCP_Browser.Forms
{
    partial class SelectCase
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btn_Close = new System.Windows.Forms.Button();
            this.btn_Discard = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.playerControl1 = new NCP_Browser.Forms.Controls.PlayerControl();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(518, 374);
            this.panel1.TabIndex = 0;
            // 
            // btn_Close
            // 
            this.btn_Close.BackColor = System.Drawing.Color.Yellow;
            this.btn_Close.FlatAppearance.BorderColor = System.Drawing.Color.Yellow;
            this.btn_Close.FlatAppearance.BorderSize = 2;
            this.btn_Close.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.btn_Close.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Yellow;
            this.btn_Close.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Close.Location = new System.Drawing.Point(12, 380);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(129, 40);
            this.btn_Close.TabIndex = 1;
            this.btn_Close.Text = "I Need to Open the Case in Salesforce";
            this.btn_Close.UseVisualStyleBackColor = false;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            this.btn_Close.MouseEnter += new System.EventHandler(this.btn_Close_MouseEnter);
            this.btn_Close.MouseLeave += new System.EventHandler(this.btn_Close_MouseLeave);
            // 
            // btn_Discard
            // 
            this.btn_Discard.BackColor = System.Drawing.Color.Red;
            this.btn_Discard.FlatAppearance.BorderColor = System.Drawing.Color.Red;
            this.btn_Discard.FlatAppearance.BorderSize = 2;
            this.btn_Discard.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btn_Discard.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red;
            this.btn_Discard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Discard.ForeColor = System.Drawing.Color.Black;
            this.btn_Discard.Location = new System.Drawing.Point(147, 380);
            this.btn_Discard.Name = "btn_Discard";
            this.btn_Discard.Size = new System.Drawing.Size(129, 40);
            this.btn_Discard.TabIndex = 2;
            this.btn_Discard.Text = "Discard Call Recording";
            this.btn_Discard.UseVisualStyleBackColor = false;
            this.btn_Discard.Click += new System.EventHandler(this.btn_Discard_Click);
            this.btn_Discard.MouseEnter += new System.EventHandler(this.btn_Discard_MouseEnter);
            this.btn_Discard.MouseLeave += new System.EventHandler(this.btn_Discard_MouseLeave);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 373);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(494, 47);
            this.progressBar1.TabIndex = 0;
            this.progressBar1.Value = 50;
            this.progressBar1.Visible = false;
            // 
            // playerControl1
            // 
            this.playerControl1.Location = new System.Drawing.Point(282, 380);
            this.playerControl1.Name = "playerControl1";
            this.playerControl1.Size = new System.Drawing.Size(155, 40);
            this.playerControl1.TabIndex = 3;
            this.playerControl1.Visible = false;
            // 
            // SelectCase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(518, 425);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.playerControl1);
            this.Controls.Add(this.btn_Discard);
            this.Controls.Add(this.btn_Close);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectCase";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SelectCase";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.SelectCase_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private Button btn_Close;
        private Button btn_Discard;
        private ProgressBar progressBar1;
        private Controls.PlayerControl playerControl1;

    }
}