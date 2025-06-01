public class PopMartScraper : IProductScraper
{
    public string SourceName => "PopMart";

    public async Task<List<ProductModel>> GetProductsAsync()
    {
        var products = new List<ProductModel>();
        var url = "https://www.popmart.nz/collections/products?sort_by=created-ascending";
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36");
        var html = await httpClient.GetStringAsync(url);

        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(html);

        var productNodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'product-info')]");
        if (productNodes != null)
        {
            foreach (var prod in productNodes)
            {
                var linkNode = prod.SelectSingleNode(".//a[contains(@class, 'product-link')]");
                string href = linkNode?.GetAttributeValue("href", "") ?? "";
                string fullUrl = string.IsNullOrEmpty(href) ? "" :
                    (href.StartsWith("http") ? href : $"https://www.popmart.nz{href}");

                var nameNode = prod.SelectSingleNode(".//div[contains(@class, 'product-block__title')]");
                string name = nameNode?.InnerText.Trim() ?? "(no name)";

                var priceNode = prod.SelectSingleNode(".//span[contains(@class, 'product-price__amount')]");
                string price = priceNode?.InnerText.Trim() ?? "(no price)";

                var imgNode = linkNode?.SelectSingleNode(".//img");
                string imageUrl = imgNode?.GetAttributeValue("src", "") ?? "";

                products.Add(new ProductModel
                {
                    Name = name,
                    Price = price,
                    Url = fullUrl,
                    ImageUrl = imageUrl
                });
            }
        }
        return products;
    }
}