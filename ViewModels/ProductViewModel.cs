
public ProductViewModel(string name, string price, string url, string imageUrl)
{
    Name = name;
    Price = price;
    Url = url;
    ImageUrl = imageUrl;
    OpenLinkCommand = new Command(() =>
    {
        if (!string.IsNullOrWhiteSpace(url))
            Launcher.OpenAsync(new Uri(url));
    });
}