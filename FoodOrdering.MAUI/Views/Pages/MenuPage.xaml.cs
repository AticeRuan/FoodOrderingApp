using FoodOrdering.MAUI.Services;
using FoodOrdering.MAUI.ViewModels;

namespace FoodOrdering.MAUI.Pages;

public partial class MenuPage : ContentPage
    {
    private readonly MenuPageViewModel _viewModel;

    public MenuPage(MenuNavigationService navigationService)
        {
        InitializeComponent();
        _viewModel = new MenuPageViewModel(navigationService);
        BindingContext = _viewModel;

        // Connect ScrollView to ViewModel
        MenuScrollView.Scrolled += (s, e) => _viewModel.OnContentScrolled(MenuScrollView.ScrollY);

        // Connect visual elements to ViewModel for position calculations
        _viewModel.SetScrollView(MenuScrollView);
        _viewModel.SetMenuItemsCollection(MenuItems);
        _viewModel.SetMenuNav(MenuNav);
        }

    protected override void OnAppearing()
        {
        base.OnAppearing();
        _viewModel.OnPageAppearing();
        }
    }