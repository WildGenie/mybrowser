namespace NCP_Browser
{
    partial class TimeCard
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
            this.timeCardBrowser = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // timeCardBrowser
            // 
            this.timeCardBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timeCardBrowser.Location = new System.Drawing.Point(0, 0);
            this.timeCardBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.timeCardBrowser.Name = "timeCardBrowser";
            this.timeCardBrowser.ScriptErrorsSuppressed = true;
            this.timeCardBrowser.Size = new System.Drawing.Size(1432, 873);
            this.timeCardBrowser.TabIndex = 0;
            this.timeCardBrowser.Url = new System.Uri("https://ncpfinance.attendanceondemand.com/ess/DEFAULT", System.UriKind.Absolute);
            // 
            // TimeCard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1432, 873);
            this.Controls.Add(this.timeCardBrowser);
            this.Name = "TimeCard";
            this.Text = "TimeCard";
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.WebBrowser timeCardBrowser;

    }
}