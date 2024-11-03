using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodOrdering.MAUI.Pages;
using FoodOrdering.MAUI.Services;
using System.ComponentModel;

namespace FoodOrdering.MAUI.Views.Components;

public partial class CartSummaryComponent : ContentView
    {
    private readonly OrderService _orderService;

    public CartSummaryComponent()
        {
        InitializeComponent();
        _orderService = Application.Current?.Handler?.MauiContext?.Services.GetService<OrderService>()
                       ?? throw new InvalidOperationException("OrderService not found");
        BindingContext = this;

        // Subscribe to order changes
        if (_orderService is INotifyPropertyChanged notifyPropertyChanged)
            {
            notifyPropertyChanged.PropertyChanged += OrderService_PropertyChanged;
            }

        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += OnCheckoutTapped;
        this.GestureRecognizers.Add(tapGesture);

        UpdateCartSummary();
        }

    private int _totalItems;
    public int TotalItems
        {
        get => _totalItems;
        private set
            {
            if (_totalItems != value)
                {
                _totalItems = value;
                OnPropertyChanged(nameof(TotalItems));
                UpdateHasItems();
                }
            }
        }

    private decimal _totalPrice;
    public decimal TotalPrice
        {
        get => _totalPrice;
        private set
            {
            if (_totalPrice != value)
                {
                _totalPrice = value;
                OnPropertyChanged(nameof(TotalPrice));
                }
            }
        }

    private bool _hasItems;
    public bool HasItems
        {
        get => _hasItems;
        private set
            {
            if (_hasItems != value)
                {
                _hasItems = value;
                OnPropertyChanged(nameof(HasItems));
                }
            }
        }

    private void OrderService_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
        if (e.PropertyName == nameof(OrderService.CurrentOrder))
            {
            UpdateCartSummary();
            }
        }

    private void UpdateCartSummary()
        {
        var order = _orderService.CurrentOrder;
        TotalItems = order.Items.Sum(item => item.Quantity);
        TotalPrice = order.Items.Sum(item => item.TotalPrice);
        }

    private void UpdateHasItems()
        {
        HasItems = TotalItems > 0;
        }

    private async void OnCheckoutTapped(object? sender, EventArgs e)
        {
        await Shell.Current.GoToAsync(nameof(CartPage));
        }

    protected override void OnHandlerChanged()
        {
        base.OnHandlerChanged();
        if (Handler != null)
            {
            UpdateCartSummary();
            }
        }
    }