using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrintTest;

public partial class ScribanTest : Form
{
    private string barcodeColumnName = "ColBarcode";
    private string nameColumnName = "ColName";
    private string priceColumnName = "ColPrice";
    private string quantityColumnName = "ColQuantity";
    private string totalColumnName = "ColTotal";
    private string rmColumnName = "ColRM";
    public ScribanTest()
    {
        InitializeComponent();
    }

    private void ScribanTest_Load(object sender, EventArgs e)
    {
        DgItems.Columns.Add(barcodeColumnName, "Barcode");
        DgItems.Columns.Add(nameColumnName, "Name");
        DgItems.Columns.Add(priceColumnName, "Price");
        DgItems.Columns.Add(quantityColumnName, "Quantity");
        DgItems.Columns.Add(totalColumnName, "Total");

        // change ColRM columnType to DataGridViewButtonColumn
        DataGridViewButtonColumn btn = new DataGridViewButtonColumn();
        btn.HeaderText = "Action";
        btn.Text = "Delete";
        btn.Name = rmColumnName;
        DgItems.Columns.Add(btn);

        // add dummy data to the dg
        for (int i = 0; i < 50; i++)
        {
            DgItems.Rows.Add("Item " + i, i, i * 10);
        }

        // Set DataGridView properties
        DgItems.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
        // change ColTotal column background color to green
        DgItems.Columns["ColTotal"].DefaultCellStyle.BackColor = Color.LightGreen;
    }

    private void DgItems_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
        // Check if the clicked cell is in the Delete button column
        if (e.ColumnIndex == DgItems.Columns[rmColumnName].Index && e.RowIndex >= 0)
        {
            //if removable
            if (DgItems.Rows[e.RowIndex].Cells[barcodeColumnName].Value != null)
            {
                // Remove the row at the clicked index
                DgItems.Rows.RemoveAt(e.RowIndex);
            }
        }
    }

    private void btnPrint_Click(object sender, EventArgs e)
    {
        List<Item> items = new List<Item>();
        for (int i = 0; i < DgItems.Rows.Count; i++)
        {
            DataGridViewRow row = DgItems.Rows[i];
            items.Add(new Item
            (
                name: row.Cells[nameColumnName].Value?.ToString() ?? "",
                quantity: Convert.ToInt32(row.Cells[quantityColumnName].Value),
                price:Convert.ToDecimal(row.Cells[priceColumnName].Value)
            ));
        }

        InvoiceModel invoice = new InvoiceModel
        (
            clientName: "John Doe",
            invoiceDate:DateTime.Now.ToString("yyyy-MM-dd"),
            items: items,
            totalPrice: items.Sum(item => item.Price * item.Quantity)
        );

        InvoiceGenerator invoiceGenerator = new InvoiceGenerator();
        
        List<string> errList = new List<string>();

        List<string> paginatedHtmlPages = invoiceGenerator.GeneratePaginatedInvoicesHtml(invoice, ref errList);

        List<string> filePaths = new List<string>();
        // save html pages to disk
        for (int i = 0; i < paginatedHtmlPages.Count; i++)
        {
            string filePath = $"FicherosINI\\invoice_{i}.html";
            filePaths.Add(filePath);
            File.WriteAllText(filePath, paginatedHtmlPages[i]);
        }

        
        if (filePaths.Count > 0)
        {
            try
            {
                // get current directory
                string currentDirectory = Directory.GetCurrentDirectory();
                // get the path of the first file
                string filePath = Path.Combine(currentDirectory, filePaths[0]);

                //Process.Start(filePath);



                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Process.Start(new ProcessStartInfo("cmd", $"/c start chrome \"{filePath}\"") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", $"-a \"Google Chrome\" --args --kiosk-printing \"{filePaths[0]}\"");
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", filePaths[0]);
                    // Note: Linux doesn't have a standard way to invoke kiosk printing across all browsers
                }
                else
                {
                    throw new PlatformNotSupportedException("Unsupported operating system");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            //HtmlPrinter htmlPrinter = new HtmlPrinter(paginatedHtmlPages[0]);
            //htmlPrinter.PrintHtml();
        }


        //PdfGenerator pdfGenerator = new PdfGenerator();
        //string outputFilePath = "invoice.pdf";

        //pdfGenerator.GenerateMultiPagePdf(paginatedHtmlPages, outputFilePath);

        // save the file to disk

    }
}
