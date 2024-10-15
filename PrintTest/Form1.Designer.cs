namespace PrintTest
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            BtnPrint = new Button();
            BtnPrint2 = new Button();
            printDocument1 = new System.Drawing.Printing.PrintDocument();
            groupBox1 = new GroupBox();
            DgItems = new DataGridView();
            Barcode = new DataGridViewTextBoxColumn();
            ColDescription = new DataGridViewTextBoxColumn();
            ColPrice = new DataGridViewTextBoxColumn();
            ColQuantity = new DataGridViewTextBoxColumn();
            ColTotal = new DataGridViewTextBoxColumn();
            ColRemove = new DataGridViewButtonColumn();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DgItems).BeginInit();
            SuspendLayout();
            // 
            // BtnPrint
            // 
            BtnPrint.Location = new Point(490, 401);
            BtnPrint.Name = "BtnPrint";
            BtnPrint.Size = new Size(200, 80);
            BtnPrint.TabIndex = 0;
            BtnPrint.Text = "Print";
            BtnPrint.UseVisualStyleBackColor = true;
            BtnPrint.Click += button1_Click;
            // 
            // BtnPrint2
            // 
            BtnPrint2.Location = new Point(31, 384);
            BtnPrint2.Name = "BtnPrint2";
            BtnPrint2.Size = new Size(200, 80);
            BtnPrint2.TabIndex = 1;
            BtnPrint2.Text = "Print2";
            BtnPrint2.UseVisualStyleBackColor = true;
            BtnPrint2.Click += BtnPrint2_Click;
            // 
            // printDocument1
            // 
            printDocument1.PrintPage += printDocument1_PrintPage_1;
            // 
            // groupBox1
            // 
            groupBox1.BackgroundImage = (Image)resources.GetObject("groupBox1.BackgroundImage");
            groupBox1.BackgroundImageLayout = ImageLayout.Stretch;
            groupBox1.Controls.Add(DgItems);
            groupBox1.Controls.Add(BtnPrint2);
            groupBox1.Controls.Add(BtnPrint);
            groupBox1.Location = new Point(61, 43);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(736, 487);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "groupBox1";
            // 
            // DgItems
            // 
            DgItems.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DgItems.Columns.AddRange(new DataGridViewColumn[] { Barcode, ColDescription, ColPrice, ColQuantity, ColTotal, ColRemove });
            DgItems.Location = new Point(47, 91);
            DgItems.Name = "DgItems";
            DgItems.Size = new Size(657, 287);
            DgItems.TabIndex = 2;
            // 
            // Barcode
            // 
            Barcode.HeaderText = "Barcode";
            Barcode.Name = "Barcode";
            // 
            // ColDescription
            // 
            ColDescription.HeaderText = "Description";
            ColDescription.Name = "ColDescription";
            // 
            // ColPrice
            // 
            ColPrice.HeaderText = "Price";
            ColPrice.Name = "ColPrice";
            // 
            // ColQuantity
            // 
            ColQuantity.HeaderText = "Quantity";
            ColQuantity.Name = "ColQuantity";
            // 
            // ColTotal
            // 
            ColTotal.HeaderText = "Total";
            ColTotal.Name = "ColTotal";
            // 
            // ColRemove
            // 
            ColRemove.HeaderText = "X";
            ColRemove.Name = "ColRemove";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(993, 627);
            Controls.Add(groupBox1);
            Name = "Form1";
            Text = "Form1";
            groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)DgItems).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button BtnPrint;
        private Button BtnPrint2;
        private System.Drawing.Printing.PrintDocument printDocument1;
        private GroupBox groupBox1;
        private DataGridView DgItems;
        private DataGridViewTextBoxColumn Barcode;
        private DataGridViewTextBoxColumn ColDescription;
        private DataGridViewTextBoxColumn ColPrice;
        private DataGridViewTextBoxColumn ColQuantity;
        private DataGridViewTextBoxColumn ColTotal;
        private DataGridViewButtonColumn ColRemove;
    }
}
