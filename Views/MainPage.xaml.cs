public partial class MainPage : ContentPage
{
	private MainPageViewModel ViewModel => (MainPageViewModel)BindingContext;

	public MainPage()
	{
		InitializeComponent();
		BindingContext = new MainPageViewModel();
		_ = ViewModel.LoadProductsOnStartAsync();
	}

	private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
	{
		ViewModel.SearchText = e.NewTextValue;
	}
}