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
            this.m_headingWineList = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.m_lbxLocation = new System.Windows.Forms.CheckedListBox();
            this.m_lbxColor = new System.Windows.Forms.CheckedListBox();
            this.button2 = new System.Windows.Forms.Button();
            this.m_fVarietalGrouping = new System.Windows.Forms.CheckBox();
            this.m_ebOutFile = new System.Windows.Forms.TextBox();
            this.m_pbUpdateSql = new System.Windows.Forms.Button();
            this.m_cbFixLeadingZeros = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.m_recStatus = new System.Windows.Forms.RichTextBox();
            this.m_pbDrinkWines = new System.Windows.Forms.Button();
            this.m_pbReloWines = new System.Windows.Forms.Button();
            this.m_headingCellarTrackerUpdate = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(11, 78);
            label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(48, 13);
            label1.TabIndex = 3;
            label1.Text = "Location";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(185, 78);
            label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(31, 13);
            label2.TabIndex = 4;
            label2.Text = "Color";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(11, 34);
            label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(39, 13);
            label3.TabIndex = 8;
            label3.Text = "Output";
            // 
            // m_headingWineList
            // 
            this.m_headingWineList.Location = new System.Drawing.Point(8, 6);
            this.m_headingWineList.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.m_headingWineList.Name = "m_headingWineList";
            this.m_headingWineList.Size = new System.Drawing.Size(434, 15);
            this.m_headingWineList.TabIndex = 31;
            this.m_headingWineList.Tag = "Wine List Generation";
            this.m_headingWineList.Text = "Wine List Generation";
            this.m_headingWineList.Paint += new System.Windows.Forms.PaintEventHandler(this.RenderHeadingLine);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(371, 23);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(71, 22);
            this.button1.TabIndex = 0;
            this.button1.Text = "Load Cellar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.DoDownloadCellar);
            // 
            // m_lbxLocation
            // 
            this.m_lbxLocation.CheckOnClick = true;
            this.m_lbxLocation.FormattingEnabled = true;
            this.m_lbxLocation.Location = new System.Drawing.Point(11, 93);
            this.m_lbxLocation.Margin = new System.Windows.Forms.Padding(2);
            this.m_lbxLocation.Name = "m_lbxLocation";
            this.m_lbxLocation.Size = new System.Drawing.Size(165, 139);
            this.m_lbxLocation.TabIndex = 1;
            // 
            // m_lbxColor
            // 
            this.m_lbxColor.CheckOnClick = true;
            this.m_lbxColor.FormattingEnabled = true;
            this.m_lbxColor.Location = new System.Drawing.Point(187, 93);
            this.m_lbxColor.Margin = new System.Windows.Forms.Padding(2);
            this.m_lbxColor.Name = "m_lbxColor";
            this.m_lbxColor.Size = new System.Drawing.Size(165, 139);
            this.m_lbxColor.TabIndex = 2;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(371, 49);
            this.button2.Margin = new System.Windows.Forms.Padding(2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(71, 22);
            this.button2.TabIndex = 5;
            this.button2.Text = "Make List";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.MakeList);
            // 
            // m_fVarietalGrouping
            // 
            this.m_fVarietalGrouping.AutoSize = true;
            this.m_fVarietalGrouping.Location = new System.Drawing.Point(11, 234);
            this.m_fVarietalGrouping.Margin = new System.Windows.Forms.Padding(2);
            this.m_fVarietalGrouping.Name = "m_fVarietalGrouping";
            this.m_fVarietalGrouping.Size = new System.Drawing.Size(107, 17);
            this.m_fVarietalGrouping.TabIndex = 6;
            this.m_fVarietalGrouping.Text = "Group by Varietal";
            this.m_fVarietalGrouping.UseVisualStyleBackColor = true;
            // 
            // m_ebOutFile
            // 
            this.m_ebOutFile.Location = new System.Drawing.Point(53, 32);
            this.m_ebOutFile.Margin = new System.Windows.Forms.Padding(2);
            this.m_ebOutFile.Name = "m_ebOutFile";
            this.m_ebOutFile.Size = new System.Drawing.Size(299, 20);
            this.m_ebOutFile.TabIndex = 7;
            this.m_ebOutFile.Text = "c:\\temp\\winelist2.html";
            // 
            // m_pbUpdateSql
            // 
            this.m_pbUpdateSql.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_pbUpdateSql.Location = new System.Drawing.Point(201, 274);
            this.m_pbUpdateSql.Margin = new System.Windows.Forms.Padding(2);
            this.m_pbUpdateSql.Name = "m_pbUpdateSql";
            this.m_pbUpdateSql.Size = new System.Drawing.Size(85, 22);
            this.m_pbUpdateSql.TabIndex = 9;
            this.m_pbUpdateSql.Text = "Sync With CT";
            this.m_pbUpdateSql.UseVisualStyleBackColor = true;
            this.m_pbUpdateSql.Click += new System.EventHandler(this.UpdateSql);
            // 
            // m_cbFixLeadingZeros
            // 
            this.m_cbFixLeadingZeros.AutoSize = true;
            this.m_cbFixLeadingZeros.Location = new System.Drawing.Point(11, 279);
            this.m_cbFixLeadingZeros.Margin = new System.Windows.Forms.Padding(2);
            this.m_cbFixLeadingZeros.Name = "m_cbFixLeadingZeros";
            this.m_cbFixLeadingZeros.Size = new System.Drawing.Size(116, 17);
            this.m_cbFixLeadingZeros.TabIndex = 10;
            this.m_cbFixLeadingZeros.Text = "Fix Leading Zeroes";
            this.m_cbFixLeadingZeros.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.m_recStatus);
            this.groupBox2.Location = new System.Drawing.Point(4, 313);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(438, 171);
            this.groupBox2.TabIndex = 28;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Status";
            // 
            // m_recStatus
            // 
            this.m_recStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_recStatus.Location = new System.Drawing.Point(7, 19);
            this.m_recStatus.Margin = new System.Windows.Forms.Padding(2);
            this.m_recStatus.Name = "m_recStatus";
            this.m_recStatus.Size = new System.Drawing.Size(428, 149);
            this.m_recStatus.TabIndex = 0;
            this.m_recStatus.Text = "";
            // 
            // m_pbDrinkWines
            // 
            this.m_pbDrinkWines.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_pbDrinkWines.Location = new System.Drawing.Point(290, 274);
            this.m_pbDrinkWines.Margin = new System.Windows.Forms.Padding(2);
            this.m_pbDrinkWines.Name = "m_pbDrinkWines";
            this.m_pbDrinkWines.Size = new System.Drawing.Size(71, 22);
            this.m_pbDrinkWines.TabIndex = 29;
            this.m_pbDrinkWines.Text = "Drink Wines";
            this.m_pbDrinkWines.UseVisualStyleBackColor = true;
            this.m_pbDrinkWines.Click += new System.EventHandler(this.DoDrinkWines);
            // 
            // m_pbReloWines
            // 
            this.m_pbReloWines.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_pbReloWines.Location = new System.Drawing.Point(365, 274);
            this.m_pbReloWines.Margin = new System.Windows.Forms.Padding(2);
            this.m_pbReloWines.Name = "m_pbReloWines";
            this.m_pbReloWines.Size = new System.Drawing.Size(71, 22);
            this.m_pbReloWines.TabIndex = 30;
            this.m_pbReloWines.Text = "Relo Wines";
            this.m_pbReloWines.UseVisualStyleBackColor = true;
            this.m_pbReloWines.Click += new System.EventHandler(this.DoRelocateWines);
            // 
            // m_headingCellarTrackerUpdate
            // 
            this.m_headingCellarTrackerUpdate.Location = new System.Drawing.Point(8, 257);
            this.m_headingCellarTrackerUpdate.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.m_headingCellarTrackerUpdate.Name = "m_headingCellarTrackerUpdate";
            this.m_headingCellarTrackerUpdate.Size = new System.Drawing.Size(434, 15);
            this.m_headingCellarTrackerUpdate.TabIndex = 32;
            this.m_headingCellarTrackerUpdate.Tag = "CellarTracker Update";
            this.m_headingCellarTrackerUpdate.Text = "CellarTracker Update";
            this.m_headingCellarTrackerUpdate.Paint += new System.Windows.Forms.PaintEventHandler(this.RenderHeadingLine);
            // 
            // CtLists
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 492);
            this.Controls.Add(this.m_headingCellarTrackerUpdate);
            this.Controls.Add(this.m_headingWineList);
            this.Controls.Add(this.m_pbReloWines);
            this.Controls.Add(this.m_cbFixLeadingZeros);
            this.Controls.Add(this.m_pbUpdateSql);
            this.Controls.Add(this.m_pbDrinkWines);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(label3);
            this.Controls.Add(this.m_ebOutFile);
            this.Controls.Add(this.m_fVarietalGrouping);
            this.Controls.Add(this.button2);
            this.Controls.Add(label2);
            this.Controls.Add(label1);
            this.Controls.Add(this.m_lbxColor);
            this.Controls.Add(this.m_lbxLocation);
            this.Controls.Add(this.button1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "CtLists";
            this.Text = "Form1";
            this.groupBox2.ResumeLayout(false);
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
        private System.Windows.Forms.Button m_pbUpdateSql;
        private System.Windows.Forms.CheckBox m_cbFixLeadingZeros;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RichTextBox m_recStatus;
        private System.Windows.Forms.Button m_pbDrinkWines;
        private System.Windows.Forms.Button m_pbReloWines;
        private System.Windows.Forms.Label m_headingWineList;
        private System.Windows.Forms.Label m_headingCellarTrackerUpdate;
    }
}

