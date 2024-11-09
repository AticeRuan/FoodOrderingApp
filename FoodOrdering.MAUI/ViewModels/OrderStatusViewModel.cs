using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodOrdering.MAUI.Services;
using FoodOrdering.Shared.Enums;
using FoodOrdering.Shared.Models;
using System.Timers;
using System.Diagnostics;

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
        private bool canCancelOrder;

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

        public OrderStatusViewModel(IApiService apiService, OrderService orderService)
            {
            _apiService = apiService;
            _orderService = orderService;

            // Initialize timer
            _refreshTimer = new System.Timers.Timer(10000); // 10 seconds
            _refreshTimer.Elapsed += RefreshTimer_Elapsed;
            _refreshTimer.AutoReset = true;
            _refreshTimer.Start();

            // Initial fetch
            RefreshOrderStatus();
            }

        private async void RefreshTimer_Elapsed(object? sender, ElapsedEventArgs e)
            {
            await RefreshOrderStatus();
            }

        private async Task RefreshOrderStatus()
            {
            try
                {
                var order = await _apiService.GetOrderAsync(OrderId);
                if (order != null)
                    {
                    CurrentStatus = order.Status;
                    UpdateStatusColors();
                    CanCancelOrder = CurrentStatus == OrderStatus.Pending;
                    }
                }
            catch (Exception ex)
                {
                // Handle error
                Debug.WriteLine($"Error refreshing order status: {ex.Message}");
                }
            }

        private void UpdateStatusColors()
            {
            var completedColor = Colors.Gold;
            var pendingColor = Colors.Grey;

            // Update status colors
            PendingColor = CurrentStatus >= OrderStatus.Pending ? completedColor : pendingColor;
            ConfirmedColor = CurrentStatus >= OrderStatus.Confirmed ? completedColor : pendingColor;
            ReadyColor = CurrentStatus >= OrderStatus.Ready ? completedColor : pendingColor;
            CompletedColor = CurrentStatus >= OrderStatus.Completed ? completedColor : pendingColor;

            // Update line colors
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