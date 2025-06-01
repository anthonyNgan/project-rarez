using System.Collections.ObjectModel;
using System.ComponentModel;
using HtmlAgilityPack;

namespace RarezItemWebScraper;

public partial class MainPage : ContentPage, INotifyPropertyChanged
{

	public ObservableCollection<ProductViewModel> AllProducts { get; set; } = new();
	public ObservableCollection<ProductViewModel> FilteredProducts { get; set; } = new();
	private string _searchText = string.Empty;

	public string SearchText
	{
		get => _searchText;
		set
		{
			if (_searchText != value)
			{
				_searchText = value;
				OnPropertyChanged();
				FilterProducts();
			}
		}
	}

	public ObservableCollection<ProductViewModel> Products { get; set; } = new();

	public MainPage()
	{
		InitializeComponent();
		BindingContext = this;
		_ = LoadProductsOnStartAsync();
	}

	private async Task LoadProductsOnStartAsync()
	{
		string url = "https://www.popmart.nz/collections/products?sort_by=created-ascending";
		try
		{
			var products = await ScrapeProductsPopmartAsync(url);

			AllProducts.Clear();
			foreach (var (Name, Price, Url, ImageUrl) in products.Take(50))
				AllProducts.Add(new ProductViewModel(Name, Price, Url, ImageUrl));
			FilterProducts();
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", ex.Message, "OK");
		}
	}

	private void FilterProducts()
	{
		FilteredProducts.Clear();
		var filtered = string.IsNullOrWhiteSpace(SearchText)
			? AllProducts
			: AllProducts.Where(p =>
				(p.Name ?? "").ToLower().Contains(SearchText.ToLower()) ||
				(p.Price ?? "").ToLower().Contains(SearchText.ToLower())
			  );
		foreach (var item in filtered)
			FilteredProducts.Add(item);
	}

	// This gets called by the SearchBar TextChanged event in XAML
	private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
	{
		SearchText = e.NewTextValue;
	}

	private async Task<List<(string Name, string Price, string Url, string ImageUrl)>> ScrapeProductsPopmartAsync(string url)
	{
		var products = new List<(string Name, string Price, string Url, string ImageUrl)>();
		using var httpClient = new HttpClient();
		httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
			"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36");
		var html = await httpClient.GetStringAsync(url);

		var doc = new HtmlDocument();
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

				products.Add((name, price, fullUrl, imageUrl));
			}
		}
		return products;
	}
}
