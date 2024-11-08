using FoodOrdering.Shared.Enums;
using FoodOrdering.Shared.Models;
using FoodOrdering.Web.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodOrdering.Web.Services
{
    public class FoodOrderingService(ApplicationDbContext context) : IFoodOrderingService
    {
        // Menu Item Methods
        public async Task<List<FoodMenuItem>> GetMenuItemsAsync()
        {
            return await context.MenuItems.ToListAsync();
        }

        public async Task<FoodMenuItem?> GetMenuItemAsync(int id)
        {
            return await context.MenuItems.FindAsync(id);
        }

        public async Task<FoodMenuItem?> CreateMenuItemAsync(FoodMenuItem item)
        {
            context.MenuItems.Add(item);
            var result = await context.SaveChangesAsync();

            return result > 0 ? item : null;
        }


        public async Task<bool> UpdateMenuItemAsync(int id, FoodMenuItem item)
        {
            var existingItem = await context.MenuItems.FindAsync(id);
            if (existingItem == null) return false;

            existingItem.Name = item.Name;
            existingItem.Description = item.Description;
            existingItem.Price = item.Price;
            existingItem.Category = item.Category;
            existingItem.IsAvailable = item.IsAvailable;

            try
            {
                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteMenuItemAsync(int id)
        {
            var item = await context.MenuItems.FindAsync(id);
            if (item == null) return false;

            context.MenuItems.Remove(item);
            await context.SaveChangesAsync();
            return true;
        }

        // Order Methods
        public async Task<List<Order>> GetOrdersAsync()
        {
            return await context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.MenuItem)                  
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderAsync(int id)
        {
            return await context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.MenuItem)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order?> CreateOrderAsync(Order order)
            {
            try
                {
                // Set required fields
                order.OrderDate = DateTime.UtcNow;
                order.Status = OrderStatus.Pending;

                // Ensure the customer details are properly set
                order.SetCustomerDetails(order.CustomerName, order.CustomerAddress);

                // Calculate total amount
                order.UpdateTotalAmount();

                // Add the order to the context
                context.Orders.Add(order);

                // For each order item
                foreach (var item in order.Items)
                    {
                    // Set the order relationship
                    item.Order = order;

                    // If we have the menu item, verify and set the price
                    if (item.MenuItem != null)
                        {
                        var menuItem = await context.MenuItems.FindAsync(item.MenuItemId);
                        if (menuItem != null)
                            {
                            item.UnitPrice = menuItem.Price;
                        
                            }
                        }
                    }

                await context.SaveChangesAsync();

                // Log successful order creation
                Console.WriteLine($"Order created successfully. Order ID: {order.Id}");
                return order;
                }
            catch (Exception ex)
                {
                Console.WriteLine($"Error in CreateOrderAsync: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw; // Rethrow to let the endpoint handle it
                }
            }

        public async Task<bool> UpdateOrderStatusAsync(int id, OrderStatus newStatus)
        {
            try
            {
                var order = await context.Orders.FindAsync(id);
                if (order == null) return false;

                // Validate status transition
                if (!IsValidStatusTransition(order.Status, newStatus))
                    return false;

                order.Status = newStatus;

                //// If order is completed or cancelled, set completion date
                //if (newStatus == OrderStatus.Completed || newStatus == OrderStatus.Cancelled)
                //{
                //    order.CompletedDate = DateTime.UtcNow;
                //}

                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CancelOrderAsync(int id)
        {
            try
            {
                var order = await context.Orders.FindAsync(id);
                if (order == null) return false;

                // Only pending or confirmed orders can be cancelled
                if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Confirmed)
                    return false;

                order.Status = OrderStatus.Cancelled;
                //order.CompletedDate = DateTime.UtcNow;

                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Helper method to validate status transitions
        private static bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            return (currentStatus, newStatus) switch
            {
                // Valid transitions
                (OrderStatus.Pending, OrderStatus.Confirmed) => true,
                (OrderStatus.Confirmed, OrderStatus.Ready) => true,
                (OrderStatus.Ready, OrderStatus.Completed) => true,
                (OrderStatus.Pending, OrderStatus.Cancelled) => true,
                (OrderStatus.Confirmed, OrderStatus.Cancelled) => true,
                // Invalid transitions
                _ => false
            };
        }
    }
}