
using FoodOrdering.MAUI.Pages;


namespace FoodOrdering.MAUI
{
    public partial class AppShell : Shell
        {
        public AppShell()
            {
            InitializeComponent();
            RegisterRoutes();
            }

        private void RegisterRoutes()
            {
            Routing.RegisterRoute(nameof(MenuPage), typeof(MenuPage));
            Routing.RegisterRoute(nameof(DeliveryPage), typeof(DeliveryPage));
            Routing.RegisterRoute(nameof(PickUpPage), typeof(PickUpPage));
            Routing.RegisterRoute(nameof(CartPage), typeof(CartPage));
            Routing.RegisterRoute(nameof(FoodOrdering.MAUI.Pages.OrderStatusPage), typeof(FoodOrdering.MAUI.Pages.OrderStatusPage));
            }
        }
    }
