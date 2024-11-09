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
            Console.WriteLine(_orderService.CurrentOrder);
            FirstName = _orderService.CurrentOrder.CustomerName.FirstName??string.Empty;
            LastName = _orderService.CurrentOrder.CustomerName.LastName ?? string.Empty;
            PhoneNumber = _orderService.CurrentOrder.CustomerPhone??string.Empty;
            UnitNumber = _orderService.CurrentOrder.CustomerAddress.Unit??string.Empty;
            StreetNumber = _orderService.CurrentOrder.CustomerAddress.StreetNumber??string.Empty;
            StreetName = _orderService.CurrentOrder.CustomerAddress.StreetName?? string.Empty;
            Suburb = _orderService.CurrentOrder.CustomerAddress.Suburb?? string.Empty;
            SelectedDateSlot = new DateSlot { Id = 0, Date = DateTime.Today };
            TimeViewModel.LoadTimeSlots(DateViewModel.SelectedDateSlot.Date);
            SelectedTimeSlot = TimeViewModel.SelectedTimeSlot ?? new TimeSlot();


          

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
        
                    }
            };


            TimeViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(TimeViewModel.SelectedTimeSlot))
                    {
                    SelectedTimeSlot = TimeViewModel.SelectedTimeSlot??new TimeSlot();
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
        [ObservableProperty]
        private bool isLoading;


        [ObservableProperty] private DateSlot selectedDateSlot;
        [ObservableProperty] private TimeSlot selectedTimeSlot;

        [RelayCommand]
        private async Task StartOrderAsync()
            {
            if (IsLoading) return;
            try { if (string.IsNullOrWhiteSpace(FirstName) ||
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

        
            string message = $"First Name: {FirstName}\nLast Name: {LastName}\nPhone: {PhoneNumber}\n" +
                             $"Address: {UnitNumber} {StreetNumber} {StreetName}, {Suburb}\n" +
                             $"Delivery Date: {SelectedDateSlot.Date:dddd, MMMM d, yyyy}\n" +
                             $"Delivery Time: {SelectedTimeSlot.StartTime:hh:mm tt}";

      
            bool confirm = false;
            if (Application.Current?.MainPage != null)
                {
                confirm = await Application.Current.MainPage.DisplayAlert("Confirm Delivery Details", message, "Continue", "Cancel");
                }
                IsLoading = true;

                if (confirm)
                {
              
                var address = new Address
                    {
                    Unit = UnitNumber,
                    StreetNumber = StreetNumber,
                    StreetName = StreetName,
                    Suburb = Suburb
                    };
                _orderService.SetName(FirstName, LastName);
                _orderService.SetAddress(address);
                _orderService.CurrentOrder.CustomerPhone = PhoneNumber;
                _orderService.SetPickupOrDelivery(isDelivery: true, scheduledDateTime: SelectedDateSlot.Date + SelectedTimeSlot.StartTime.TimeOfDay);
                
                

            
                await Shell.Current.GoToAsync(nameof(MenuPage));
                } } finally { IsLoading = false; }

           
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

    