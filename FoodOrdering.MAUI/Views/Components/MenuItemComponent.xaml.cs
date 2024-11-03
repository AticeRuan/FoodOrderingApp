using CommunityToolkit.Mvvm.Input;
using FoodOrdering.MAUI.Services;
using FoodOrdering.Shared.Models;

namespace FoodOrdering.MAUI.Views.Components;

public partial class MenuItemComponent : ContentView
    {
    private readonly OrderService _orderService;

    public MenuItemComponent()
        {
        InitializeComponent();
        _orderService = Application.Current?.Handler?.MauiContext?.Services.GetService<OrderService>()
                       ?? throw new InvalidOperationException("OrderService not found");
        BindingContext = this;
        }

    public static readonly BindableProperty MenuItemProperty = BindableProperty.Create(
        nameof(MenuItem),
        typeof(FoodMenuItem),
        typeof(MenuItemComponent),
        null);

    public FoodMenuItem? MenuItem
        {
        get => (FoodMenuItem?)GetValue(MenuItemProperty);
        set => SetValue(MenuItemProperty, value);
        }

    [RelayCommand]
    private void AddToOrder()
        {
        if (MenuItem == null) return;

        var orderItem = new OrderItem
            {
            MenuItem = MenuItem,
            MenuItemId = MenuItem.Id,
            Quantity = 1,
            UnitPrice = MenuItem.Price
            };

        _orderService.AddOrderItem(orderItem);
        }
    }