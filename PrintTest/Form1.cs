using System.Drawing.Printing;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
namespace PrintTest;

public partial class Form1 : Form
{
    private PrintDocument printDocument = new PrintDocument();
    public Form1()
    {
        InitializeComponent();
        printDocument.PrintPage += new PrintPageEventHandler(PrintInvoice);
        
        // add dummy data to the dg
        for (int i = 0; i < 50; i++)
        {
            DgItems.Rows.Add("Item " + i, i, i * 10);
        }
    }

    private void button1_Click(object sender, EventArgs e)
    {
        PrintDialog printDialog = new PrintDialog();
        printDialog.Document = printDocument;

        if (printDialog.ShowDialog() == DialogResult.OK)
        {
            printDocument.Print();
        }
    }
    private void PrintInvoice(object sender, PrintPageEventArgs e)
    {
        // Define fonts, margins, and other printing variables
        Font font = new Font("Arial", 12);
        int startX = 50, startY = 50, offset = 40;

        // Sample Client Data
        string clientName = "Client Name: John Doe";
        string clientAddress = "Address: 123 Main Street, City";
        string invoiceDate = "Date: " + DateTime.Now.ToShortDateString();

        // Draw Client Data
        e.Graphics.DrawString(clientName, font, Brushes.Black, startX, startY);
        e.Graphics.DrawString(clientAddress, font, Brushes.Black, startX, startY + offset);
        e.Graphics.DrawString(invoiceDate, font, Brushes.Black, startX, startY + 2 * offset);

        // Draw Table Headers
        offset += 60;
        e.Graphics.DrawString("Item", font, Brushes.Black, startX, startY + offset);
        e.Graphics.DrawString("Quantity", font, Brushes.Black, startX + 200, startY + offset);
        e.Graphics.DrawString("Price", font, Brushes.Black, startX + 400, startY + offset);

        // Sample Item Data
        offset += 30;
        string[] items = { "Item 1", "Item 2", "Item 3" };
        int[] quantities = { 1, 2, 3 };
        decimal[] prices = { 10.00m, 20.00m, 30.00m };
        decimal total = 0;

        // Draw Item Rows
        for (int i = 0; i < items.Length; i++)
        {
            e.Graphics.DrawString(items[i], font, Brushes.Black, startX, startY + offset);
            e.Graphics.DrawString(quantities[i].ToString(), font, Brushes.Black, startX + 200, startY + offset);
            e.Graphics.DrawString(prices[i].ToString("C"), font, Brushes.Black, startX + 400, startY + offset);
            total += prices[i];
            offset += 30;
        }

        // Draw Total
        offset += 20;
        e.Graphics.DrawString("Total: " + total.ToString("C"), font, Brushes.Black, startX + 400, startY + offset);
    }
    private Bitmap memoryImage;
    private Bitmap dgImage;
    private void BtnPrint2_Click(object sender, EventArgs e)
    {
        Graphics myGraphics = this.CreateGraphics();
        //Size s = groupBox1.Size;
        Size s = groupBox1.ClientSize;
        memoryImage = new Bitmap(s.Width, s.Height);

        groupBox1.DrawToBitmap(memoryImage, new Rectangle(0, 0, s.Width, s.Height));

        dgImage = new Bitmap(DgItems.Width, DgItems.Height);
        DgItems.DrawToBitmap(dgImage, new Rectangle(0, 0, s.Width, s.Height));



        // Show Print Preview Dialog
        PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();
        printPreviewDialog.Document = printDocument1;

        // Optional: Set a larger preview dialog size
        printPreviewDialog.ClientSize = new Size(800, 600);

        // Show the preview dialog
        printPreviewDialog.ShowDialog();
    }

    private void printDocument1_PrintPage_1(object sender, PrintPageEventArgs e)
    {
        // Adjust the scaling for proper DPI if needed
        float scale = e.PageSettings.PrinterResolution.X / e.Graphics.DpiX;
        e.Graphics.ScaleTransform(scale, scale);

        // Draw the captured form image on the print document
        e.Graphics.DrawImage(dgImage, 0, 0);
    }
    private void BtnPrint_Click(object sender, EventArgs e)
    {
        // Standard Print Dialog
        PrintDialog printDialog = new PrintDialog();
        printDialog.Document = printDocument1;

        if (printDialog.ShowDialog() == DialogResult.OK)
        {
            printDocument1.Print();
        }
    }
}
