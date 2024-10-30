using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Shared.Models
    {
    public class Address
        {
        public string Unit { get; set; } = string.Empty;
        public string StreetNumber { get; set; } = string.Empty;
        public string StreetName { get; set; } = string.Empty;
        public string Suburb { get; set; } = string.Empty;
     
        // Full address for display or submission
        public string FullAddress => $"{Unit}, {StreetNumber}, {StreetName}, {Suburb}, Taupo";
        }

    }
