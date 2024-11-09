using FoodOrdering.MAUI.ViewModels;
namespace FoodOrdering.MAUI.Pages;

public partial class OrderStatusPage : ContentPage
    {
    private OrderStatusViewModel viewModel;
    public OrderStatusPage()
        {
        InitializeComponent();
        viewModel = new OrderStatusViewModel();
        BindingContext = viewModel;
        }
    }
