using System.Collections.ObjectModel;
using System.ComponentModel;

public class ProductModel : INotifyPropertyChanged
{
    public string Name { get; }
    public string Price { get; }
    public string Url { get; }
    public string ImageUrl { get; }
    public ObservableCollection<string> DetailImageUrls { get; } = new();

    public ProductModel(string name, string price, string url, string imageUrl)
    {
        Name = name;
        Price = price;
        Url = url;
        ImageUrl = imageUrl;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
