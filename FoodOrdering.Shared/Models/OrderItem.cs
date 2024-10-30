using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Shared.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
        public int MenuItemId { get; set; }
        public FoodMenuItem? MenuItem { get; set; }
        public List<Extra> Extras { get; set; } = new List<Extra>();

        public List<string> Spice { get; set; } = new List<string>();
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public decimal TotalPrice => (UnitPrice + Extras.Sum(extra => extra.TotalPrice)) * Quantity;


        }
}
