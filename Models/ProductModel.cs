using System.Windows.Input;

public class ProductModel
{
    public string Name { get; }
    public string Price { get; }
    public string Url { get; }
    public string ImageUrl { get; }
    public ICommand OpenLinkCommand { get; }
}
