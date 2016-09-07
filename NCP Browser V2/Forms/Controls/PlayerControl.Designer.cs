namespace NCP_Browser.Forms.Controls
{
    partial class PlayerControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.playPausePic = new System.Windows.Forms.PictureBox();
            this.stopPic = new System.Windows.Forms.PictureBox();
            this.rightPic = new System.Windows.Forms.PictureBox();
            this.leftPic = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.playPausePic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.stopPic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightPic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftPic)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.playPausePic, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.stopPic, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.rightPic, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.leftPic, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(617, 150);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // playPausePic
            // 
            this.playPausePic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.playPausePic.Image = global::NCP_Browser.Properties.Resources.Pause;
            this.playPausePic.Location = new System.Drawing.Point(3, 3);
            this.playPausePic.Name = "playPausePic";
            this.playPausePic.Size = new System.Drawing.Size(148, 144);
            this.playPausePic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.playPausePic.TabIndex = 0;
            this.playPausePic.TabStop = false;
            // 
            // stopPic
            // 
            this.stopPic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stopPic.Image = global::NCP_Browser.Properties.Resources.Stop;
            this.stopPic.Location = new System.Drawing.Point(157, 3);
            this.stopPic.Name = "stopPic";
            this.stopPic.Size = new System.Drawing.Size(148, 144);
            this.stopPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.stopPic.TabIndex = 1;
            this.stopPic.TabStop = false;
            // 
            // rightPic
            // 
            this.rightPic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightPic.Image = global::NCP_Browser.Properties.Resources.SeekRight;
            this.rightPic.Location = new System.Drawing.Point(465, 3);
            this.rightPic.Name = "rightPic";
            this.rightPic.Size = new System.Drawing.Size(149, 144);
            this.rightPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.rightPic.TabIndex = 2;
            this.rightPic.TabStop = false;
            // 
            // leftPic
            // 
            this.leftPic.BackColor = System.Drawing.Color.Transparent;
            this.leftPic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.leftPic.Image = global::NCP_Browser.Properties.Resources.SeekLeft;
            this.leftPic.Location = new System.Drawing.Point(311, 3);
            this.leftPic.Name = "leftPic";
            this.leftPic.Size = new System.Drawing.Size(148, 144);
            this.leftPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.leftPic.TabIndex = 3;
            this.leftPic.TabStop = false;
            // 
            // PlayerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "PlayerControl";
            this.Size = new System.Drawing.Size(617, 150);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.playPausePic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.stopPic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightPic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftPic)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox playPausePic;
        private System.Windows.Forms.PictureBox stopPic;
        private System.Windows.Forms.PictureBox rightPic;
        private System.Windows.Forms.PictureBox leftPic;
    }
}
