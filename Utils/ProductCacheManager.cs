using System.Text.Json;
using System.Text.Json.Serialization;

public static class ProductCacheManager
{
    private static readonly string CacheFile = "products_cache.json";
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static void SaveProducts(IEnumerable<ProductModel> products)
    {
        string json = JsonSerializer.Serialize(products, JsonOptions);
        File.WriteAllText(CacheFile, json);
    }

    public static List<ProductModel> LoadProducts()
    {
        if (!File.Exists(CacheFile))
            return new List<ProductModel>();
        string json = File.ReadAllText(CacheFile);
        return JsonSerializer.Deserialize<List<ProductModel>>(json, JsonOptions) ?? new List<ProductModel>();
    }

    public static void UpsertProduct(ProductModel newProduct)
    {
        var products = LoadProducts();
        var idx = products.FindIndex(p => p.Url == newProduct.Url);
        if (idx >= 0)
            products[idx] = newProduct;
        else
            products.Add(newProduct);
        SaveProducts(products);
    }
}