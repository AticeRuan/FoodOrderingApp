using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodOrdering.MAUI.Pages;
using FoodOrdering.MAUI.Services;
using System.ComponentModel;

namespace FoodOrdering.MAUI.Views.Components;

public partial class CartSummaryComponent : ContentView, IDisposable
    {
    private readonly OrderService _orderService;

    public CartSummaryComponent()
        {
        InitializeComponent();

        try
            {
            _orderService = Application.Current?.Handler?.MauiContext?.Services.GetService<OrderService>()
                           ?? throw new InvalidOperationException("OrderService not found");
            Console.WriteLine("OrderService successfully injected in CartSummaryComponent");

            // Subscribe to PropertyChanged event
            _orderService.PropertyChanged += OrderService_PropertyChanged;

            // Initialize values
            UpdateCartSummary();
            }
        catch (Exception ex)
            {
            Console.WriteLine($"Error initializing CartSummaryComponent: {ex.Message}");
            }

        BindingContext = this;
        }

    protected override void OnHandlerChanged()
        {
        base.OnHandlerChanged();
        if (Handler != null)
            {
            UpdateCartSummary();
            }
        }

    private void OrderService_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
        if (e.PropertyName == nameof(OrderService.CurrentOrder))
            {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                try
                    {
                    UpdateCartSummary();
                    Console.WriteLine($"Cart updated: Items={TotalItems}, Price=${TotalPrice}");
                    }
                catch (Exception ex)
                    {
                    Console.WriteLine($"Error updating cart summary: {ex.Message}");
                    }
            });
            }
        }


    private void UpdateCartSummary()
        {
        if (_orderService == null) return;

        var order = _orderService.CurrentOrder;
        TotalItems = order.Items.Sum(item => item.Quantity);
        TotalPrice = order.Items.Sum(item => item.TotalPrice);
        HasItems = TotalItems > 0;

        Console.WriteLine($"UpdateCartSummary called: Items={TotalItems}, Price=${TotalPrice}, Visible={HasItems}");
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
                Console.WriteLine($"HasItems changed to: {value}");
                }
            }
        }

    private void UpdateHasItems()
        {
        HasItems = TotalItems > 0;
        }

    private async void OnCheckoutTapped(object? sender, EventArgs e)
        {
        await Shell.Current.GoToAsync(nameof(CartPage));
        }

    public void Dispose()
        {
        if (_orderService is INotifyPropertyChanged notifyPropertyChanged)
            {
            notifyPropertyChanged.PropertyChanged -= OrderService_PropertyChanged;
            Console.WriteLine("Unsubscribed from OrderService changes");
            }
        }
    }