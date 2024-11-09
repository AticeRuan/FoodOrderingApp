using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
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
        private readonly JsonSerializerOptions _serializerOptions;
        public ApiService()
            {
#if DEBUG
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            _httpClient = new HttpClient(handler);



          
            var baseUrl = DeviceInfo.Platform == DevicePlatform.Android
                ? "https://10.0.2.2:7235"  // Android Emulator
                : "https://localhost:7235"; // Windows/iOS/MacCatalyst

            Debug.WriteLine($"Using base URL: {baseUrl}");
            _httpClient.BaseAddress = new Uri(baseUrl);
#else
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("https://your-production-api-url.com");
#endif

    
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            _serializerOptions = new JsonSerializerOptions
                {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
                };

            }

        // Menu Items
        public async Task<List<FoodMenuItem>> GetMenuItemsAsync()
        {
            try
            {
                Debug.WriteLine("Fetching menu items...");
                var response = await _httpClient.GetAsync("/api/menu");

            
                var responseContent = await response.Content.ReadAsStringAsync();
            

                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"Error status code: {response.StatusCode}");
                    return [];
                }

               
                var items = await response.Content.ReadFromJsonAsync<List<FoodMenuItem>>();
             
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
                Debug.WriteLine($"Fetching order with id: {id}");
                var response = await _httpClient.GetAsync($"/api/orders/{id}");

               
                var responseContent = await response.Content.ReadAsStringAsync();
          

                if (!response.IsSuccessStatusCode)
                    {
                    Debug.WriteLine($"Error status code: {response.StatusCode}");
                    return null;
                    }

                if (response.Content.Headers.ContentType?.MediaType != "application/json")
                    {
                    Debug.WriteLine("Error: Response is not JSON");
                    return null;
                    }

                var order = await response.Content.ReadFromJsonAsync<Order>();
                Debug.WriteLine($"Successfully deserialized order: {order?.Id}");
                return order;
                }
            catch (JsonException ex)
                {
                Debug.WriteLine($"JSON parsing error: {ex.Message}");
                throw;
                }
            catch (Exception ex)
                {
                Debug.WriteLine($"Error getting order: {ex.Message}");
                throw;
                }
            }

        public async Task<Order> CreateOrderAsync(Order order)
            {
            try
                {
                Debug.WriteLine("Sending order to API...");
                var response = await _httpClient.PostAsJsonAsync("/api/orders", order, _serializerOptions);

                // Log response details
                var responseContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Response status: {response.StatusCode}");
                Debug.WriteLine($"Response content: {responseContent}");

                if (response.IsSuccessStatusCode)
                    {
                    try
                        {
                        var newOrder = await response.Content.ReadFromJsonAsync<Order>(_serializerOptions);
                        if (newOrder != null)
                            {
                            Debug.WriteLine($"Order created successfully with ID: {newOrder.Id}");
                            return newOrder;
                            }
                        }
                    catch (JsonException ex)
                        {
                        Debug.WriteLine($"Error deserializing response: {ex.Message}");
                    
                        return order;
                        }
                    }

                if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                         
                    return order;
                    }

 
                response.EnsureSuccessStatusCode();
                return order;
                }
            catch (HttpRequestException ex)
                {
                Debug.WriteLine($"\n=== HTTP Error Details ===");
                Debug.WriteLine($"Status Code: {ex.StatusCode}");
                Debug.WriteLine($"Message: {ex.Message}");

                if (ex.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                    Debug.WriteLine("Returning original order despite 500 error");
                    return order;
                    }

                throw;
                }
            catch (Exception ex)
                {
                Debug.WriteLine($"Error creating order: {ex.Message}");
                throw;
                }
            }

        public async Task<bool> UpdateOrderStatusAsync(int id, OrderStatus newStatus)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"/api/orders/{id}/status", newStatus);
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