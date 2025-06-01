using System.Collections.ObjectModel;
using System.ComponentModel;
using RarezItemWebScraper.Models;
using RarezItemWebScraper.Scrapers;

namespace RarezItemWebScraper.ViewModels;

public class MainPageViewModel : INotifyPropertyChanged
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
                OnPropertyChanged(nameof(SearchText));
                FilterProducts();
            }
        }
    }

    private readonly PopMartScraper _scraper = new();

    public async Task LoadProductsOnStartAsync()
    {
        string url = "https://www.popmart.nz/collections/products?sort_by=created-ascending";
        try
        {
            var products = await _scraper.GetProductsAsync(url);
            AllProducts.Clear();
            foreach (var product in products.Take(50))
                AllProducts.Add(product);
            FilterProducts();
        }
        catch (Exception ex)
        {
            // Handle error
        }
    }

    public void FilterProducts()
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

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}