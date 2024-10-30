using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.MAUI.Models
    {
    public  partial class DateSlot
        {
        public int Id { get; set; }
        public DateTime Date { get; set; }

        public override string ToString() => Date.ToString("dd MMMM, yyyy"); 
        }
    }
