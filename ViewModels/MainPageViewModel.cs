using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Input;

namespace RarezItemWebScraper.ViewModels;

public class MainPageViewModel : INotifyPropertyChanged
{
    public ObservableCollection<ProductModel> AllProducts { get; } = new();
    public ObservableCollection<ProductModel> FilteredProducts { get; } = new();
    public AsyncRelayCommand<ProductModel?> ShowProductDetailCommand { get; }

    private readonly PopMartScraper _scraper = new();

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

    public MainPageViewModel()
    {
        _ = LoadProductsOnStartAsync();
        ShowProductDetailCommand = new AsyncRelayCommand<ProductModel?>(LoadDetailImagesAsync);
    }

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
            // Handle/log error, or expose an error property for the view
        }
    }

    public void FilterProducts()
    {
        FilteredProducts.Clear();
        var filtered = string.IsNullOrWhiteSpace(SearchText)
            ? AllProducts
            : AllProducts.Where(p =>
                (p.Name ?? "").Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                (p.Price ?? "").Contains(SearchText, StringComparison.OrdinalIgnoreCase)
              );
        foreach (var item in filtered)
            FilteredProducts.Add(item);
    }

    private async Task ShowProductDetailAsync(ProductModel product)
    {
        if (product == null) return;

        // Clear before load (to clear old images)
        product.DetailImageUrls.Clear();

        var detailImages = await _scraper.GetProductDetailImagesAsync(product.Url);
        foreach (var img in detailImages)
            product.DetailImageUrls.Add(img);

        // Now navigate or show modal -- for test, just log or break here
        // await Shell.Current.GoToAsync("ProductDetailPage", ...);
    }

    public async Task LoadDetailImagesAsync(ProductModel product)
    {
        if (product == null || string.IsNullOrWhiteSpace(product.Url))
            return;

        var detailImages = await _scraper.GetProductDetailImagesAsync(product.Url);
        product.DetailImageUrls.Clear();
        foreach (var img in detailImages)
            product.DetailImageUrls.Add(img);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
