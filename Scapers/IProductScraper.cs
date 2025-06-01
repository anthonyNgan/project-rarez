public interface IProductScraper
{
    Task<List<ProductModel>> GetProductsAsync(string url);
    Task<List<string>> GetProductDetailImagesAsync(string url);
    Task<string> GetProductStockStatusAsync(string productUrl);
    string SourceName { get; }
}