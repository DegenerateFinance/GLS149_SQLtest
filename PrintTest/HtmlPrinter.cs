using Scriban.Parsing;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.Design.AxImporter;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace PrintTest;

public class HtmlPrinter
{
    private string htmlContent;
    public HtmlPrinter(string htmlContent)
    {
        this.htmlContent = htmlContent;
    }
    public void PrintHtml()
    {
        PrintDocument pd = new PrintDocument();
        pd.PrintPage += PrintPage;

        PrintDialog printDialog = new PrintDialog();
        printDialog.Document = pd;

        // show print preview dialog
        PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();
        printPreviewDialog.Document = pd;

        // Optional: Set a larger preview dialog size
        printPreviewDialog.ClientSize = new Size(800, 600);
        // Show the preview dialog
        printPreviewDialog.ShowDialog();

        //if (printDialog.ShowDialog() == DialogResult.OK)
        //{
        //    pd.Print();
        //}
    }

    private void PrintPage(object sender, PrintPageEventArgs e)
    {
        using (WebBrowser browser = new WebBrowser())
        {
            browser.ScrollBarsEnabled = false;
            browser.DocumentText = htmlContent;
            browser.DocumentCompleted += (s, args) =>
            {
                // Set the WebBrowser size to the full page size
                browser.Width = (int)e.PageSettings.PrintableArea.Width;
                browser.Height = (int)e.PageSettings.PrintableArea.Height;

                if (browser.Document?.Body != null)
                {
                    // Force the browser to layout its content for the new size
                    browser.Document.Body.Style = "margin: 0; padding: 0;";
                    browser.Document.ExecCommand("SelectAll", false, null);
                    browser.Document.ExecCommand("Unselect", false, null);

                    using (Bitmap bitmap = new Bitmap(browser.Width, browser.Height))
                    {
                        browser.DrawToBitmap(bitmap, new Rectangle(0, 0, browser.Width, browser.Height));
                        e.Graphics?.DrawImage(bitmap, e.PageBounds);
                    }
                }
            };
            while (browser.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }
        }
    }
}