using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrintTest;

public partial class InvoiceForm : Form
{
    private PrintDocument printDocument = new PrintDocument();
    private int totalPages;
    private int currentPage;

    private string barcodeColumnName = "ColBarcode";
    private string nameColumnName = "ColName";
    private string priceColumnName = "ColPrice";
    private string quantityColumnName = "ColQuantity";
    private string totalColumnName = "ColTotal";
    private string rmColumnName = "ColRM";
    public InvoiceForm()
    {
        InitializeComponent();
        printDocument.PrintPage += new PrintPageEventHandler(printDocument1_PrintPage);
    }
    private void InvoiceForm_Load(object sender, EventArgs e)
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
        btn.Name = "btnDelete";
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
        if (e.ColumnIndex == DgItems.Columns["btnDelete"].Index && e.RowIndex >= 0)
        {
            // Remove the row at the clicked index
            DgItems.Rows.RemoveAt(e.RowIndex);
        }
    }
    private void PrintDocument_BeginPrint(object sender, PrintEventArgs e)
    {
        rowIndex = 0; // Reset row index
        totalPages = 1; // Initial number of pages, calculate later
        currentPage = 1; // Reset page counter
    }
    private int rowIndex = 0; // Keeps track of the current row being printed
    //private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
    //{
    //    // Margins and layout
    //    int marginTop = 50;
    //    int marginLeft = 50;
    //    int marginBottom = 50;
    //    int pageWidth = e.PageBounds.Width;
    //    int pageHeight = e.PageBounds.Height;
    //    int contentHeight = pageHeight - marginTop - marginBottom;
    //    int itemsHeight = 0;

    //    // 1. Draw the header (client info)
    //    DrawHeader(e.Graphics, marginLeft, marginTop);

    //    // 2.0 Draw DataGridView column headers on each page
    //    itemsHeight = DrawColumnHeaders(e.Graphics, marginLeft, marginTop + 100);

    //    // 2.1 Draw DataGridView items (handle multi-page)
    //    itemsHeight = DrawItems(e.Graphics, marginLeft, marginTop + 100, contentHeight);

    //    // 3. Draw footer (page number)
    //    DrawFooter(e.Graphics, marginLeft, pageHeight - marginBottom, currentPage, totalPages);

    //    // Check if there are more pages
    //    if (rowIndex < DgItems.Rows.Count)
    //    {
    //        e.HasMorePages = true;
    //        currentPage++;
    //    }
    //    else
    //    {
    //        // 4. Draw the order summary on the last page
    //        DrawOrderSummary(e.Graphics, marginLeft, itemsHeight + 50);
    //        e.HasMorePages = false;
    //    }
    //}
    private int currentPrintRow = 0;
    private int pageNumber = 1;
    private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
    {
        using (Graphics graphics = e.Graphics)
        {
            const int paperWidth = 800; // 3 inches width
            const int paperHeight = 1116; // 11 inches height
            const int margin = 25;
            const int headerHeight = 150;
            const int footerHeight = 100;
            const string fontName = "Arial";
            const int fontSize = 11;
            const FontStyle fontStyle = FontStyle.Regular;

            // get DataGridView font
            Font dgFont = DgItems.Font;

            using (Font font = new Font(fontName, fontSize, fontStyle))
            {
                using (StringFormat stringFormat = new StringFormat())
                {
                    stringFormat.Alignment = StringAlignment.Near;
                    stringFormat.LineAlignment = StringAlignment.Center;

                    // Draw header
                    DrawHeader(e.Graphics, font, fontStyle, stringFormat, margin, headerHeight, paperWidth, paperHeight);
                    // Draw body
                    DrawBody(e.Graphics, font, stringFormat, margin, headerHeight, footerHeight, paperHeight, e);
                    // Draw footer
                    DrawFooter(e.Graphics, font, stringFormat, margin, footerHeight, paperWidth, paperHeight, pageNumber);
                }
            }
        }

        if (e.HasMorePages)
        {
            pageNumber++;
        }
        else
        {
            pageNumber = 1;
            currentPrintRow = 0;
        }
    }
    private void DrawHeader(Graphics graphics, Font font, FontStyle fs, StringFormat stringFormat, int margin, int headerHeight, int paperWidth, int paperHeight)
    {
        //// Draw logo
        //Bitmap logo = Properties.Resources.csh;
        //graphics.DrawImage(logo, margin, margin, 120, 120);

        // Draw header text
        string headerText = "C# Solutions Hub";
        string headerText1 = "PO Box 38, Avatip, Sepik Wara";
        string headerText2 = "Wewak, ESP 38";
        int textY = margin + 20; // Add 20 pixels of spacing between the logo and the text
        int lineHeight = 20; // Line height for each header text

        // Create a bold font
        using (Font boldFont = new Font(font.FontFamily, font.Size, FontStyle.Bold))
        {
            graphics.DrawString(headerText, boldFont, Brushes.Black, new Rectangle(margin, textY, paperWidth - margin * 2, headerHeight), stringFormat);
        }

        graphics.DrawString(headerText1, font, Brushes.Black, new Rectangle(margin, textY + lineHeight, paperWidth - margin * 2, headerHeight), stringFormat);
        graphics.DrawString(headerText2, font, Brushes.Black, new Rectangle(margin, textY + lineHeight * 2, paperWidth - margin * 2, headerHeight), stringFormat);
    }
    private void DrawBody(Graphics graphics, Font font, StringFormat stringFormat, int margin, int headerHeight, int footerHeight, int paperHeight, PrintPageEventArgs e)
    {
        int startY = headerHeight + margin;
        int itemsPerPage = (paperHeight - headerHeight - footerHeight - margin * 2) / 25;

        // Draw column names
        DrawColumnNames(graphics, font, stringFormat, margin, startY);

        startY += 25; // Move down to make space for the column names
        try
        {
            for (int i = currentPrintRow; i < DgItems.Rows.Count; i++)
            {
            
                DataGridViewRow row = DgItems.Rows[i];

                if (row.Cells[barcodeColumnName].Value != null)
                {
                    // Draw row data
                    DrawRowData(graphics, font, stringFormat, margin, startY, row);
                    startY += 25;

                    if (startY > paperHeight - footerHeight - margin)
                    {
                        e.HasMorePages = true; // Set HasMorePages to true
                        currentPrintRow = i + 1; // Increment currentPrintRow
                        return; // Return to raise the PrintPage event again
                    }
                }
            
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
    private int rowHeight = 25;
    private int barcodeColumnPrintWidth = 200;
    private int nameColumnPrintWidth = 150;
    private int priceColumnPrintWidth = 100;
    private int quantityColumnPrintWidth = 100;
    private int totalColumnPrintWidth = 100;

    private void DrawColumnNames(Graphics graphics, Font font, StringFormat stringFormat, int margin, int startY)
    {
        int initialMargin = margin;
        Font boldFont = new Font(font.FontFamily, font.Size, FontStyle.Bold);

        
        Pen pen = new Pen(Brushes.Black, 2);
        
        // Draw horizontal grid line
        graphics.DrawLine(pen, margin, startY, margin, startY + rowHeight);

        graphics.DrawString(DgItems.Columns[barcodeColumnName].HeaderText, boldFont, Brushes.Black, new Rectangle(margin, startY, DgItems.Columns[barcodeColumnName].Width, rowHeight), stringFormat);
        margin += barcodeColumnPrintWidth;

        // Draw horizontal grid line
        graphics.DrawLine(pen, margin, startY, margin, startY + rowHeight);
        graphics.DrawString(DgItems.Columns[nameColumnName].HeaderText, boldFont, Brushes.Black, new Rectangle(margin, startY, DgItems.Columns[nameColumnName].Width, rowHeight), stringFormat);
        margin += nameColumnPrintWidth;

        // Draw horizontal grid lines
        graphics.DrawLine(pen, margin, startY, margin, startY + rowHeight);
        graphics.DrawString(DgItems.Columns[priceColumnName].HeaderText, boldFont, Brushes.Black, new Rectangle(margin, startY, DgItems.Columns[priceColumnName].Width, rowHeight), stringFormat);
        margin += priceColumnPrintWidth;

        // Draw horizontal grid lines
        graphics.DrawLine(pen, margin, startY, margin, startY + rowHeight);
        graphics.DrawString(DgItems.Columns[quantityColumnName].HeaderText, boldFont, Brushes.Black, new Rectangle(margin, startY, DgItems.Columns[quantityColumnName].Width, rowHeight), stringFormat);
        margin += quantityColumnPrintWidth;

        // Draw horizontal grid lines
        graphics.DrawLine(pen, margin, startY, margin, startY + rowHeight);
        graphics.DrawString(DgItems.Columns[totalColumnName].HeaderText, boldFont, Brushes.Black, new Rectangle(margin, startY, DgItems.Columns[totalColumnName].Width, rowHeight), stringFormat);
        margin += totalColumnPrintWidth;

        // Draw horizontal grid lines
        graphics.DrawLine(pen, margin, startY, margin, startY + rowHeight);

        // Draw top line
        graphics.DrawLine(pen, initialMargin, startY, margin, startY);

        // Draw bottom line
        graphics.DrawLine(pen, initialMargin, startY + rowHeight, margin, startY + rowHeight);
    }
    private void DrawRowData(Graphics graphics, Font font, StringFormat stringFormat, int margin, int startY, DataGridViewRow row)
    {
        int rowHeight = 25;
        // null check for each cell value
        string barcode = row.Cells[barcodeColumnName].Value?.ToString() ?? "";
        string name = row.Cells[nameColumnName].Value?.ToString() ?? "";
        string price = row.Cells[priceColumnName].Value?.ToString() ?? "";
        string quantity = row.Cells[quantityColumnName].Value?.ToString() ?? "";
        string total = row.Cells[totalColumnName].Value?.ToString() ?? "";

        // get cell font
        Font cellFont = row.Cells[barcodeColumnName].Style.Font ?? DgItems.Font;

        graphics.DrawString(barcode, cellFont, Brushes.Black, new RectangleF(margin, startY, DgItems.Columns[barcodeColumnName].Width, rowHeight), stringFormat);
        margin += barcodeColumnPrintWidth;  
        graphics.DrawString(name, cellFont, Brushes.Black, new RectangleF(margin, startY, DgItems.Columns[nameColumnName].Width, rowHeight), stringFormat);
        margin += nameColumnPrintWidth;
        graphics.DrawString(price, cellFont, Brushes.Black, new RectangleF(margin, startY, DgItems.Columns[priceColumnName].Width, rowHeight), stringFormat);
        margin += priceColumnPrintWidth;
        graphics.DrawString(quantity, cellFont, Brushes.Black, new RectangleF(margin, startY, DgItems.Columns[quantityColumnName].Width, rowHeight), stringFormat);
        margin += quantityColumnPrintWidth;
        graphics.DrawString(total, cellFont, Brushes.Black, new RectangleF(margin, startY, DgItems.Columns[totalColumnName].Width, rowHeight), stringFormat);

    }
    private void DrawFooter(Graphics graphics, Font font, StringFormat stringFormat, int margin, int footerHeight, int paperWidth, int paperHeight, int pageNumber)
    {
        string footerText = $"Page {pageNumber}";

        int textSize = (int)graphics.MeasureString(footerText, font).Width;

        // Draw footer text left-aligned
        graphics.DrawString(footerText, font, Brushes.Black, new Rectangle(paperWidth-margin- textSize, paperHeight - footerHeight, paperWidth - margin * 2, footerHeight), stringFormat);
    }
    //private void DrawHeader(Graphics graphics, int x, int y)
    //{
    //    // Example client info
    //    string clientInfo = "Client: ABC Corp\nDate: " + DateTime.Now.ToShortDateString();
    //    Font headerFont = new Font("Arial", 12, FontStyle.Bold);
    //    graphics.DrawString(clientInfo, headerFont, Brushes.Black, x, y);
    //}

    //private int DrawItems(Graphics graphics, int x, int y, int contentHeight)
    //{
    //    // Drawing DataGridView items (handle each row and column)
    //    Font itemFont = new Font("Arial", 10);
    //    int currentY = y;

    //    // Loop through the remaining rows to print
    //    while (rowIndex < DgItems.Rows.Count)
    //    {
    //        DataGridViewRow row = DgItems.Rows[rowIndex];

    //        // Check if the current row can fit in the available space
    //        if (currentY + 20 > contentHeight)
    //        {
    //            // Not enough space, stop printing on this page and return
    //            return currentY;
    //        }

    //        // Print the row
    //        int currentX = x;
    //        foreach (DataGridViewCell cell in row.Cells)
    //        {
    //            // Render each cell value
    //            graphics.DrawString(cell.Value?.ToString() ?? "", itemFont, Brushes.Black, currentX, currentY);
    //            currentX += 100; // Adjust x position based on column width
    //        }

    //        // Move down for the next row
    //        currentY += 20;
    //        rowIndex++; // Move to the next row
    //    }

    //    return currentY; // Return the current Y position after printing
    //}
    private int DrawColumnHeaders(Graphics graphics, int x, int y)
    {
        Font headerFont = DgItems.ColumnHeadersDefaultCellStyle.Font ?? DgItems.Font;
        Color backColor = DgItems.ColumnHeadersDefaultCellStyle.BackColor;
        Color foreColor = DgItems.ColumnHeadersDefaultCellStyle.ForeColor;
        Pen gridPen = new Pen(DgItems.GridColor);

        // Draw the column headers background
        int currentX = x;
        int headerHeight = DgItems.ColumnHeadersHeight;

        // Loop through the columns and print each header cell
        foreach (DataGridViewColumn column in DgItems.Columns)
        {
            Rectangle cellBounds = new Rectangle(currentX, y, column.Width, headerHeight);

            // Draw header background
            using (Brush backBrush = new SolidBrush(backColor))
            {
                graphics.FillRectangle(backBrush, cellBounds);
            }

            // Draw header text
            string headerText = column.HeaderText;
            TextRenderer.DrawText(graphics, headerText, headerFont, cellBounds, foreColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            // Draw grid lines
            graphics.DrawRectangle(gridPen, cellBounds);

            currentX += column.Width; // Move to the next column
        }

        return y + headerHeight; // Return the new y position (after headers)
    }
    private int DrawItems(Graphics graphics, int x, int y, int contentHeight)
    {
        int currentY = y;
        int rowHeight = 0;

        // Loop through the remaining rows to print
        while (rowIndex < DgItems.Rows.Count)
        {
            DataGridViewRow row = DgItems.Rows[rowIndex];

            // Get the height of the current row (based on DataGridView settings)
            rowHeight = row.Height;

            // Check if the current row can fit in the available space
            if (currentY + rowHeight > contentHeight)
            {
                // Not enough space, stop printing on this page and return
                return currentY;
            }

            // Print each cell in the row
            int currentX = x;
            foreach (DataGridViewCell cell in row.Cells)
            {
                // Get the style for the current cell
                DataGridViewCellStyle cellStyle = cell.Style;
                Font cellFont = cellStyle.Font ?? DgItems.Font; // Use cell font or default DataGridView font
                Color backColor = cellStyle.BackColor != Color.Empty ? cellStyle.BackColor : DgItems.DefaultCellStyle.BackColor;
                Color foreColor = cellStyle.ForeColor != Color.Empty ? cellStyle.ForeColor : DgItems.DefaultCellStyle.ForeColor;

                // Draw the cell background
                Rectangle cellBounds = new Rectangle(currentX, currentY, cell.Size.Width, rowHeight);
                using (Brush backBrush = new SolidBrush(backColor))
                {
                    graphics.FillRectangle(backBrush, cellBounds);
                }

                // Draw the cell content (text)
                string cellText = cell.Value?.ToString() ?? string.Empty;
                TextRenderer.DrawText(graphics, cellText, cellFont, cellBounds, foreColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

                // Draw cell border/grid
                Pen gridPen = new Pen(DgItems.GridColor);
                graphics.DrawRectangle(gridPen, cellBounds);

                currentX += cell.Size.Width; // Move to the next column
            }

            // Move down for the next row
            currentY += rowHeight;
            rowIndex++; // Move to the next row
        }

        return currentY; // Return the current Y position after printing
    }
    private void DrawFooter(Graphics graphics, int x, int y, int currentPage, int totalPages)
    {
        string footerText = $"Page {currentPage} of {totalPages}";
        Font footerFont = new Font("Arial", 10);
        graphics.DrawString(footerText, footerFont, Brushes.Black, x, y);
    }

    private void DrawOrderSummary(Graphics graphics, int x, int y)
    {
        string summary = "Total: $XXXX.XX\nThank you for your business!";
        Font summaryFont = new Font("Arial", 12, FontStyle.Bold);
        graphics.DrawString(summary, summaryFont, Brushes.Black, x, y);
    }

    private void btnPrint_Click(object sender, EventArgs e)
    {
        // Show Print Preview Dialog
        PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();
        printPreviewDialog.Document = printDocument;

        // Optional: Set a larger preview dialog size
        printPreviewDialog.ClientSize = new Size(800, 600);
        // Show the preview dialog
        printPreviewDialog.ShowDialog();

        //// Show print dialog
        //PrintDialog printDialog = new PrintDialog();
        //printDialog.Document = printDocument;
        //if (printDialog.ShowDialog() == DialogResult.OK)
        //{
        //    printDocument.Print();
        //}
    }
}
