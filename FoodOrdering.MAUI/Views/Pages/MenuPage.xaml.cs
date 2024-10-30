using FoodOrdering.MAUI.Services;
using FoodOrdering.Shared.Models;
using Microsoft.Extensions.DependencyInjection;

namespace FoodOrdering.MAUI.Pages
{
    public partial class MenuPage : ContentPage
    {
        private readonly IApiService _apiService;
        private List<FoodMenuItem> _menuItems = [];
        private bool _isBusy;

        public new bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        // Parameterless constructor for Shell
        public MenuPage()
        {
            InitializeComponent();
            _apiService = Application.Current?.Handler?.MauiContext?.Services.GetService<IApiService>()
                          ?? throw new InvalidOperationException("IApiService not found");
            BindingContext = this;
        }

        // Constructor with dependency injection
        public MenuPage(IApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadMenuItems();
        }

        private async Task LoadMenuItems()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                _menuItems = await _apiService.GetMenuItemsAsync();
                MenuItemsCollectionView.ItemsSource = _menuItems;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to load menu items.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error loading menu items: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}