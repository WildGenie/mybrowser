namespace NCP_Browser
{
    partial class Salesforce
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
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.closeSalesforceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshSalesforceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadSalesforceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nCPTimeCardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.suspenseLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lMSDowntimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customerLookupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loanDetailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loanLookupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loanNotesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openWindows = new System.Windows.Forms.ToolStripMenuItem();
            this.internetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exampleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.example2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.example3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wikiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Docker = new System.Windows.Forms.ToolStripMenuItem();
            this.teestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showDevToolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CheckForUIUpdates = new System.Windows.Forms.Timer(this.components);
            this.ncpBrowser = new System.Windows.Forms.WebBrowser();
            this.panel1 = new System.Windows.Forms.Panel();
            this.timerTicker = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeSalesforceToolStripMenuItem,
            this.refreshSalesforceToolStripMenuItem,
            this.reloadSalesforceToolStripMenuItem,
            this.nCPTimeCardToolStripMenuItem,
            this.suspenseLogToolStripMenuItem,
            this.lMSDowntimeToolStripMenuItem,
            this.openWindows,
            this.internetToolStripMenuItem,
            this.wikiToolStripMenuItem,
            this.Docker,
            this.teestToolStripMenuItem,
            this.showDevToolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1074, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // closeSalesforceToolStripMenuItem
            // 
            this.closeSalesforceToolStripMenuItem.Name = "closeSalesforceToolStripMenuItem";
            this.closeSalesforceToolStripMenuItem.Size = new System.Drawing.Size(98, 20);
            this.closeSalesforceToolStripMenuItem.Text = "Close Salesforce";
            this.closeSalesforceToolStripMenuItem.Click += new System.EventHandler(this.closeSalesforceToolStripMenuItem_Click);
            // 
            // refreshSalesforceToolStripMenuItem
            // 
            this.refreshSalesforceToolStripMenuItem.Name = "refreshSalesforceToolStripMenuItem";
            this.refreshSalesforceToolStripMenuItem.Size = new System.Drawing.Size(110, 20);
            this.refreshSalesforceToolStripMenuItem.Text = "Refresh Salesforce";
            this.refreshSalesforceToolStripMenuItem.Click += new System.EventHandler(this.refreshSalesforceToolStripMenuItem_Click);
            // 
            // reloadSalesforceToolStripMenuItem
            // 
            this.reloadSalesforceToolStripMenuItem.Name = "reloadSalesforceToolStripMenuItem";
            this.reloadSalesforceToolStripMenuItem.Size = new System.Drawing.Size(105, 20);
            this.reloadSalesforceToolStripMenuItem.Text = "Reload Salesforce";
            this.reloadSalesforceToolStripMenuItem.Visible = false;
            this.reloadSalesforceToolStripMenuItem.Click += new System.EventHandler(this.reloadSalesforceToolStripMenuItem_Click);
            // 
            // nCPTimeCardToolStripMenuItem
            // 
            this.nCPTimeCardToolStripMenuItem.Name = "nCPTimeCardToolStripMenuItem";
            this.nCPTimeCardToolStripMenuItem.Size = new System.Drawing.Size(90, 20);
            this.nCPTimeCardToolStripMenuItem.Text = "NCP Time Card";
            this.nCPTimeCardToolStripMenuItem.Click += new System.EventHandler(this.nCPTimeCardToolStripMenuItem_Click);
            // 
            // suspenseLogToolStripMenuItem
            // 
            this.suspenseLogToolStripMenuItem.Name = "suspenseLogToolStripMenuItem";
            this.suspenseLogToolStripMenuItem.Size = new System.Drawing.Size(85, 20);
            this.suspenseLogToolStripMenuItem.Text = "Suspense Log";
            this.suspenseLogToolStripMenuItem.Click += new System.EventHandler(this.suspenseLogToolStripMenuItem_Click);
            // 
            // lMSDowntimeToolStripMenuItem
            // 
            this.lMSDowntimeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.customerLookupToolStripMenuItem,
            this.loanDetailToolStripMenuItem,
            this.loanLookupToolStripMenuItem,
            this.loanNotesToolStripMenuItem});
            this.lMSDowntimeToolStripMenuItem.Name = "lMSDowntimeToolStripMenuItem";
            this.lMSDowntimeToolStripMenuItem.Size = new System.Drawing.Size(88, 20);
            this.lMSDowntimeToolStripMenuItem.Text = "LMS Downtime";
            // 
            // customerLookupToolStripMenuItem
            // 
            this.customerLookupToolStripMenuItem.Name = "customerLookupToolStripMenuItem";
            this.customerLookupToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.customerLookupToolStripMenuItem.Text = "Customer Lookup";
            this.customerLookupToolStripMenuItem.Click += new System.EventHandler(this.customerLookupToolStripMenuItem_Click);
            // 
            // loanDetailToolStripMenuItem
            // 
            this.loanDetailToolStripMenuItem.Name = "loanDetailToolStripMenuItem";
            this.loanDetailToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.loanDetailToolStripMenuItem.Text = "Loan Detail";
            this.loanDetailToolStripMenuItem.Click += new System.EventHandler(this.loanDetailToolStripMenuItem_Click);
            // 
            // loanLookupToolStripMenuItem
            // 
            this.loanLookupToolStripMenuItem.Name = "loanLookupToolStripMenuItem";
            this.loanLookupToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.loanLookupToolStripMenuItem.Text = "Loan Lookup";
            this.loanLookupToolStripMenuItem.Click += new System.EventHandler(this.loanLookupToolStripMenuItem_Click);
            // 
            // loanNotesToolStripMenuItem
            // 
            this.loanNotesToolStripMenuItem.Name = "loanNotesToolStripMenuItem";
            this.loanNotesToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.loanNotesToolStripMenuItem.Text = "Loan Notes";
            this.loanNotesToolStripMenuItem.Click += new System.EventHandler(this.loanNotesToolStripMenuItem_Click);
            // 
            // openWindows
            // 
            this.openWindows.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.openWindows.Name = "openWindows";
            this.openWindows.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.openWindows.Size = new System.Drawing.Size(91, 20);
            this.openWindows.Text = "Open Windows";
            this.openWindows.MouseEnter += new System.EventHandler(this.openWindows_MouseEnter);
            // 
            // internetToolStripMenuItem
            // 
            this.internetToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exampleToolStripMenuItem,
            this.example2ToolStripMenuItem,
            this.example3ToolStripMenuItem});
            this.internetToolStripMenuItem.Name = "internetToolStripMenuItem";
            this.internetToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.internetToolStripMenuItem.Text = "Internet";
            // 
            // exampleToolStripMenuItem
            // 
            this.exampleToolStripMenuItem.Enabled = false;
            this.exampleToolStripMenuItem.Name = "exampleToolStripMenuItem";
            this.exampleToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.exampleToolStripMenuItem.Text = "Example";
            this.exampleToolStripMenuItem.Click += new System.EventHandler(this.exampleToolStripMenuItem_Click);
            // 
            // example2ToolStripMenuItem
            // 
            this.example2ToolStripMenuItem.Enabled = false;
            this.example2ToolStripMenuItem.Name = "example2ToolStripMenuItem";
            this.example2ToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.example2ToolStripMenuItem.Text = "Example 2";
            this.example2ToolStripMenuItem.Click += new System.EventHandler(this.example2ToolStripMenuItem_Click);
            // 
            // example3ToolStripMenuItem
            // 
            this.example3ToolStripMenuItem.Enabled = false;
            this.example3ToolStripMenuItem.Name = "example3ToolStripMenuItem";
            this.example3ToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.example3ToolStripMenuItem.Text = "Example 3";
            this.example3ToolStripMenuItem.Click += new System.EventHandler(this.example3ToolStripMenuItem_Click);
            // 
            // wikiToolStripMenuItem
            // 
            this.wikiToolStripMenuItem.Name = "wikiToolStripMenuItem";
            this.wikiToolStripMenuItem.Size = new System.Drawing.Size(38, 20);
            this.wikiToolStripMenuItem.Text = "Wiki";
            this.wikiToolStripMenuItem.Click += new System.EventHandler(this.wikiToolStripMenuItem_Click);
            // 
            // Docker
            // 
            this.Docker.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.Docker.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.Docker.Name = "Docker";
            this.Docker.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Docker.Size = new System.Drawing.Size(58, 20);
            this.Docker.Text = "Un Dock";
            this.Docker.Click += new System.EventHandler(this.Docker_Click);
            // 
            // teestToolStripMenuItem
            // 
            this.teestToolStripMenuItem.Name = "teestToolStripMenuItem";
            this.teestToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this.teestToolStripMenuItem.Text = "QFund";
            this.teestToolStripMenuItem.Click += new System.EventHandler(this.teestToolStripMenuItem_Click);
            // 
            // showDevToolsToolStripMenuItem
            // 
            this.showDevToolsToolStripMenuItem.Name = "showDevToolsToolStripMenuItem";
            this.showDevToolsToolStripMenuItem.Size = new System.Drawing.Size(95, 20);
            this.showDevToolsToolStripMenuItem.Text = "Show Dev Tools";
            this.showDevToolsToolStripMenuItem.Click += new System.EventHandler(this.showDevToolsToolStripMenuItem_Click);
            // 
            // CheckForUIUpdates
            // 
            this.CheckForUIUpdates.Enabled = true;
            this.CheckForUIUpdates.Tick += new System.EventHandler(this.CheckForUIUpdates_Tick);
            // 
            // ncpBrowser
            // 
            this.ncpBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ncpBrowser.IsWebBrowserContextMenuEnabled = false;
            this.ncpBrowser.Location = new System.Drawing.Point(0, 24);
            this.ncpBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.ncpBrowser.Name = "ncpBrowser";
            this.ncpBrowser.Size = new System.Drawing.Size(801, 363);
            this.ncpBrowser.TabIndex = 0;
            this.ncpBrowser.Url = new System.Uri("https://test.salesforce.com", System.UriKind.Absolute);
            this.ncpBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.ncpBrowser_DocumentCompleted);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.menuStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1074, 387);
            this.panel1.TabIndex = 2;
            this.panel1.MouseEnter += new System.EventHandler(this.panel1_MouseEnter);
            // 
            // timerTicker
            // 
            this.timerTicker.Interval = 5000;
            this.timerTicker.Tick += new System.EventHandler(this.timerTicker_Tick);
            // 
            // Salesforce
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1074, 387);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Salesforce";
            this.Text = "NCP Web";
            this.Activated += new System.EventHandler(this.Salesforce_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Salesforce_FormClosing);
            this.Load += new System.EventHandler(this.Salesforce_Load);
            this.MouseEnter += new System.EventHandler(this.Salesforce_MouseEnter);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem nCPTimeCardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem internetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exampleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem example2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem example3ToolStripMenuItem;
        internal System.Windows.Forms.WebBrowser ncpBrowser;
        private System.Windows.Forms.ToolStripMenuItem suspenseLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lMSDowntimeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem customerLookupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loanDetailToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loanLookupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loanNotesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reloadSalesforceToolStripMenuItem;
        private System.Windows.Forms.Timer CheckForUIUpdates;
        internal System.Windows.Forms.ToolStripMenuItem openWindows;
        private System.Windows.Forms.ToolStripMenuItem closeSalesforceToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem refreshSalesforceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wikiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Docker;
        private System.Windows.Forms.ToolStripMenuItem teestToolStripMenuItem;
        private System.Windows.Forms.Timer timerTicker;
        private System.Windows.Forms.ToolStripMenuItem showDevToolsToolStripMenuItem;
    }
}

