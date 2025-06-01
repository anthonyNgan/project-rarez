using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

public class ProductModel : ObservableObject, INotifyPropertyChanged
{
    public string Name { get; }
    public string Price { get; }
    public string Url { get; }
    public string ImageUrl { get; }
    public ObservableCollection<string> DetailImageUrls { get; set; } = new();
    private string _stockStatus;
    public string StockStatus
    {
        get => _stockStatus;
        set
        {
            if (_stockStatus != value)
            {
                _stockStatus = value;
                OnPropertyChanged(nameof(StockStatus));
                OnPropertyChanged(nameof(StockStatusColor));
            }
        }
    }

    public Color StockStatusColor =>
        StockStatus == "In Stock" ? Colors.LimeGreen :
        StockStatus == "Out of Stock" ? Colors.Red :
        Colors.Gray;

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
