using FoodOrdering.Shared.Models;
using FoodOrdering.Web.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrdering.Web.Api
{
    public static class MenuEndpoints
    {
        public static RouteGroupBuilder MapMenuApi(this RouteGroupBuilder group)
        {
            group.MapGet("/menu", async ([FromServices] IFoodOrderingService service) =>
            {
                var items = await service.GetMenuItemsAsync();
                return Results.Ok(items);
            });

            group.MapGet("/menu/{id}", async (int id, [FromServices] IFoodOrderingService service) =>
            {
                var item = await service.GetMenuItemAsync(id);
                return item is null ? Results.NotFound() : Results.Ok(item);
            });

            group.MapPost("/menu", async ([FromBody] FoodMenuItem item, [FromServices] IFoodOrderingService service) =>
            {
                var result = await service.CreateMenuItemAsync(item);

                if (result is null)
                {
                    return Results.BadRequest("Failed to create menu item.");
                }

                return Results.Created($"/api/menu/{result.Id}", result);
            });


            group.MapPut("/menu/{id}", async (int id, [FromBody] FoodMenuItem item, [FromServices] IFoodOrderingService service) =>
            {
                var result = await service.UpdateMenuItemAsync(id, item);
                return result ? Results.NoContent() : Results.NotFound();
            });

            group.MapDelete("/menu/{id}", async (int id, [FromServices] IFoodOrderingService service) =>
            {
                var result = await service.DeleteMenuItemAsync(id);
                return result ? Results.NoContent() : Results.NotFound();
            });

            return group;
        }
    }
}