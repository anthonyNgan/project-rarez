using RarezItemWebScraper.ViewModels;

namespace RarezItemWebScraper.Views
{
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
		private void OnHomeClicked(object sender, EventArgs e) { /* Navigate Home */ }
		private void OnProductsClicked(object sender, EventArgs e) { /* Navigate or filter to products */ }
		private void OnSearchClicked(object sender, EventArgs e) { /* Open advanced search */ }
		private void OnAccountClicked(object sender, EventArgs e) { /* Show account/login page */ }
		private void OnFilterSearchPressed(object sender, EventArgs e) { /* Optional: trigger filter */ }
	}
}
