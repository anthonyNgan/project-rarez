public class PopMartScraper : IProductScraper
{
    public string SourceName => "PopMart";

    public async Task<List<ProductModel>> GetProductsAsync(string url)
    {
        var products = new List<ProductModel>();
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

                var imgNode = prod.SelectSingleNode(".//img[contains(@class, 'rimage__image')]");

                string imageUrl = "";

                if (imgNode != null)
                {
                    // Try the data-srcset attribute first (preferred, more reliable than src)
                    imageUrl = imgNode.GetAttributeValue("data-srcset", null)
                        ?? imgNode.GetAttributeValue("srcset", null)
                        ?? imgNode.GetAttributeValue("src", null)
                        ?? imgNode.GetAttributeValue("data-src", null)
                        ?? "";

                    // If the srcset/data-srcset contains multiple sizes, pick the first URL only
                    if (!string.IsNullOrWhiteSpace(imageUrl) && imageUrl.Contains(","))
                    {
                        // Take the first URL (strip any '180w' etc after space)
                        imageUrl = imageUrl.Split(',')[0].Trim().Split(' ')[0];
                    }

                    // Make sure it's an absolute URL (starts with https:)
                    if (!string.IsNullOrWhiteSpace(imageUrl))
                    {
                        if (imageUrl.StartsWith("//"))
                            imageUrl = "https:" + imageUrl;
                        else if (imageUrl.StartsWith("/"))
                            imageUrl = "https://www.popmart.nz" + imageUrl;
                    }
                }
                products.Add(new ProductModel(name, price, fullUrl, imageUrl));
            }
        }
        return products;
    }

    public static async Task<List<string>> GetProductDetailImagesAsync(string detailUrl)
    {
        var imageUrls = new List<string>();
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36");
        var html = await httpClient.GetStringAsync(detailUrl);

        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(html);

        // Find all main detail images
        var imgNodes = doc.DocumentNode.SelectNodes("//img[contains(@class, 'rimage__image')]");
        if (imgNodes != null)
        {
            foreach (var imgNode in imgNodes)
            {
                var srcSet = imgNode.GetAttributeValue("data-srcset", null)
                        ?? imgNode.GetAttributeValue("srcset", null)
                        ?? imgNode.GetAttributeValue("src", null)
                        ?? imgNode.GetAttributeValue("data-src", null);

                if (!string.IsNullOrWhiteSpace(srcSet))
                {
                    // Pick a decent resolution (e.g., 540x, 720x, or highest available in srcset)
                    var urls = srcSet.Split(',');
                    // Pick the highest available (last in the list)
                    var lastUrl = urls.LastOrDefault()?.Trim().Split(' ')[0] ?? "";
                    string imageUrl = lastUrl;

                    if (imageUrl.StartsWith("//"))
                        imageUrl = "https:" + imageUrl;
                    else if (imageUrl.StartsWith("/"))
                        imageUrl = "https://www.popmart.nz" + imageUrl;

                    if (!string.IsNullOrWhiteSpace(imageUrl) && !imageUrls.Contains(imageUrl))
                        imageUrls.Add(imageUrl);
                }
            }
        }
        return imageUrls;
    }
}