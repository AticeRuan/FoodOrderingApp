using FoodOrdering.Shared.Enums;
using FoodOrdering.Shared.Models;

namespace FoodOrdering.Web.Services
    {
    public interface IFoodOrderingService
        {
        // Menu Items
        Task<List<FoodMenuItem>> GetMenuItemsAsync();
        Task<FoodMenuItem?> GetMenuItemAsync(int id);
        Task<FoodMenuItem?> CreateMenuItemAsync(FoodMenuItem item);
        Task<bool> UpdateMenuItemAsync(int id, FoodMenuItem item);
        Task<bool> DeleteMenuItemAsync(int id);

        // Orders
        Task<List<Order>> GetOrdersAsync();
        Task<Order?> GetOrderAsync(int id);
        Task<Order?> CreateOrderAsync(Order order);  // Add this method
        Task<bool> UpdateOrderStatusAsync(int id, OrderStatus newStatus);
        }
    }