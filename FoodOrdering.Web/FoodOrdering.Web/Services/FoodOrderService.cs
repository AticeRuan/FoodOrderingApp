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
            Console.WriteLine("=== Creating Order - Debug Info ===");
            try
                {
                // Step 1: Validate and prepare menu items
                foreach (var item in order.Items.ToList())
                    {
                    Console.WriteLine($"Processing item: MenuItemId = {item.MenuItemId}");
                    var menuItem = await context.MenuItems.FindAsync(item.MenuItemId);
                    if (menuItem == null)
                        {
                        throw new InvalidOperationException($"Menu item not found: {item.MenuItemId}");
                        }

                    // Create new order item without circular references
                    var newItem = new OrderItem
                        {
                        MenuItemId = item.MenuItemId,
                        MenuItem = menuItem,
                        Quantity = item.Quantity,
                        UnitPrice = menuItem.Price,
                        
                        Extras = new List<Extra>(), // Initialize empty extras
                        Spice = item.Spice ?? new List<string>()
                        };

                    // Replace the original item with the new one
                    item.MenuItem = null; // Remove circular reference
                    item.UnitPrice = newItem.UnitPrice;                  
                    }

                // Step 2: Initialize empty collections and properties
                order.OrderDate = DateTime.UtcNow;
                order.Status = OrderStatus.Pending;
                order.CustomerName ??= new Name();
                order.CustomerAddress ??= new Address { Suburb = "Taupo" };
                order.CustomerPhone ??= string.Empty;

                // Step 3: Set derived properties
                order.SetCustomerDetails(order.CustomerName, order.CustomerAddress);
                order.UpdateTotalAmount();

                // Step 4: Create order in database
                await context.Orders.AddAsync(order);

                Console.WriteLine("Attempting to save order to database");
                await context.SaveChangesAsync();
                Console.WriteLine($"Order saved successfully with ID: {order.Id}");

                return order;
                }
            catch (DbUpdateException dbEx)
                {
                Console.WriteLine("Database update error:");
                Console.WriteLine(dbEx.Message);
                Console.WriteLine(dbEx.InnerException?.Message);
                throw;
                }
            catch (Exception ex)
                {
                Console.WriteLine("General error:");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
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