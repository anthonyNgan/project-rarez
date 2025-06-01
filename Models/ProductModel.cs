using System.Windows.Input;
public class ProductModel
{
    public string Name { get; }
    public string Price { get; }
    public string Url { get; }
    public string ImageUrl { get; }
    public ICommand OpenLinkCommand { get; }

    public ProductModel(string name, string price, string url, string imageUrl, ICommand? openLinkCommand = null)
    {
        Name = name;
        Price = price;
        Url = url;
        ImageUrl = imageUrl;
        OpenLinkCommand = openLinkCommand ?? new Command(async () =>
        {
            if (!string.IsNullOrWhiteSpace(url))
                await Launcher.Default.OpenAsync(url);
        });
    }
}
