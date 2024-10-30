using FoodOrdering.Shared.Enums;
using FoodOrdering.Shared.Models;
using FoodOrdering.Web.Api.Data;
using FoodOrdering.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace FoodOrdering.Web.Api
{
    public static class OrderEndpoints
    {
        public static RouteGroupBuilder MapOrderApi(this RouteGroupBuilder group)
        {
            // Get all orders
            group.MapGet("/orders", async (ApplicationDbContext db) =>
            {
                var orders = await db.Orders
                    .Include(o => o.Items)
                        .ThenInclude(i => i.MenuItem)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();

                return Results.Ok(orders);
            });

            // Get order by id
            group.MapGet("/orders/{id}", async (int id, ApplicationDbContext db) =>
            {
                var order = await db.Orders
                    .Include(o => o.Items)
                        .ThenInclude(i => i.MenuItem)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                    return Results.NotFound();

                return Results.Ok(order);
            });

            // Create new order
            group.MapPost("/orders", async (Order order, ApplicationDbContext db) =>
            {
                // Set order date
                order.OrderDate = DateTime.UtcNow;

                // Calculate total amount
             

                // Set initial status
                order.Status = OrderStatus.Pending;

                db.Orders.Add(order);
                await db.SaveChangesAsync();

                return Results.Created($"/api/orders/{order.Id}", order);
            });

            // Update order status
            group.MapPut("/orders/{id}/status", async (int id, OrderStatus newStatus, ApplicationDbContext db) =>
            {
                var order = await db.Orders.FindAsync(id);
                if (order == null)
                    return Results.NotFound();

                order.Status = newStatus;
                await db.SaveChangesAsync();

                return Results.Ok(order);
            });

            // Delete order (optional, you might want to just cancel instead of delete)
            group.MapDelete("/orders/{id}", async (int id, ApplicationDbContext db) =>
            {
                var order = await db.Orders.FindAsync(id);
                if (order == null)
                    return Results.NotFound();

                db.Orders.Remove(order);
                await db.SaveChangesAsync();

                return Results.Ok();
            });

            return group;
        }
    }
}