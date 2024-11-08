using FoodOrdering.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;


using System.Runtime.CompilerServices;

namespace FoodOrdering.MAUI.Services
    {
    public class OrderService
        {
        // The main Order instance to hold all details
        public Order _currentOrder { get; private set; } = new Order();
        public event PropertyChangedEventHandler? PropertyChanged;
        public Order CurrentOrder
            {
            get => _currentOrder;
            private set
                {
                if (_currentOrder != value)
                    {
                    _currentOrder = value;
                    OnPropertyChanged(nameof(CurrentOrder));
                    }
                }
            }
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            Console.WriteLine($"OrderService property changed: {propertyName}");
            }

        public void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
            {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            });
            }
        public void ResetOrder()
            {
            CurrentOrder = new Order();
            CurrentOrder.SetCustomerDetails(CurrentOrder.CustomerName,CurrentOrder.CustomerAddress);
            NotifyPropertyChanged(nameof(CurrentOrder));
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
            CurrentOrder.UpdateTotalAmount();
            NotifyPropertyChanged(nameof(CurrentOrder));
            Console.WriteLine($"Item added to order. Total items: {CurrentOrder.Items.Count}");
            }
        public void UpdateOrder()
            {
            CurrentOrder.UpdateTotalAmount();
            NotifyPropertyChanged(nameof(CurrentOrder));
            }
        public void RemoveItem(OrderItem item)
            {
            if (CurrentOrder.Items.Contains(item))
                {
                CurrentOrder.Items.Remove(item);
                CurrentOrder.UpdateTotalAmount();
                NotifyPropertyChanged(nameof(CurrentOrder));
                }
            }

        public void UpdateItemQuantity(OrderItem item, int quantity)
            {
            if (item != null && quantity > 0)
                {
                item.Quantity = quantity;
                CurrentOrder.UpdateTotalAmount();
                NotifyPropertyChanged(nameof(CurrentOrder));
                }
            }
        }
        }
    
