using HtmlAgilityPack;
using System.Net.Http;

namespace RarezItemWebScraper;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	private async void OnScrapeButtonClicked(object sender, EventArgs e)
	{
		string url = UrlEntry.Text?.Trim();
		if (string.IsNullOrWhiteSpace(url))
		{
			ResultLabel.Text = "Please enter a URL.";
			return;
		}

		ResultLabel.Text = "Scraping...";
		try
		{
			string title = await ScrapeTitleAsync(url);
			ResultLabel.Text = $"Page Title: {title}";
		}
		catch (Exception ex)
		{
			ResultLabel.Text = $"Error: {ex.Message}";
		}
	}

	private async Task<string> ScrapeTitleAsync(string url)
	{
		using var httpClient = new HttpClient();
		httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
			"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36");

		var html = await httpClient.GetStringAsync(url);

		var doc = new HtmlDocument();
		doc.LoadHtml(html);

		var titleNode = doc.DocumentNode.SelectSingleNode("//title");
		return titleNode?.InnerText?.Trim() ?? "[No <title> found]";
	}
}
