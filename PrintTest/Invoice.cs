using Scriban;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class InvoiceModel
{
    public string ClientName { get; set; }
    public string InvoiceDate { get; set; }
    public List<Item> Items { get; set; }
    public decimal TotalPrice { get; set; }
    public InvoiceModel(string clientName, string invoiceDate, List<Item> items, decimal totalPrice)
    {
        ClientName = clientName;
        InvoiceDate = invoiceDate;
        Items = items;
        TotalPrice = totalPrice;
    }
}

public class Item
{
    public string Name { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public Item(string name, int quantity, decimal price)
    {
        Name = name;
        Quantity = quantity;
        Price = price;
    }
}

public class InvoiceGenerator
{
    public List<string> GeneratePaginatedInvoicesHtml(InvoiceModel model, ref List<string> errors)
    {
        string templateFile = "FicherosINI/invoice_template.html";
        const int itemsPerPage = 50;
        var paginatedHtmlList = new List<string>();
        var templateContent = File.ReadAllText(templateFile);

        var template = Template.Parse(templateContent);

        // Check for any errors
        if (template.HasErrors)
        {
            foreach (var error in template.Messages)
            {
                Console.WriteLine(error);
            }
        }

        try
        {
            var validItems = model.Items
                .Where(item => item != null && item.Name != null)
                .ToList();

            int totalItems = validItems.Count;

            for (int i = 0; i < totalItems; i += itemsPerPage)
            {
                var currentPageItems = validItems.GetRange(i, Math.Min(itemsPerPage, totalItems - i));
                var pageData = new
                {
                    client_name = model.ClientName ?? "",
                    invoice_date = model.InvoiceDate ?? "",
                    items = currentPageItems.Select(item => new
                    {
                        name = item.Name,
                        quantity = item.Quantity,
                        price = item.Price
                    }).ToList(),
                    total_price = model.TotalPrice,
                    page_number = (i / itemsPerPage) + 1,
                    is_last_page = (i + itemsPerPage) >= totalItems
                };
                var result = template.Render(pageData);
                paginatedHtmlList.Add(result);
            }
        }
        catch (Exception e)
        {
            MessageBox.Show($"An error occurred: {e.Message}");
        }

        return paginatedHtmlList;
    }
}