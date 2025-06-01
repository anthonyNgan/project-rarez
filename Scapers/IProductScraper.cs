public interface IProductScraper
{
    Task<List<ProductModel>> GetProductsAsync();
    string SourceName { get; } // e.g., "PopMart" or "Pokemon"
}