public interface IProductScraper
{
    Task<List<ProductModel>> GetProductsAsync(string url);
    string SourceName { get; }
}