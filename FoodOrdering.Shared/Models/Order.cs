using FoodOrdering.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Shared.Models
{
    public class Order
    {
        private decimal _totalAmount;
        private string _fullName = string.Empty;
        private string _fulladdress = string.Empty;

        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public bool IsDelivery { get; set; }

        public DateTime ScheduledDateTime { get; set; }
        public Name CustomerName { get; set; } = new Name();
        public Address CustomerAddress { get; set; } = new Address();

        public string CustomerPhone { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public List<OrderItem> Items { get; set; } = [];

        public decimal TotalAmount
            {
            get => _totalAmount;
            private set => _totalAmount = value;
            }
        public string FullName
            {
            get => _fullName;
            set => _fullName = value;
            }

        public string FullAddress
            {
            get => _fulladdress;
            set => _fulladdress = value;
            }

        public void UpdateTotalAmount()
            {
            _totalAmount = CalculateTotalPrice();
            }

        private decimal CalculateTotalPrice()
            {
            return Items.Sum(item => item.TotalPrice);
            }
        public void SetCustomerDetails(Name customerName, Address customerAddress)
            {
            _fullName = customerName.FullName;
            _fulladdress = customerAddress.FullAddress;
            }
        }

}
