namespace CtLists
{
    partial class CtLists
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
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label3;
            this.button1 = new System.Windows.Forms.Button();
            this.m_lbxLocation = new System.Windows.Forms.CheckedListBox();
            this.m_lbxColor = new System.Windows.Forms.CheckedListBox();
            this.button2 = new System.Windows.Forms.Button();
            this.m_fVarietalGrouping = new System.Windows.Forms.CheckBox();
            this.m_ebOutFile = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 83);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(70, 20);
            label1.TabIndex = 3;
            label1.Text = "Location";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(273, 83);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(46, 20);
            label2.TabIndex = 4;
            label2.Text = "Color";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(546, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(106, 34);
            this.button1.TabIndex = 0;
            this.button1.Text = "Load Cellar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.DoTestIt);
            // 
            // m_lbxLocation
            // 
            this.m_lbxLocation.CheckOnClick = true;
            this.m_lbxLocation.FormattingEnabled = true;
            this.m_lbxLocation.Location = new System.Drawing.Point(12, 106);
            this.m_lbxLocation.Name = "m_lbxLocation";
            this.m_lbxLocation.Size = new System.Drawing.Size(246, 211);
            this.m_lbxLocation.TabIndex = 1;
            // 
            // m_lbxColor
            // 
            this.m_lbxColor.CheckOnClick = true;
            this.m_lbxColor.FormattingEnabled = true;
            this.m_lbxColor.Location = new System.Drawing.Point(277, 106);
            this.m_lbxColor.Name = "m_lbxColor";
            this.m_lbxColor.Size = new System.Drawing.Size(246, 211);
            this.m_lbxColor.TabIndex = 2;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(546, 60);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(106, 34);
            this.button2.TabIndex = 5;
            this.button2.Text = "Make List";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.MakeList);
            // 
            // m_fVarietalGrouping
            // 
            this.m_fVarietalGrouping.AutoSize = true;
            this.m_fVarietalGrouping.Location = new System.Drawing.Point(12, 323);
            this.m_fVarietalGrouping.Name = "m_fVarietalGrouping";
            this.m_fVarietalGrouping.Size = new System.Drawing.Size(158, 24);
            this.m_fVarietalGrouping.TabIndex = 6;
            this.m_fVarietalGrouping.Text = "Group by Varietal";
            this.m_fVarietalGrouping.UseVisualStyleBackColor = true;
            // 
            // m_ebOutFile
            // 
            this.m_ebOutFile.Location = new System.Drawing.Point(76, 12);
            this.m_ebOutFile.Name = "m_ebOutFile";
            this.m_ebOutFile.Size = new System.Drawing.Size(447, 26);
            this.m_ebOutFile.TabIndex = 7;
            this.m_ebOutFile.Text = "c:\\temp\\winelist2.html";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(12, 15);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(58, 20);
            label3.TabIndex = 8;
            label3.Text = "Output";
            // 
            // CtLists
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 396);
            this.Controls.Add(label3);
            this.Controls.Add(this.m_ebOutFile);
            this.Controls.Add(this.m_fVarietalGrouping);
            this.Controls.Add(this.button2);
            this.Controls.Add(label2);
            this.Controls.Add(label1);
            this.Controls.Add(this.m_lbxColor);
            this.Controls.Add(this.m_lbxLocation);
            this.Controls.Add(this.button1);
            this.Name = "CtLists";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckedListBox m_lbxLocation;
        private System.Windows.Forms.CheckedListBox m_lbxColor;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox m_fVarietalGrouping;
        private System.Windows.Forms.TextBox m_ebOutFile;
    }
}

