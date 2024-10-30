using FoodOrdering.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace FoodOrdering.MAUI.Services
    {
    public class OrderService
        {
        // The main Order instance to hold all details
        public Order CurrentOrder { get; private set; } = new Order();

        // Resets the order (e.g., after submission or cancellation)
        public void ResetOrder()
            {
            CurrentOrder = new Order();
            CurrentOrder.SetCustomerDetails(CurrentOrder.CustomerName,CurrentOrder.CustomerAddress);
            }

        // Additional methods to set individual properties if needed
        public void SetName(string firstName, string lastName)
            {
            CurrentOrder.CustomerName.FirstName = firstName;
            CurrentOrder.CustomerName.LastName = lastName;
            }

        public void SetAddress(Address address)
            {
            CurrentOrder.CustomerAddress = address;
            }

        public void SetPickupOrDelivery(bool isDelivery, DateTime scheduledDateTime)
            {
            CurrentOrder.IsDelivery = isDelivery;
            CurrentOrder.ScheduledDateTime = scheduledDateTime;
            }

        public void AddOrderItem(OrderItem item)
            {
            CurrentOrder.Items.Add(item);
            }
        }
    }
