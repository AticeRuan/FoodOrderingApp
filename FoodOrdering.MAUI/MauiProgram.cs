﻿using CommunityToolkit.Maui;
using FoodOrdering.MAUI.Pages;
using FoodOrdering.MAUI.Services;
using FoodOrdering.MAUI.ViewModels;
using FoodOrdering.MAUI.Views.Pages;
using Microsoft.Extensions.Logging;

namespace FoodOrdering.MAUI
{
    public static class MauiProgram
        {
        public static MauiApp CreateMauiApp()
            {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Poppins-Black.ttf", "PBlack");
                    fonts.AddFont("Poppins-Bold.ttf", "PBold");
                    fonts.AddFont("Poppins-ExtraBold.ttf", "PExtraBold");
                    fonts.AddFont("Poppins-ExtraLight.ttf", "PExtraLight");
                    fonts.AddFont("Poppins-Light.ttf", "PLight");
                    fonts.AddFont("Poppins-Medium.ttf", "PMedium");
                    fonts.AddFont("Poppins-Regular.ttf", "PRegular");
                    fonts.AddFont("Poppins-SemiBold.ttf", "PSemiBold");
                    fonts.AddFont("Poppins-Thin.ttf", "PThin");
                });
            // Register services
            builder.Services.AddSingleton<IApiService, ApiService>();
            builder.Services.AddSingleton<OrderService>();
            builder.Services.AddSingleton<MenuNavigationService>();



            // Register pages 
            builder.Services.AddTransient<FoodOrdering.MAUI.Views.Pages.OrderStatusPage>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<MenuPage>();
            builder.Services.AddTransient<CartPage>();
            builder.Services.AddTransient<DeliveryPage>();
            builder.Services.AddTransient<PickUpPage>();
            builder.Services.AddTransient<FoodOrdering.MAUI.Views.Pages.OrderStatusPage>();
            builder.Services.AddTransient<OrderStatusViewModel>();
            // Register ViewModels
            builder.Services.AddTransient<MainPageViewModel>();
            builder.Services.AddTransient<PickupPageViewModel>();
            builder.Services.AddTransient<DeliveryPageViewModel>();
            builder.Services.AddTransient<MenuPageViewModel>();
            builder.Services.AddTransient<CartPageViewModel>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
            }
        }
}
