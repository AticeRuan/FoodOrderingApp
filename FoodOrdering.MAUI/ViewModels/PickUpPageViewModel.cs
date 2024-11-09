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
        private DateSlot _selectedDateSlot;
        private TimeSlot _selectedTimeSlot;
        public PickupPageViewModel() : this(new OrderService())
            {
            }

        public PickupPageViewModel(OrderService orderService)
            {
            _orderService = Application.Current?.Handler?.MauiContext?.Services.GetService<OrderService>()
                       ?? throw new InvalidOperationException("OrderService not found");
            DateViewModel = new DateSlotViewModel();
            TimeViewModel = new TimeSlotViewModel();
            SetProperty(ref firstName, _orderService.CurrentOrder.CustomerName.FirstName ?? string.Empty, nameof(FirstName));
            SetProperty(ref lastName, _orderService.CurrentOrder.CustomerName.LastName ?? string.Empty, nameof(LastName));
            SetProperty(ref phoneNumber, _orderService.CurrentOrder.CustomerPhone ?? string.Empty, nameof(PhoneNumber));

            _selectedDateSlot = new DateSlot { Id = 0, Date = DateTime.Today };
            TimeViewModel.LoadTimeSlots(SelectedDateSlot.Date);
            _selectedTimeSlot = TimeViewModel.SelectedTimeSlot ?? new TimeSlot();

            if (_orderService.CurrentOrder.ScheduledDateTime != default)
                {
                var scheduledDate = _orderService.CurrentOrder.ScheduledDateTime.Date;
                var scheduledTime = _orderService.CurrentOrder.ScheduledDateTime;

         
                var matchingDate = DateViewModel.DateSlots.FirstOrDefault(d => d.Date.Date == scheduledDate);
                if (matchingDate != null)
                    {
                    SelectedDateSlot = matchingDate;
                    }

       
                TimeViewModel.LoadTimeSlots(SelectedDateSlot.Date);
                var matchingTime = TimeViewModel.TimeSlots.FirstOrDefault(t =>
                    t.StartTime.Hour == scheduledTime.Hour &&
                    t.StartTime.Minute == scheduledTime.Minute);
                if (matchingTime != null)
                    {
                    TimeViewModel.SelectedTimeSlot = matchingTime;
                    }
                }

            DateViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(DateViewModel.SelectedDateSlot))
                    {
                    SelectedDateSlot = DateViewModel.SelectedDateSlot;
                    TimeViewModel.LoadTimeSlots(SelectedDateSlot.Date);
                    }
            };

            TimeViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(TimeViewModel.SelectedTimeSlot))
                    {
                    SelectedTimeSlot = TimeViewModel.SelectedTimeSlot ?? new TimeSlot();
                    }
            };

            }


        public DateSlotViewModel DateViewModel { get; }
        public TimeSlotViewModel TimeViewModel { get; }

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string firstName;

        [ObservableProperty]
        private string lastName;

        [ObservableProperty] private string phoneNumber;

        public DateSlot SelectedDateSlot
            {
            get => _selectedDateSlot;
            set
                {
                if (_selectedDateSlot != value)
                    {
                    _selectedDateSlot = value;
                    OnPropertyChanged(nameof(SelectedDateSlot));
                    OnSelectedDateSlotChanged(value);
                    }
                }
            }

        public TimeSlot SelectedTimeSlot
            {
            get => _selectedTimeSlot;
            set
                {
                if (_selectedTimeSlot != value)
                    {
                    _selectedTimeSlot = value;
                    OnPropertyChanged(nameof(SelectedTimeSlot));
                    }
                }
            }

        [RelayCommand]
        private async Task StartOrderAsync()
            {

            if (IsLoading) return;


            try
                {
             
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

            if (string.IsNullOrWhiteSpace(PhoneNumber))
                {
                if (Application.Current?.MainPage != null)
                    {
                    await Application.Current.MainPage.DisplayAlert("Input Error", "Phone number is required.", "OK");
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

          
            string message = $"First Name: {FirstName}\nLast Name: {LastName}\nPhone: {PhoneNumber}\n" +
                             $"Pickup Date: {SelectedDateSlot.Date:dddd, MMMM d, yyyy}\n" +
                             $"Pickup Time: {SelectedTimeSlot.StartTime:hh:mm tt}";

            if (Application.Current?.MainPage != null)
                {
                bool continueOrder = await Application.Current.MainPage.DisplayAlert(
                    "Confirm Pickup Details",
                    message,
                    "Continue",
                    "Cancel"
                );
                    IsLoading = true;

                    if (continueOrder)
                    {
                  
                    _orderService.SetName(FirstName, LastName);
                    _orderService.CurrentOrder.CustomerPhone = PhoneNumber;
                    _orderService.SetPickupOrDelivery(
                        isDelivery: false,
                        scheduledDateTime: SelectedDateSlot.Date + SelectedTimeSlot.StartTime.TimeOfDay);

                    
                    await Shell.Current.GoToAsync(nameof(MenuPage));
                       
                        }
                }
                }
            finally
                {
                IsLoading = false;
                }



            }

        [RelayCommand]
        private async Task SwitchToDeliveryAsync()
            {
            await Shell.Current.GoToAsync(nameof(DeliveryPage));
            }

        private void OnSelectedDateSlotChanged(DateSlot value)
            {
          
            if (value != null)
                {
               
                TimeViewModel.LoadTimeSlots(value.Date);
                }
            }
        }
    }
