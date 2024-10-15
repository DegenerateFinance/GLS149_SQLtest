using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
            // Remove the row at the clicked index
            DgItems.Rows.RemoveAt(e.RowIndex);
        }
    }

    private async void btnPrint_Click(object sender, EventArgs e)
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
        
        List<string> paginatedHtmlPages = await invoiceGenerator.GeneratePaginatedInvoicesHtml(invoice);

        // save html pages to disk
        for (int i = 0; i < paginatedHtmlPages.Count; i++)
        {
            string filePath = $"invoice_{i}.html";
            await File.WriteAllTextAsync(filePath, paginatedHtmlPages[i]);
        }


        //PdfGenerator pdfGenerator = new PdfGenerator();
        //string outputFilePath = "invoice.pdf";

        //pdfGenerator.GenerateMultiPagePdf(paginatedHtmlPages, outputFilePath);

        // save the file to disk

    }
}
