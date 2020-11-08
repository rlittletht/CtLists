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
            this.button1 = new System.Windows.Forms.Button();
            this.m_lbxLocation = new System.Windows.Forms.CheckedListBox();
            this.m_lbxColor = new System.Windows.Forms.CheckedListBox();
            this.button2 = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 36);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(70, 20);
            label1.TabIndex = 3;
            label1.Text = "Location";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(273, 36);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(46, 20);
            label2.TabIndex = 4;
            label2.Text = "Color";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(682, 12);
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
            this.m_lbxLocation.Location = new System.Drawing.Point(12, 59);
            this.m_lbxLocation.Name = "m_lbxLocation";
            this.m_lbxLocation.Size = new System.Drawing.Size(246, 211);
            this.m_lbxLocation.TabIndex = 1;
            // 
            // m_lbxColor
            // 
            this.m_lbxColor.CheckOnClick = true;
            this.m_lbxColor.FormattingEnabled = true;
            this.m_lbxColor.Location = new System.Drawing.Point(277, 59);
            this.m_lbxColor.Name = "m_lbxColor";
            this.m_lbxColor.Size = new System.Drawing.Size(246, 211);
            this.m_lbxColor.TabIndex = 2;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(682, 59);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(106, 34);
            this.button2.TabIndex = 5;
            this.button2.Text = "Make List";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.MakeList);
            // 
            // CtLists
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
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
    }
}

