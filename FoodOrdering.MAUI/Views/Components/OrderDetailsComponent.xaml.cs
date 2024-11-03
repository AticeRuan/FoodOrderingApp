using CommunityToolkit.Mvvm.ComponentModel;
using FoodOrdering.MAUI.Services;
using FoodOrdering.MAUI.Pages;
using System.ComponentModel;

namespace FoodOrdering.MAUI.Views.Components;

public partial class OrderDetailsComponent : ContentView
    {
    private readonly OrderService _orderService;
    private const string STORE_ADDRESS = "226 Taharepa Road, Tauhara, Taupō";

    public OrderDetailsComponent()
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

        UpdateOrderDetails();
        }

    private string _orderTypeText = string.Empty;
    public string OrderTypeText
        {
        get => _orderTypeText;
        private set
            {
            if (_orderTypeText != value)
                {
                _orderTypeText = value;
                OnPropertyChanged(nameof(OrderTypeText));
                }
            }
        }

    private string _addressText = string.Empty;
    public string AddressText
        {
        get => _addressText;
        private set
            {
            if (_addressText != value)
                {
                _addressText = value;
                OnPropertyChanged(nameof(AddressText));
                }
            }
        }

    private string _scheduledTimeText = string.Empty;
    public string ScheduledTimeText
        {
        get => _scheduledTimeText;
        private set
            {
            if (_scheduledTimeText != value)
                {
                _scheduledTimeText = value;
                OnPropertyChanged(nameof(ScheduledTimeText));
                }
            }
        }

    private void OrderService_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
        if (e.PropertyName == nameof(OrderService.CurrentOrder))
            {
            UpdateOrderDetails();
            }
        }

    private void UpdateOrderDetails()
        {
        var order = _orderService.CurrentOrder;

   

        // Set order type text
        OrderTypeText = order.IsDelivery ? "Delivery • " : "Pick Up • ";

        // Set scheduled time text with bullet point
        ScheduledTimeText = $"{order.ScheduledDateTime:d MMM, h:mm tt}";

        // Set address text without trailing bullet point
        AddressText = order.IsDelivery
            ? $"{order.FullAddress}, Taupō"
            : STORE_ADDRESS;
        }

    private async void OnOrderDetailsTapped(object sender, TappedEventArgs e)
        {
        var order = _orderService.CurrentOrder;
        try
            {
            if (order.IsDelivery)
                {
                await Shell.Current.GoToAsync(nameof(DeliveryPage));
                }
            else
                {
                await Shell.Current.GoToAsync(nameof(PickUpPage));
                }
            }
        catch (Exception ex)
            {
            // Log the error if needed
            System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");

            // Show an alert to the user
            if (Application.Current?.MainPage != null)
                {
                await Application.Current.MainPage.DisplayAlert(
                    "Navigation Error",
                    "Unable to navigate to the selected page. Please try again.",
                    "OK");
                }
            }
        }

    protected override void OnHandlerChanged()
        {
        base.OnHandlerChanged();
        if (Handler != null)
            {
            UpdateOrderDetails();
            }
        }
    }