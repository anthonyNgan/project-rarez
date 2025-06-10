public interface IProductScraper
{
    string SourceName { get; }
    Task<List<ProductModel>> GetProductsAsync(string url);
    Task<List<string>> GetProductDetailImagesAsync(string productUrl);
    Task<string> GetProductStockStatusAsync(string productUrl);
}