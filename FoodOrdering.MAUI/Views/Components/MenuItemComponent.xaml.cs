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
        Console.WriteLine("MenuItemComponent initialized");

        try
            {
            _orderService = Application.Current?.Handler?.MauiContext?.Services.GetService<OrderService>()
                           ?? throw new InvalidOperationException("OrderService not found");
            Console.WriteLine("OrderService successfully injected");
            }
        catch (Exception ex)
            {
            Console.WriteLine($"Failed to get OrderService: {ex.Message}");
            }

        BindingContext = this;
        }

    public static readonly BindableProperty MenuItemProperty = BindableProperty.Create(
      nameof(MenuItem),
      typeof(FoodMenuItem),
      typeof(MenuItemComponent),
      null,
      propertyChanged: OnMenuItemChanged);

    public FoodMenuItem? MenuItem
        {
        get => (FoodMenuItem?)GetValue(MenuItemProperty);
        set => SetValue(MenuItemProperty, value);
        }

    private static void OnMenuItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
        if (bindable is MenuItemComponent component)
            {
            component.UpdateBindings();
            }
        }

    private void UpdateBindings()
        {
        OnPropertyChanged(nameof(Name));
        OnPropertyChanged(nameof(Description));
        OnPropertyChanged(nameof(Price));
        }

    // Properties to bind in XAML
    public string Name => MenuItem?.Name ?? string.Empty;
    public string Description => MenuItem?.Description ?? string.Empty;
    public decimal Price => MenuItem?.Price ?? 0;

    [RelayCommand]
    private void AddToOrder()
        {
        Console.WriteLine("AddToOrder method called");

        if (MenuItem == null)
            {
            Console.WriteLine("MenuItem is null, cannot add to order");
            return;
            }

        try
            {
            var orderItem = new OrderItem
                {
                MenuItem = MenuItem,
                MenuItemId = MenuItem.Id,
                Quantity = 1,
                UnitPrice = MenuItem.Price
                };

            _orderService.AddOrderItem(orderItem);
            Console.WriteLine($"Successfully added item to cart: {MenuItem.Name}, Price: ${MenuItem.Price}");
            Console.WriteLine($"Current cart total items: {_orderService.CurrentOrder.Items.Count}");
            }
        catch (Exception ex)
            {
            Console.WriteLine($"Error adding item to cart: {ex.Message}");
            }
        }
    private void OnAddButtonClicked(object sender, EventArgs e)
        {
        Console.WriteLine("Add button clicked");

        if (MenuItem == null)
            {
            Console.WriteLine("MenuItem is null, cannot add to order");
            return;
            }

        try
            {
            var orderItem = new OrderItem
                {
                MenuItem = MenuItem,
                MenuItemId = MenuItem.Id,
                Quantity = 1,
                UnitPrice = MenuItem.Price
                };

            _orderService.AddOrderItem(orderItem);
            Console.WriteLine($"Successfully added item to cart: {MenuItem.Name}, Price: ${MenuItem.Price}");
            }
        catch (Exception ex)
            {
            Console.WriteLine($"Error adding item to cart: {ex.Message}");
            }
        }

    }