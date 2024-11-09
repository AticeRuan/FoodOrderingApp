using FoodOrdering.MAUI.Services;
using FoodOrdering.MAUI.ViewModels;

namespace FoodOrdering.MAUI.Pages;

public partial class CartPage : ContentPage
    {
    private bool _isNavigating = false;
    public CartPage()
        {
        InitializeComponent();
        BindingContext = new CartPageViewModel();
        }

    protected override void OnDisappearing()
        {
        base.OnDisappearing();
        if (BindingContext is IDisposable disposable)
            {
            disposable.Dispose();
            }
        }


    }