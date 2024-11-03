using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodOrdering.Shared.Models;
using FoodOrdering.MAUI.Models;
using FoodOrdering.MAUI.Services;
using System;
using System.Threading.Tasks;
using FoodOrdering.MAUI.Pages;


namespace FoodOrdering.MAUI.ViewModels
    {
    public partial class DeliveryPageViewModel : ObservableObject
        {
        private readonly OrderService _orderService;

        public DeliveryPageViewModel() : this(new OrderService())
            {
            }

        public DeliveryPageViewModel(OrderService orderService)
            {
            _orderService = Application.Current?.Handler?.MauiContext?.Services.GetService<OrderService>()
                       ?? throw new InvalidOperationException("OrderService not found");
            DateViewModel = new DateSlotViewModel();
            TimeViewModel = new TimeSlotViewModel();
            FirstName = string.Empty;
            LastName = string.Empty;
            PhoneNumber = string.Empty;
            UnitNumber = string.Empty;
            StreetNumber = string.Empty;
            StreetName = string.Empty;
            Suburb = string.Empty;
            SelectedDateSlot = new DateSlot();
            SelectedTimeSlot = new TimeSlot();
            TimeViewModel.LoadTimeSlots(DateViewModel.SelectedDateSlot.Date);

            // Subscribe to DateViewModel changes
            DateViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(DateViewModel.SelectedDateSlot))
                    {
                    SelectedDateSlot = DateViewModel.SelectedDateSlot;
        
                    }
            };

            // Subscribe to TimeViewModel changes
            TimeViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(TimeViewModel.SelectedTimeSlot))
                    {
                    SelectedTimeSlot = TimeViewModel.SelectedTimeSlot;
                    }
            };
            }

        public DateSlotViewModel DateViewModel { get; }
        public TimeSlotViewModel TimeViewModel { get; }

        [ObservableProperty] private string firstName;
        [ObservableProperty] private string lastName;
        [ObservableProperty] private string phoneNumber;
        [ObservableProperty] private string unitNumber;
        [ObservableProperty] private string streetNumber;
        [ObservableProperty] private string streetName;
        [ObservableProperty] private string suburb;

        [ObservableProperty] private DateSlot selectedDateSlot;
        [ObservableProperty] private TimeSlot selectedTimeSlot;

        [RelayCommand]
        private async Task StartOrderAsync()
            {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(FirstName) ||
                string.IsNullOrWhiteSpace(LastName) ||
                string.IsNullOrWhiteSpace(PhoneNumber) ||       
                string.IsNullOrWhiteSpace(StreetNumber) ||
                string.IsNullOrWhiteSpace(StreetName) ||
                string.IsNullOrWhiteSpace(Suburb) ||
                SelectedDateSlot == null ||
                SelectedTimeSlot == null)
                {
                if (Application.Current?.MainPage != null)
                    {
                    await Application.Current.MainPage.DisplayAlert("Input Error", "Please fill in all fields.", "OK");
                    }
                return;
                }

            // Create confirmation message
            string message = $"First Name: {FirstName}\nLast Name: {LastName}\nPhone: {PhoneNumber}\n" +
                             $"Address: {UnitNumber} {StreetNumber} {StreetName}, {Suburb}\n" +
                             $"Delivery Date: {SelectedDateSlot.Date:dddd, MMMM d, yyyy}\n" +
                             $"Delivery Time: {SelectedTimeSlot.StartTime:hh:mm tt}";

            // Confirm order details
            bool confirm = false;
            if (Application.Current?.MainPage != null)
                {
                confirm = await Application.Current.MainPage.DisplayAlert("Confirm Delivery Details", message, "Continue", "Cancel");
                }

            if (confirm)
                {
                // Set details in OrderService
                var address = new Address
                    {
                    Unit = UnitNumber,
                    StreetNumber = StreetNumber,
                    StreetName = StreetName,
                    Suburb = Suburb
                    };
                _orderService.SetName(FirstName, LastName);
                _orderService.SetAddress(address);
                _orderService.SetPickupOrDelivery(isDelivery: true, scheduledDateTime: SelectedDateSlot.Date + SelectedTimeSlot.StartTime.TimeOfDay);
                
                

                // Navigate to MenuPage
                await Shell.Current.GoToAsync(nameof(MenuPage));
                }
            }

        [RelayCommand]
        private async Task SwitchToPickupAsync()
            {
            await Shell.Current.GoToAsync(nameof(PickUpPage));
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

    