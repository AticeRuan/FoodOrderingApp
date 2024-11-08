using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodOrdering.MAUI.Services;
using FoodOrdering.Shared.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace FoodOrdering.MAUI.ViewModels
    {
    public partial class CartPageViewModel : ObservableObject, IDisposable
        {
        private readonly OrderService _orderService;
        private readonly IApiService _apiService;

        [ObservableProperty]
        private ObservableCollection<OrderItem> orderItems;

        [ObservableProperty]
        private int totalItems;

        [ObservableProperty]
        private decimal totalPrice;

        [ObservableProperty]
        private bool isOrdering;

        public CartPageViewModel()
            {
            // Get services from IoC container
            _orderService = Application.Current?.Handler?.MauiContext?.Services.GetService<OrderService>()
                          ?? throw new InvalidOperationException("OrderService not found");
            _apiService = Application.Current?.Handler?.MauiContext?.Services.GetService<IApiService>()
                         ?? throw new InvalidOperationException("ApiService not found");

            orderItems = new ObservableCollection<OrderItem>();

            // Subscribe to order changes
            if (_orderService is INotifyPropertyChanged notifyPropertyChanged)
                {
                notifyPropertyChanged.PropertyChanged += OrderService_PropertyChanged;
                }

            UpdateCartDetails();
            }

        private void OrderService_PropertyChanged(object? sender, PropertyChangedEventArgs e)
            {
            if (e.PropertyName == nameof(OrderService.CurrentOrder))
                {
                UpdateCartDetails();
                }
            }

        private void UpdateCartDetails()
            {
            var currentOrder = _orderService.CurrentOrder;

            // Update observable collection
            OrderItems.Clear();
            foreach (var item in currentOrder.Items)
                {
                OrderItems.Add(item);
                }

            // Update totals
            TotalItems = currentOrder.Items.Sum(item => item.Quantity);
            TotalPrice = currentOrder.TotalAmount;
            }

        [RelayCommand]
        private async Task PlaceOrderAsync()
            {
            if (IsOrdering) return;

            try
                {
                IsOrdering = true;

                if (_orderService.CurrentOrder.Items.Count == 0)
                    {
                    if (Application.Current?.MainPage != null)
                        {
                        await Application.Current.MainPage.DisplayAlert(
                            "Empty Cart",
                            "Please add items to your cart before placing an order.",
                            "OK");
                        }
                    return;
                    }

                // Create the order
                var newOrder = await _apiService.CreateOrderAsync(_orderService.CurrentOrder);

                if (newOrder != null)
                    {
                    if (Application.Current?.MainPage != null)
                        {
                        await Application.Current.MainPage.DisplayAlert(
                            "Success",
                            "Your order has been placed successfully!",
                            "OK");
                        }

                    // Reset the current order
                    _orderService.ResetOrder();

                    // Navigate back to main page
                    await Shell.Current.GoToAsync("//MainPage");
                    }
                }
            catch (Exception ex)
                {
                if (Application.Current?.MainPage != null)
                    {
                    await Application.Current.MainPage.DisplayAlert(
                        "Error",
                        "Failed to place order. Please try again.",
                        "OK");
                    }

                Console.WriteLine($"Error placing order: {ex.Message}");
                }
            finally
                {
                IsOrdering = false;
                }
            }

        public void Dispose()
            {
            if (_orderService is INotifyPropertyChanged notifyPropertyChanged)
                {
                notifyPropertyChanged.PropertyChanged -= OrderService_PropertyChanged;
                }
            }
        }
    }