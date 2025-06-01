public interface IProductScraper
{
    Task<List<ProductModel>> GetProductsAsync();
    string SourceName { get; }
}