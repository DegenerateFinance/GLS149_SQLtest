using DinkToPdf;
using DinkToPdf.Contracts;
using System.Drawing.Imaging;
using System.Drawing.Printing;

namespace PrintTest;
public class PdfGenerator
{
    public void GenerateMultiPagePdf(List<string> htmlPages, string outputFilePath)
    {
        var converter = new SynchronizedConverter(new PdfTools());

        var doc = new HtmlToPdfDocument()
        {
            GlobalSettings = {
                ColorMode = DinkToPdf.ColorMode.Color,
                Orientation = DinkToPdf.Orientation.Portrait,
                PaperSize = DinkToPdf.PaperKind.A4,
                Out = outputFilePath // Specify output file path here
            }
        };

        // Instead of assigning, we add each page's content to the Objects list
        foreach (var htmlContent in htmlPages)
        {
            doc.Objects.Add(new ObjectSettings()
            {
                HtmlContent = htmlContent,
                HeaderSettings = { Center = "Invoice Header" },
                FooterSettings = { Right = "[page]" } // Adds page numbering
            });
        }

        // Pass only the document to the Convert method
        converter.Convert(doc);
    }
}

