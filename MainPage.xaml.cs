using HtmlAgilityPack;
using System.Net.Http;

namespace RarezItemWebScraper;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
		_ = LoadProductsOnStartAsync();
	}

	private async Task LoadProductsOnStartAsync()
	{
		string url = "https://www.popmart.nz/collections/products?sort_by=created-ascending";
		ResultLabel.Text = "Scraping products...";
		try
		{
			var products = await ScrapeProductsPopmartAsync(url);

			if (products.Count > 0)
			{
				var lines = products
					.Take(10)
					.Select(p => $"{p.Name} — {p.Price}\n{p.Url}");
				ResultLabel.Text = "Products:\n\n" + string.Join("\n\n", lines);
			}
			else
			{
				ResultLabel.Text = "No products found.";
			}
		}
		catch (Exception ex)
		{
			ResultLabel.Text = $"Error: {ex.Message}";
		}
	}

	private async Task<List<(string Name, string Price, string Url)>> ScrapeProductsPopmartAsync(string url)
	{
		var products = new List<(string Name, string Price, string Url)>();
		using var httpClient = new HttpClient();
		httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
			"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36");
		var html = await httpClient.GetStringAsync(url);

		var doc = new HtmlDocument();
		doc.LoadHtml(html);

		// Find all product-info containers
		var productNodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'product-info')]");
		if (productNodes != null)
		{
			foreach (var prod in productNodes)
			{
				// Find the product link inside .product-link
				var linkNode = prod.SelectSingleNode(".//a[contains(@class, 'product-link')]");
				string href = linkNode?.GetAttributeValue("href", "") ?? "";
				string fullUrl = string.IsNullOrEmpty(href) ? "" :
					(href.StartsWith("http") ? href : $"https://www.popmart.nz{href}");

				// Find the product name inside .product-block__title
				var nameNode = prod.SelectSingleNode(".//div[contains(@class, 'product-block__title')]");
				string name = nameNode?.InnerText.Trim() ?? "(no name)";

				// Find the price inside .product-price__amount
				var priceNode = prod.SelectSingleNode(".//span[contains(@class, 'product-price__amount')]");
				string price = priceNode?.InnerText.Trim() ?? "(no price)";

				products.Add((name, price, fullUrl));
			}
		}
		return products;
	}

}
