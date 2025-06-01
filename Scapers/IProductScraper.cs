public interface IProductScraper
{
    Task<List<ProductModel>> GetProductsAsync(string url);

    Task<List<string>> GetProductDetailImagesAsync(string url);

    string SourceName { get; }
}