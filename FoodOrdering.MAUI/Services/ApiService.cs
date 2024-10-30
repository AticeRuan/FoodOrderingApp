using System.Diagnostics;
using System.Net.Http.Json;
using FoodOrdering.Shared.Enums;
using FoodOrdering.Shared.Models;

namespace FoodOrdering.MAUI.Services
{
    public interface IApiService
    {
        // Menu Items
        Task<List<FoodMenuItem>> GetMenuItemsAsync();
        Task<FoodMenuItem?> GetMenuItemAsync(int id);

        // Orders
        Task<List<Order>> GetOrdersAsync();
        Task<Order?> GetOrderAsync(int id);
        Task<Order> CreateOrderAsync(Order order);
        Task<bool> UpdateOrderStatusAsync(int id, OrderStatus newStatus);
    }

    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService()
        {
            _httpClient = new HttpClient();

#if DEBUG
            // Use different URLs based on platform when debugging
            var baseUrl = DeviceInfo.Platform == DevicePlatform.Android
                ? "https://10.0.2.2:7235"  // Android Emulator
                : "https://localhost:7235"; // Windows/iOS/MacCatalyst

            Debug.WriteLine($"Using base URL: {baseUrl}");
            _httpClient.BaseAddress = new Uri(baseUrl);

            // Add accept header
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
#endif
        }

        // Menu Items
        public async Task<List<FoodMenuItem>> GetMenuItemsAsync()
        {
            try
            {
                Debug.WriteLine("Fetching menu items...");
                var response = await _httpClient.GetAsync("/api/menu");

                // Log the response
                var responseContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Response status: {response.StatusCode}");
                Debug.WriteLine($"Response content: {responseContent}");

                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"Error status code: {response.StatusCode}");
                    return [];
                }

                // Try to deserialize the response
                var items = await response.Content.ReadFromJsonAsync<List<FoodMenuItem>>();
                Debug.WriteLine($"Successfully deserialized {items?.Count ?? 0} items");
                return items ?? [];
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in GetMenuItemsAsync: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<FoodMenuItem?> GetMenuItemAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/menu/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<FoodMenuItem>();
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting menu item: {ex.Message}");
                throw;
            }
        }

        // Orders
        public async Task<List<Order>> GetOrdersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/orders");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<Order>>()
                           ?? [];
                }
                return [];
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting orders: {ex.Message}");
                throw;
            }
        }

        public async Task<Order?> GetOrderAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/orders/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Order>();
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting order: {ex.Message}");
                throw;
            }
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/orders", order);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<Order>() ?? order;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating order: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(int id, OrderStatus newStatus)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"/orders/{id}/status", newStatus);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating order status: {ex.Message}");
                throw;
            }
        }
    }
}