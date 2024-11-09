using FoodOrdering.Shared.Enums;
using FoodOrdering.Shared.Models;
using FoodOrdering.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrdering.Web.Api
    {
    public static class OrderEndpoints
        {
        public static RouteGroupBuilder MapOrderApi(this RouteGroupBuilder group)
            {
            // Get all orders
            group.MapGet("/orders", async ([FromServices] IFoodOrderingService service) =>
            {
                var orders = await service.GetOrdersAsync();
                return Results.Ok(orders);
            });

            // Get order by id
            group.MapGet("/orders/{id}", async (int id, [FromServices] IFoodOrderingService service) =>
            {
                var order = await service.GetOrderAsync(id);
                if (order == null)
                    return Results.NotFound();

                return Results.Ok(order);
            });

            // Create new order
            group.MapPost("/orders", async ([FromBody] Order order, [FromServices] IFoodOrderingService service) =>
            {
                try
                    {
                    Console.WriteLine("Receiving order creation request...");
                    var newOrder = await service.CreateOrderAsync(order);

                    if (newOrder != null)
                        {
                        Console.WriteLine($"Order created successfully with ID: {newOrder.Id}");
                        // Clean up the response object to prevent serialization issues
                        foreach (var item in newOrder.Items)
                            {
                            item.Order = null; // Remove circular reference
                            if (item.MenuItem != null)
                                {
                                item.MenuItemId = item.MenuItem.Id;
                                item.MenuItem = null; // Remove complex navigation property
                                }
                            }

                        return Results.Ok(newOrder); // Change to Ok instead of Created
                        }

                    Console.WriteLine("Order creation failed - returned null");
                    return Results.BadRequest("Failed to create order");
                    }
                catch (Exception ex)
                    {
                    Console.WriteLine($"Error in order creation endpoint: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    return Results.BadRequest(new { error = ex.Message });
                    }
            });

            // Update order status
            group.MapPut("/orders/{id}/status", async (int id, [FromBody] OrderStatus newStatus, [FromServices] IFoodOrderingService service) =>
            {
                var success = await service.UpdateOrderStatusAsync(id, newStatus);
                if (!success)
                    return Results.NotFound();

                return Results.NoContent();
            });

            return group;
            }
        }
    }