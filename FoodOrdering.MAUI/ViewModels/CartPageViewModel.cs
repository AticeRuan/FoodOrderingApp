using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodOrdering.MAUI.Services;
using FoodOrdering.Shared.Enums;
using FoodOrdering.Shared.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text.Json;
using System.Diagnostics;

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

        public bool HasItems => OrderItems.Any();

        public CartPageViewModel()
            {
            _orderService = Application.Current?.Handler?.MauiContext?.Services.GetService<OrderService>()
                          ?? throw new InvalidOperationException("OrderService not found");
            _apiService = Application.Current?.Handler?.MauiContext?.Services.GetService<IApiService>()
                         ?? throw new InvalidOperationException("ApiService not found");

            // Initialize as an ObservableCollection directly from CurrentOrder.Items
            orderItems = new ObservableCollection<OrderItem>(_orderService.CurrentOrder.Items);

            // Subscribe to collection changes
            OrderItems.CollectionChanged += OrderItems_CollectionChanged;

            // Subscribe to order service changes
            _orderService.PropertyChanged += OrderService_PropertyChanged;

            // Initial update
            UpdateTotals();
            }

        private void OrderItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
            {
            UpdateTotals();
            OnPropertyChanged(nameof(HasItems));
            }

        [RelayCommand]
        private async Task NavigateToMenu()
            {
            await Shell.Current.GoToAsync("..");
            }
        private void OrderService_PropertyChanged(object? sender, PropertyChangedEventArgs e)
            {
            if (e.PropertyName == nameof(OrderService.CurrentOrder))
                {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // Synchronize collections
                    OrderItems.Clear();
                    foreach (var item in _orderService.CurrentOrder.Items)
                        {
                        OrderItems.Add(item);
                        }
                    UpdateTotals();
                    OnPropertyChanged(nameof(HasItems));
                });
                }
            }

        private void UpdateTotals()
            {
            TotalItems = OrderItems.Sum(item => item.Quantity);
            TotalPrice = OrderItems.Sum(item => item.TotalPrice);
            }
        private bool ValidateOrder(out string validationMessage)
            {
            validationMessage = string.Empty;
            var order = _orderService.CurrentOrder;

            // Check for items
            if (!OrderItems.Any())
                {
                validationMessage = "Cart is empty";
                return false;
                }

            // Validate name
            if (order.CustomerName == null ||
                string.IsNullOrWhiteSpace(order.CustomerName.FirstName) ||
                string.IsNullOrWhiteSpace(order.CustomerName.LastName) )
          
                {
                validationMessage = "Please enter valid first and last names";
                return false;
                }

            // Validate ScheduledDateTime
            if (order.ScheduledDateTime == default)
                {
                validationMessage = "Please select a pickup/delivery time";
                return false;
                }

            //// Validate phone number (required for all orders)
            //if (string.IsNullOrWhiteSpace(order.CustomerPhone))
            //    {
            //    validationMessage = "Phone number is required";
            //    return false;
            //    }

            //if (order.CustomerPhone.Length < 8 || order.CustomerPhone.Length > 20)
            //    {
            //    validationMessage = "Please enter a valid phone number";
            //    return false;
            //    }

            // Specific validation for delivery orders
            if (order.IsDelivery)
                {
                if (order.CustomerAddress == null ||
                    string.IsNullOrWhiteSpace(order.CustomerAddress.StreetNumber) ||
                    string.IsNullOrWhiteSpace(order.CustomerAddress.StreetName) ||
                    string.IsNullOrWhiteSpace(order.CustomerAddress.Suburb))
                    {
                    validationMessage = "Complete delivery address is required";
                    return false;
                    }
                }

            return true;
            }

        private void PrepareOrderForSubmission(Order order)
            {
            // Set basic order properties
            order.OrderDate = DateTime.UtcNow;
            order.Status = OrderStatus.Pending;

            // Clear address for pickup orders
            if (!order.IsDelivery)
                {
                order.CustomerAddress = new Address
                    {
                    Unit = "0",
                    StreetNumber = "236",
                    StreetName = "Tahapepa Road",
                    Suburb = "Taupo"
                    };
                }

            // Set customer details
            order.SetCustomerDetails(order.CustomerName, order.CustomerAddress);

            // Update totals
            order.UpdateTotalAmount();

            // Process order items
            foreach (var item in order.Items)
                {
                // Initialize or clear Extras if null
                item.Extras ??= new List<Extra>();

                // Remove any empty extras
                item.Extras = item.Extras
                    .Where(e => !string.IsNullOrEmpty(e.Name) && e.Quantity > 0)
                    .ToList();

                // Initialize Spice list if null
                item.Spice ??= new List<string>();

                // Remove circular reference
                item.Order = null;

                if (item.MenuItem != null)
                    {
                    item.MenuItemId = item.MenuItem.Id;
                    item.UnitPrice = item.MenuItem.Price;            
                   
                    }
                }
            }
        [RelayCommand]
        private async Task PlaceOrderAsync()
            {
            if (IsOrdering) return;

            try
                {
                IsOrdering = true;

                // Prepare order for submission
                PrepareOrderForSubmission(_orderService.CurrentOrder);

                // Submit order
                var newOrder = await _apiService.CreateOrderAsync(_orderService.CurrentOrder);

                _orderService.CurrentOrder.Id = newOrder.Id;

                // If we get here, either the order was successful or we got a 500 but know the order went through
                if (Application.Current?.MainPage != null)
                    {
                    await Application.Current.MainPage.DisplayAlert(
                        "Success",
                        "Your order has been placed successfully!",
                        "OK");
                    }

                _orderService.ResetOrder();
                await Shell.Current.GoToAsync($"{nameof(FoodOrdering.MAUI.Pages.OrderStatusPage)}?orderId={newOrder.Id}");
                }
            catch (HttpRequestException ex)
                {
                Debug.WriteLine($"\n=== HTTP Error Details ===");
                Debug.WriteLine($"Status Code: {ex.StatusCode}");
                Debug.WriteLine($"Message: {ex.Message}");

                var errorMessage = "There was a problem placing your order. ";
                if (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                    errorMessage = "The order information is incomplete. Please check all required fields.";
                    }

                if (Application.Current?.MainPage != null)
                    {
                    await Application.Current.MainPage.DisplayAlert(
                        "Error",
                        errorMessage,
                        "OK");
                    }
                }
            catch (Exception ex)
                {
                Debug.WriteLine($"\n=== Error Details ===");
                Debug.WriteLine($"Error Type: {ex.GetType().Name}");
                Debug.WriteLine($"Message: {ex.Message}");
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");

                if (Application.Current?.MainPage != null)
                    {
                    await Application.Current.MainPage.DisplayAlert(
                        "Error",
                        "Failed to place order. Please try again.",
                        "OK");
                    }
                }
            finally
                {
                IsOrdering = false;
                }
            }


        public void Dispose()
            {
            OrderItems.CollectionChanged -= OrderItems_CollectionChanged;
            _orderService.PropertyChanged -= OrderService_PropertyChanged;
            }
        }
    }