namespace NCP_Browser
{
    partial class Internet
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
            this.webNav = new System.Windows.Forms.ToolStrip();
            this.homeButton = new System.Windows.Forms.ToolStripButton();
            this.backButton = new System.Windows.Forms.ToolStripButton();
            this.forwardButton = new System.Windows.Forms.ToolStripButton();
            this.haltButton = new System.Windows.Forms.ToolStripButton();
            this.reloadButton = new System.Windows.Forms.ToolStripButton();
            this.internetBrowser = new System.Windows.Forms.WebBrowser();
            this.webNav.SuspendLayout();
            this.SuspendLayout();
            // 
            // webNav
            // 
            this.webNav.AutoSize = false;
            this.webNav.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.webNav.ImageScalingSize = new System.Drawing.Size(45, 45);
            this.webNav.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.homeButton,
            this.backButton,
            this.forwardButton,
            this.haltButton,
            this.reloadButton});
            this.webNav.Location = new System.Drawing.Point(0, 0);
            this.webNav.Name = "webNav";
            this.webNav.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.webNav.Size = new System.Drawing.Size(1432, 50);
            this.webNav.TabIndex = 0;
            this.webNav.Text = "toolStrip1";
            // 
            // homeButton
            // 
            this.homeButton.AutoSize = false;
            this.homeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.homeButton.Image = global::NCP_Browser.NCP_Browser_Resources.home;
            this.homeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.homeButton.Name = "homeButton";
            this.homeButton.Size = new System.Drawing.Size(50, 47);
            this.homeButton.Text = "toolStripButton4";
            this.homeButton.Click += new System.EventHandler(this.homeButton_Click);
            // 
            // backButton
            // 
            this.backButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.backButton.Image = global::NCP_Browser.NCP_Browser_Resources.back;
            this.backButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(49, 47);
            this.backButton.Text = "toolStripButton1";
            this.backButton.Click += new System.EventHandler(this.backButton_Click);
            // 
            // forwardButton
            // 
            this.forwardButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.forwardButton.Image = global::NCP_Browser.NCP_Browser_Resources.forward;
            this.forwardButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.forwardButton.Name = "forwardButton";
            this.forwardButton.Size = new System.Drawing.Size(49, 47);
            this.forwardButton.Text = "toolStripButton2";
            this.forwardButton.Click += new System.EventHandler(this.forwardButton_Click);
            // 
            // haltButton
            // 
            this.haltButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.haltButton.Image = global::NCP_Browser.NCP_Browser_Resources.halt;
            this.haltButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.haltButton.Name = "haltButton";
            this.haltButton.Size = new System.Drawing.Size(49, 47);
            this.haltButton.Text = "toolStripButton3";
            this.haltButton.Click += new System.EventHandler(this.haltButton_Click);
            // 
            // reloadButton
            // 
            this.reloadButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.reloadButton.Image = global::NCP_Browser.NCP_Browser_Resources.reload;
            this.reloadButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.reloadButton.Name = "reloadButton";
            this.reloadButton.Size = new System.Drawing.Size(49, 47);
            this.reloadButton.Text = "toolStripButton6";
            this.reloadButton.Click += new System.EventHandler(this.reloadButton_Click);
            // 
            // internetBrowser
            // 
            this.internetBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.internetBrowser.Location = new System.Drawing.Point(0, 50);
            this.internetBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.internetBrowser.Name = "internetBrowser";
            this.internetBrowser.ScriptErrorsSuppressed = true;
            this.internetBrowser.Size = new System.Drawing.Size(1432, 823);
            this.internetBrowser.TabIndex = 1;
            this.internetBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.internetBrowser_DocumentCompleted);
            this.internetBrowser.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.internetBrowser_Navigated);
            this.internetBrowser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.internetBrowser_Navigating);
            // 
            // Internet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1432, 873);
            this.Controls.Add(this.internetBrowser);
            this.Controls.Add(this.webNav);
            this.Name = "Internet";
            this.Text = "Internet";
            this.Load += new System.EventHandler(this.Internet_Load);
            this.webNav.ResumeLayout(false);
            this.webNav.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripButton backButton;
        private System.Windows.Forms.ToolStripButton forwardButton;
        private System.Windows.Forms.ToolStripButton haltButton;
        private System.Windows.Forms.ToolStripButton homeButton;
        private System.Windows.Forms.ToolStripButton reloadButton;
        internal System.Windows.Forms.WebBrowser internetBrowser;
        internal System.Windows.Forms.ToolStrip webNav;
    }
}