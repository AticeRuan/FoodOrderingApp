using FoodOrdering.Web.Api;
using FoodOrdering.Web.Api.Data;
using FoodOrdering.Web.Components;
using FoodOrdering.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace FoodOrdering.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddInteractiveWebAssemblyComponents();

            // Configure the database context with SQLite
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register server-side HttpClient
            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri(builder.Configuration["BaseUrl"] ?? "https://localhost:7235/")
            });

            // Register both the interface and implementation
            builder.Services.AddScoped<IFoodOrderingService, FoodOrderingService>();

            // Configure CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });

            var app = builder.Build();

            // Rest of the configuration remains the same...
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAntiforgery();
            app.UseCors("AllowAllOrigins");

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode()
                .AddInteractiveWebAssemblyRenderMode();

            var apiGroup = app.MapGroup("/api");
            apiGroup.MapMenuApi();   
            apiGroup.MapOrderApi();
           



            app.Run();
        }
    }
}