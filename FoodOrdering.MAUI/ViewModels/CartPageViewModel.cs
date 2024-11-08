using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodOrdering.MAUI.Services;
using FoodOrdering.Shared.Enums;
using FoodOrdering.Shared.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text.Json;

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
                string.IsNullOrWhiteSpace(order.CustomerName.LastName) ||
                order.CustomerName.FirstName.Length < 2 ||
                order.CustomerName.LastName.Length < 2)
                {
                validationMessage = "Please enter valid first and last names (minimum 2 characters each)";
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
                    Unit = null,
                    StreetNumber = null,
                    StreetName = null,
                    Suburb = "Taupo" // Keep only the city for pickup orders
                    };
                order.CustomerPhone = "N/A";
                }

            // Set customer details (this will handle FullName and FullAddress properly)
            order.SetCustomerDetails(order.CustomerName, order.CustomerAddress);

            // Update totals
            order.UpdateTotalAmount();

            // Update order items
            foreach (var item in order.Items)
                {
                item.Order = null; // Remove circular reference
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

                // Validate order
                if (!ValidateOrder(out string validationMessage))
                    {
                    if (Application.Current?.MainPage != null)
                        {
                        await Application.Current.MainPage.DisplayAlert(
                            "Invalid Order",
                            validationMessage,
                            "OK");
                        }
                    return;
                    }

                // Prepare order for submission
                PrepareOrderForSubmission(_orderService.CurrentOrder);

                // Log order details
                Console.WriteLine("\n=== Order Details Before Submission ===");
                var orderJson = JsonSerializer.Serialize(_orderService.CurrentOrder,
                    new JsonSerializerOptions { WriteIndented = true });
                Console.WriteLine(orderJson);

                // Submit order
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

                    _orderService.ResetOrder();
                    await Shell.Current.GoToAsync("//MainPage");
                    }
                }
            catch (HttpRequestException ex)
                {
                Console.WriteLine($"\n=== HTTP Error Details ===");
                Console.WriteLine($"Status Code: {ex.StatusCode}");
                Console.WriteLine($"Message: {ex.Message}");

                var errorMessage = "There was a problem connecting to the server. ";
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
                Console.WriteLine($"\n=== Error Details ===");
                Console.WriteLine($"Error Type: {ex.GetType().Name}");
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

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