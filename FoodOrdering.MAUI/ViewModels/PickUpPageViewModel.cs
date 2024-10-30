using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using FoodOrdering.MAUI.Models;
using FoodOrdering.MAUI.Services;
using FoodOrdering.MAUI.Pages;

namespace FoodOrdering.MAUI.ViewModels
    {
    public partial class PickupPageViewModel : ObservableObject
        {
        private readonly OrderService _orderService;

        public PickupPageViewModel() : this(new OrderService())
            {
            }

        public PickupPageViewModel(OrderService orderService)
            {
            _orderService = orderService;
            DateViewModel = new DateSlotViewModel();
            TimeViewModel = new TimeSlotViewModel();
            firstName = string.Empty; 
            lastName = string.Empty;
            selectedDateSlot = new DateSlot(); 
            selectedTimeSlot = new TimeSlot { Id = 0, StartTime = DateTime.Now }; 
            TimeViewModel.LoadTimeSlots(DateViewModel.SelectedDateSlot.Date);
            }


        public DateSlotViewModel DateViewModel { get; }
        public TimeSlotViewModel TimeViewModel { get; }

        [ObservableProperty]
        private string firstName;

        [ObservableProperty]
        private string lastName;
   
        [ObservableProperty]
        private DateSlot selectedDateSlot;

        [ObservableProperty]
        private TimeSlot selectedTimeSlot;

        [RelayCommand]
        private async Task StartOrderAsync()
            {
            // Input validation
            if (string.IsNullOrWhiteSpace(FirstName))
                {
                if (Application.Current?.MainPage != null)
                    {
                    await Application.Current.MainPage.DisplayAlert("Input Error", "First name is required.", "OK");
                    }
                return;
                }

            if (string.IsNullOrWhiteSpace(LastName))
                {
                if (Application.Current?.MainPage != null)
                    {
                    await Application.Current.MainPage.DisplayAlert("Input Error", "Last name is required.", "OK");
                    }
                return;
                }

            if (SelectedDateSlot == null)
                {
                if (Application.Current?.MainPage != null)
                    {
                    await Application.Current.MainPage.DisplayAlert("Input Error", "Please select a pickup date.", "OK");
                    }
                return;
                }

            if (SelectedTimeSlot == null)
                {
                if (Application.Current?.MainPage != null)
                    {
                    await Application.Current.MainPage.DisplayAlert("Input Error", "Please select a pickup time.", "OK");
                    }
                return;
                }

            // Combine first name, last name, and pickup date and time for display in the alert
            string message = $"First Name: {FirstName}\nLast Name: {LastName}\n" +
                             $"Pickup Date: {SelectedDateSlot.Date:dddd, MMMM d, yyyy}\n" +
                             $"Pickup Time: {SelectedTimeSlot.StartTime:hh:mm tt}";

            // Display the alert with "Cancel" and "Continue" options
            if (Application.Current?.MainPage != null)
                {
                bool continueOrder = await Application.Current.MainPage.DisplayAlert(
                    "Confirm Pickup Details",
                    message,
                    "Continue",
                    "Cancel"
                );

                // If the user selected "Continue," proceed with the order
                if (continueOrder)
                    {
                    // Update the global order with user inputs
                    _orderService.SetName(FirstName, LastName);
                    _orderService.SetPickupOrDelivery(
                        isDelivery: false,
                        scheduledDateTime: SelectedDateSlot.Date + SelectedTimeSlot.StartTime.TimeOfDay);

                    // Navigate to the MenuPage
                    await Shell.Current.GoToAsync(nameof(MenuPage));
                    }
                }
            // If "Cancel" is selected, do nothing and stay on the current page
            }

        [RelayCommand]
        private async Task SwitchToDeliveryAsync()
            {
            await Shell.Current.GoToAsync(nameof(DeliveryPage));
            }

        partial void OnSelectedDateSlotChanged(DateSlot value)
            {
            if (value != null)
                {
                TimeViewModel.LoadTimeSlots(value.Date);
                }
            }
        }
    }
