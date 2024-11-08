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
        BindingContext = this;
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
        set => SetValue(OrderItemProperty, value);
        }

    // Properties for binding
    public decimal TotalPrice => OrderItem?.TotalPrice ?? 0;
    public int Quantity => OrderItem?.Quantity ?? 0;

    private static void OnOrderItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
        if (bindable is OrderItemComponent component)
            {
            component.UpdateBindings();
            }
        }

    private void UpdateBindings()
        {
   
        OnPropertyChanged(nameof(TotalPrice));
        OnPropertyChanged(nameof(Quantity));
        }

    private void OnRemoveClicked(object sender, EventArgs e)
        {
        if (OrderItem == null) return;

        var currentOrder = _orderService.CurrentOrder;
        MainThread.BeginInvokeOnMainThread(() =>
        {
            currentOrder.Items.Remove(OrderItem);
            currentOrder.UpdateTotalAmount();
            _orderService.NotifyPropertyChanged(nameof(OrderService.CurrentOrder));
        });
        }

    private void OnIncrementClicked(object sender, EventArgs e)
        {
        if (OrderItem == null) return;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            OrderItem.Quantity++;
            _orderService.CurrentOrder.UpdateTotalAmount();
            _orderService.NotifyPropertyChanged(nameof(OrderService.CurrentOrder));
            UpdateBindings();
        });
        }

    private void OnDecrementClicked(object sender, EventArgs e)
        {
        if (OrderItem == null || OrderItem.Quantity <= 1) return;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            OrderItem.Quantity--;
            _orderService.CurrentOrder.UpdateTotalAmount();
            _orderService.NotifyPropertyChanged(nameof(OrderService.CurrentOrder));
            UpdateBindings();
        });
        }
    }