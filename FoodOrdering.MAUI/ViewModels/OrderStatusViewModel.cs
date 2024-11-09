using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodOrdering.MAUI.Services;
using FoodOrdering.Shared.Enums;
using FoodOrdering.Shared.Models;
using System.Timers;
using System.Diagnostics;
using System.Text.Json;
namespace FoodOrdering.MAUI.ViewModels
    {
    [QueryProperty(nameof(OrderId), "orderId")]
    public partial class OrderStatusViewModel : ObservableObject, IDisposable
        {
        private readonly IApiService _apiService;
        private readonly OrderService _orderService;
        private System.Timers.Timer _refreshTimer;

        [ObservableProperty]
        private int orderId;

        [ObservableProperty]
        private OrderStatus currentStatus;

        [ObservableProperty]
        private bool showCancelButton;

        [ObservableProperty]
        private bool showNewOrderButton;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private bool hasError;

        // Status colors
        [ObservableProperty] private Color pendingColor;
        [ObservableProperty] private Color confirmedColor;
        [ObservableProperty] private Color readyColor;
        [ObservableProperty] private Color completedColor;

        // Line colors
        [ObservableProperty] private Color pendingLineColor;
        [ObservableProperty] private Color confirmedLineColor;
        [ObservableProperty] private Color readyLineColor;

        // Completion indicators
        [ObservableProperty] private bool isPendingCompleted;
        [ObservableProperty] private bool isConfirmedCompleted;
        [ObservableProperty] private bool isReadyCompleted;
        [ObservableProperty] private bool isCompletedCompleted;

        public OrderStatusViewModel()
            {
            _apiService = Application.Current?.Handler?.MauiContext?.Services.GetService<IApiService>()
                ?? throw new InvalidOperationException("ApiService not found");
            _orderService = Application.Current?.Handler?.MauiContext?.Services.GetService<OrderService>()
                ?? throw new InvalidOperationException("OrderService not found");

       

     
            _refreshTimer = new System.Timers.Timer(10000); // 10 seconds
            _refreshTimer.Elapsed += RefreshTimer_Elapsed;
            _refreshTimer.AutoReset = true;
            _refreshTimer.Start();

      
            _ = RefreshOrderStatus();
            }

        private async Task RefreshOrderStatus()
            {
            try
                {
                Debug.WriteLine($"Refreshing status for order {OrderId}");
                var order = await _apiService.GetOrderAsync(OrderId);

                if (order != null)
                    {
                    CurrentStatus = order.Status;
                    UpdateStatusColors();
                    UpdateButtonVisibility();

              
                    if (CurrentStatus == OrderStatus.Completed)
                        {
                        _refreshTimer?.Stop();
                        Debug.WriteLine("Order completed, stopping refresh timer");
                        }
                    HasError = false;
                    ErrorMessage = string.Empty;
                    }
                else
                    {
                    HandleError("Unable to fetch order status");
                    }
                }
            catch (HttpRequestException ex)
                {
                Debug.WriteLine($"HTTP error refreshing order status: {ex.Message}");
                HandleError("Network error occurred while fetching order status");
                }
            catch (JsonException ex)
                {
                Debug.WriteLine($"JSON error refreshing order status: {ex.Message}");
                HandleError("Error processing server response");
                }
            catch (Exception ex)
                {
                Debug.WriteLine($"Error refreshing order status: {ex.Message}");
                HandleError("An unexpected error occurred");
                }
            }

        private void UpdateButtonVisibility()
            {
            ShowCancelButton = CurrentStatus != OrderStatus.Completed;
            ShowNewOrderButton = CurrentStatus == OrderStatus.Completed;
            }
        private void HandleError(string message)
            {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ErrorMessage = message;
                HasError = true;
            });
            }
        [RelayCommand]
        private async Task NewOrder()
            {
            try
                {
          
                _orderService.ResetOrder();

    
                await Shell.Current.GoToAsync("//MainPage");
                }
            catch (Exception ex)
                {
                Debug.WriteLine($"Error starting new order: {ex.Message}");
                await Shell.Current.DisplayAlert(
                    "Error",
                    "Unable to start a new order. Please try again.",
                    "OK");
                }
            }
        private void StartRefreshTimer()
            {
            _refreshTimer?.Stop();
            _refreshTimer?.Dispose();

            _refreshTimer = new System.Timers.Timer(1000); // 10 seconds
            _refreshTimer.Elapsed += RefreshTimer_Elapsed;
            _refreshTimer.AutoReset = true;
            if (CurrentStatus != OrderStatus.Completed)
                {
                _refreshTimer.Start();
                }
            }

        private async void RefreshTimer_Elapsed(object? sender, ElapsedEventArgs e)
            {
            if (CurrentStatus != OrderStatus.Completed)
                {
                await RefreshOrderStatus();
                }
            }

        partial void OnOrderIdChanged(int value)
            {
            Debug.WriteLine($"OrderId changed to: {value}");
            if (value > 0)
                {
                StartRefreshTimer();
                RefreshOrderStatus().ConfigureAwait(false);
                }
            }

        private void UpdateStatusColors()
            {
            var completedColor = Color.FromArgb("#F8D581"); // Gold color
            var pendingColor = Color.FromArgb("#9B9B9B"); // Darker gray

            // Update status colors based on current status
            PendingColor = CurrentStatus >= OrderStatus.Pending ? completedColor : pendingColor;
            ConfirmedColor = CurrentStatus >= OrderStatus.Confirmed ? completedColor : pendingColor;
            ReadyColor = CurrentStatus >= OrderStatus.Ready ? completedColor : pendingColor;
            CompletedColor = CurrentStatus >= OrderStatus.Completed ? completedColor : pendingColor;

            // Update connecting line colors - showing progress to next state
            PendingLineColor = CurrentStatus > OrderStatus.Pending ? completedColor : pendingColor;
            ConfirmedLineColor = CurrentStatus > OrderStatus.Confirmed ? completedColor : pendingColor;
            ReadyLineColor = CurrentStatus > OrderStatus.Ready ? completedColor : pendingColor;

            // Update completion indicators
            IsPendingCompleted = CurrentStatus >= OrderStatus.Pending;
            IsConfirmedCompleted = CurrentStatus >= OrderStatus.Confirmed;
            IsReadyCompleted = CurrentStatus >= OrderStatus.Ready;
            IsCompletedCompleted = CurrentStatus >= OrderStatus.Completed;
            }

        [RelayCommand]
        private async Task CancelOrder()
            {
            if (CurrentStatus != OrderStatus.Pending)
                {
                await Shell.Current.DisplayAlert(
                    "Cannot Cancel Order",
                    "Sorry, this order cannot be cancelled as it is already being processed.",
                    "OK");
                return;
                }

            bool confirm = await Shell.Current.DisplayAlert(
                "Cancel Order",
                "Are you sure you want to cancel this order?",
                "Yes", "No");

            if (confirm)
                {
                try
                    {
                    await _apiService.UpdateOrderStatusAsync(OrderId, OrderStatus.Cancelled);
                    await Shell.Current.GoToAsync("//MainPage");
                    }
                catch (Exception ex)
                    {
                    await Shell.Current.DisplayAlert(
                        "Error",
                        "Failed to cancel the order. Please try again.",
                        "OK");
                    }
                }
            }

        public void Dispose()
            {
            _refreshTimer?.Stop();
            _refreshTimer?.Dispose();
            }
        }
    }