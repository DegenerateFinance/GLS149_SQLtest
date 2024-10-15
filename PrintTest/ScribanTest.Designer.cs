namespace PrintTest
{
    partial class ScribanTest
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
            btnPrint = new Button();
            DgItems = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)DgItems).BeginInit();
            SuspendLayout();
            // 
            // btnPrint
            // 
            btnPrint.Location = new Point(72, 372);
            btnPrint.Name = "btnPrint";
            btnPrint.Size = new Size(75, 23);
            btnPrint.TabIndex = 6;
            btnPrint.Text = "Pirnt";
            btnPrint.UseVisualStyleBackColor = true;
            btnPrint.Click += btnPrint_Click;
            // 
            // DgItems
            // 
            DgItems.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DgItems.Location = new Point(72, 56);
            DgItems.Name = "DgItems";
            DgItems.Size = new Size(657, 287);
            DgItems.TabIndex = 5;
            DgItems.CellContentClick += DgItems_CellContentClick;
            // 
            // ScribanTest
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnPrint);
            Controls.Add(DgItems);
            Name = "ScribanTest";
            Text = "ScribanTest";
            Load += ScribanTest_Load;
            ((System.ComponentModel.ISupportInitialize)DgItems).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button btnPrint;
        private DataGridView DgItems;
    }
}