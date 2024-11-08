using FoodOrdering.MAUI.Services;
using FoodOrdering.Shared.Models;
using System.ComponentModel;

namespace FoodOrdering.MAUI.Views.Components;

public partial class OrderItemComponent : ContentView, INotifyPropertyChanged
    {
    private readonly OrderService _orderService;

    public OrderItemComponent()
        {
        InitializeComponent();

        _orderService = Application.Current?.Handler?.MauiContext?.Services.GetService<OrderService>()
                       ?? throw new InvalidOperationException("OrderService not found");

        }

    public static readonly BindableProperty OrderItemProperty = BindableProperty.Create(
        nameof(OrderItem),
        typeof(OrderItem),
        typeof(OrderItemComponent),
        null,
        propertyChanged: OnOrderItemChanged);

    public OrderItem? OrderItem
        {
        get => (OrderItem?)GetValue(OrderItemProperty);
        set
            {
            SetValue(OrderItemProperty, value);
            BindingContext = value; 
            }
        }

    public bool CanDecrement => OrderItem?.Quantity > 1;

    private static void OnOrderItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
        if (bindable is OrderItemComponent component)
            {
            component.OnPropertyChanged(nameof(OrderItem));
            component.OnPropertyChanged(nameof(CanDecrement));
            }
        }

    private void OnRemoveClicked(object sender, EventArgs e)
        {
        if (OrderItem == null) return;

        var currentOrder = _orderService.CurrentOrder;
        currentOrder.Items.Remove(OrderItem);
        currentOrder.UpdateTotalAmount();

        // Notify changes through the service
        _orderService.UpdateOrder();
        }

    private void OnIncrementClicked(object sender, EventArgs e)
        {
        if (OrderItem == null) return;

        OrderItem.Quantity++;
        _orderService.CurrentOrder.UpdateTotalAmount();
        OnPropertyChanged(nameof(CanDecrement));

        // Notify changes through the service
        _orderService.UpdateOrder();
        }

    private void OnDecrementClicked(object sender, EventArgs e)
        {
        if (OrderItem == null || OrderItem.Quantity==1) return;

        OrderItem.Quantity--;
        _orderService.CurrentOrder.UpdateTotalAmount();
        OnPropertyChanged(nameof(CanDecrement));

        // Notify changes through the service
        _orderService.UpdateOrder();
        }
    }