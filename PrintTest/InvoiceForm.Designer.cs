namespace PrintTest
{
    partial class InvoiceForm
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
            DgItems = new DataGridView();
            btnPrint = new Button();
            ((System.ComponentModel.ISupportInitialize)DgItems).BeginInit();
            SuspendLayout();
            // 
            // DgItems
            // 
            DgItems.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DgItems.Location = new Point(35, 75);
            DgItems.Name = "DgItems";
            DgItems.Size = new Size(657, 287);
            DgItems.TabIndex = 3;
            DgItems.CellContentClick += DgItems_CellContentClick;
            // 
            // btnPrint
            // 
            btnPrint.Location = new Point(35, 391);
            btnPrint.Name = "btnPrint";
            btnPrint.Size = new Size(75, 23);
            btnPrint.TabIndex = 4;
            btnPrint.Text = "Pirnt";
            btnPrint.UseVisualStyleBackColor = true;
            btnPrint.Click += btnPrint_Click;
            // 
            // InvoiceForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnPrint);
            Controls.Add(DgItems);
            Name = "InvoiceForm";
            Text = "InvoiceForm";
            Load += InvoiceForm_Load;
            ((System.ComponentModel.ISupportInitialize)DgItems).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView DgItems;
        private Button btnPrint;
    }
}