using FoodOrdering.MAUI.Services;
using FoodOrdering.MAUI.ViewModels;

namespace FoodOrdering.MAUI.Pages;

public partial class CartPage : ContentPage
    {
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