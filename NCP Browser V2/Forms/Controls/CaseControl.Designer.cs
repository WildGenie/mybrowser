namespace NCP_Browser.Forms.Controls
{
    partial class CaseControl
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_CaseNumber = new System.Windows.Forms.TextBox();
            this.txt_LoanNumber = new System.Windows.Forms.TextBox();
            this.btn_Select = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_CustomerName = new System.Windows.Forms.TextBox();
            this.txt_CreationDate = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Case Number:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(33, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Loan Number:";
            // 
            // txt_CaseNumber
            // 
            this.txt_CaseNumber.BackColor = System.Drawing.Color.White;
            this.txt_CaseNumber.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_CaseNumber.Location = new System.Drawing.Point(113, 15);
            this.txt_CaseNumber.Name = "txt_CaseNumber";
            this.txt_CaseNumber.ReadOnly = true;
            this.txt_CaseNumber.Size = new System.Drawing.Size(195, 20);
            this.txt_CaseNumber.TabIndex = 2;
            this.txt_CaseNumber.Enter += new System.EventHandler(this.txt_CaseNumber_Enter);
            this.txt_CaseNumber.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txt_CaseNumber_MouseDown);
            this.txt_CaseNumber.MouseEnter += new System.EventHandler(this.txt_CaseNumber_MouseEnter);
            this.txt_CaseNumber.MouseMove += new System.Windows.Forms.MouseEventHandler(this.txt_CaseNumber_MouseMove);
            // 
            // txt_LoanNumber
            // 
            this.txt_LoanNumber.BackColor = System.Drawing.Color.White;
            this.txt_LoanNumber.Location = new System.Drawing.Point(113, 67);
            this.txt_LoanNumber.Name = "txt_LoanNumber";
            this.txt_LoanNumber.ReadOnly = true;
            this.txt_LoanNumber.Size = new System.Drawing.Size(195, 20);
            this.txt_LoanNumber.TabIndex = 3;
            this.txt_LoanNumber.Enter += new System.EventHandler(this.txt_CaseNumber_Enter);
            this.txt_LoanNumber.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txt_CaseNumber_MouseDown);
            this.txt_LoanNumber.MouseEnter += new System.EventHandler(this.txt_CaseNumber_MouseEnter);
            this.txt_LoanNumber.MouseMove += new System.Windows.Forms.MouseEventHandler(this.txt_CaseNumber_MouseMove);
            // 
            // btn_Select
            // 
            this.btn_Select.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.btn_Select.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.btn_Select.FlatAppearance.BorderSize = 3;
            this.btn_Select.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(0)))));
            this.btn_Select.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.btn_Select.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Select.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Select.ForeColor = System.Drawing.Color.White;
            this.btn_Select.Location = new System.Drawing.Point(314, 15);
            this.btn_Select.Name = "btn_Select";
            this.btn_Select.Size = new System.Drawing.Size(115, 98);
            this.btn_Select.TabIndex = 4;
            this.btn_Select.Text = "Attach Recording To Case";
            this.btn_Select.UseVisualStyleBackColor = false;
            this.btn_Select.Click += new System.EventHandler(this.btn_Select_Click);
            this.btn_Select.MouseEnter += new System.EventHandler(this.btn_Select_MouseEnter);
            this.btn_Select.MouseLeave += new System.EventHandler(this.btn_Select_MouseLeave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 91);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Customer Name:";
            // 
            // txt_CustomerName
            // 
            this.txt_CustomerName.BackColor = System.Drawing.Color.White;
            this.txt_CustomerName.Location = new System.Drawing.Point(113, 93);
            this.txt_CustomerName.Name = "txt_CustomerName";
            this.txt_CustomerName.ReadOnly = true;
            this.txt_CustomerName.Size = new System.Drawing.Size(195, 20);
            this.txt_CustomerName.TabIndex = 6;
            this.txt_CustomerName.Enter += new System.EventHandler(this.txt_CaseNumber_Enter);
            this.txt_CustomerName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txt_CaseNumber_MouseDown);
            this.txt_CustomerName.MouseEnter += new System.EventHandler(this.txt_CaseNumber_MouseEnter);
            this.txt_CustomerName.MouseMove += new System.Windows.Forms.MouseEventHandler(this.txt_CaseNumber_MouseMove);
            // 
            // txt_CreationDate
            // 
            this.txt_CreationDate.BackColor = System.Drawing.Color.White;
            this.txt_CreationDate.Location = new System.Drawing.Point(113, 41);
            this.txt_CreationDate.Name = "txt_CreationDate";
            this.txt_CreationDate.ReadOnly = true;
            this.txt_CreationDate.Size = new System.Drawing.Size(195, 20);
            this.txt_CreationDate.TabIndex = 7;
            this.txt_CreationDate.Enter += new System.EventHandler(this.txt_CaseNumber_Enter);
            this.txt_CreationDate.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txt_CaseNumber_MouseDown);
            this.txt_CreationDate.MouseEnter += new System.EventHandler(this.txt_CaseNumber_MouseEnter);
            this.txt_CreationDate.MouseMove += new System.Windows.Forms.MouseEventHandler(this.txt_CaseNumber_MouseMove);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 44);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(102, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Case Creation Date:";
            // 
            // CaseControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Yellow;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txt_CreationDate);
            this.Controls.Add(this.txt_CustomerName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btn_Select);
            this.Controls.Add(this.txt_LoanNumber);
            this.Controls.Add(this.txt_CaseNumber);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "CaseControl";
            this.Size = new System.Drawing.Size(437, 127);
            this.Load += new System.EventHandler(this.CaseControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.TextBox txt_CustomerName;
        public System.Windows.Forms.TextBox txt_CaseNumber;
        public System.Windows.Forms.TextBox txt_LoanNumber;
        public System.Windows.Forms.Button btn_Select;
        public System.Windows.Forms.TextBox txt_CreationDate;
        private System.Windows.Forms.Label label4;
    }
}
