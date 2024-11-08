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
                    // Use the service to create the order
                    var newOrder = await service.CreateOrderAsync(order);
                    if (newOrder == null)
                        return Results.BadRequest("Failed to create order");

                    return Results.Created($"/api/orders/{newOrder.Id}", newOrder);
                    }
                catch (Exception ex)
                    {
                    Console.WriteLine($"Error creating order: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    return Results.BadRequest(ex.Message);
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