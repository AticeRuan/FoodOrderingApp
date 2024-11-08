﻿using CommunityToolkit.Mvvm.ComponentModel;
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
            firstName = _orderService.CurrentOrder.CustomerName.FirstName ?? string.Empty;
            lastName = _orderService.CurrentOrder.CustomerName.LastName ?? string.Empty;
            PhoneNumber = _orderService.CurrentOrder.CustomerPhone ?? string.Empty;
            _selectedDateSlot = new DateSlot { Id = 0, Date = DateTime.Today };
            TimeViewModel.LoadTimeSlots(SelectedDateSlot.Date);

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

            _selectedTimeSlot = TimeViewModel?.SelectedTimeSlot ?? new TimeSlot();
            }


        public DateSlotViewModel DateViewModel { get; }
        public TimeSlotViewModel TimeViewModel { get; }

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
