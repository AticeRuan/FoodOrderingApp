using System.Windows.Input;
using Microsoft.Maui.Devices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodOrdering.MAUI.Services;

namespace FoodOrdering.MAUI.ViewModels
    {
    public partial class MainPageViewModel : ObservableObject
        {
        public ICommand NavigateToDeliveryCommand { get; }
        public ICommand NavigateToPickUpCommand { get; }

        private readonly OrderService _orderService;

        public MainPageViewModel() : this(new OrderService())
            {
            }

        public MainPageViewModel(OrderService orderService)
            {
            _orderService = orderService;
            SetHeaderHeight();
            SetCircleWidth();
            NavigateToDeliveryCommand = new AsyncRelayCommand(NavigateToDeliveryPage);
            NavigateToPickUpCommand = new AsyncRelayCommand(NavigateToPickUpPage);
            }

        [ObservableProperty]
        private double headerHeight;

        [ObservableProperty]
        private double circleWidth;

        private async Task NavigateToDeliveryPage()
            {
     
          
            await Shell.Current.GoToAsync(nameof(Pages.DeliveryPage));
            Console.WriteLine("delivery button tapped");
            }

        private async Task NavigateToPickUpPage()
            {
      
            
            await Shell.Current.GoToAsync(nameof(Pages.PickUpPage));
            Console.WriteLine("pick up button tapped");
            }

        private void SetHeaderHeight()
            {
            double screenHeight = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;
#if WINDOWS
                HeaderHeight = 500;
#else
            HeaderHeight = screenHeight * 0.5;
#endif
            }

        private void SetCircleWidth()
            {
            double screenWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
#if WINDOWS
                CircleWidth = 400;
#else
            CircleWidth = screenWidth * 0.8;
#endif
            }
        }
    }
