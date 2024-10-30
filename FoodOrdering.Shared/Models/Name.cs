using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Shared.Models
    {
    public class Name
        {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
             
        public string FullName => $"{FirstName} {LastName}";
        }
    }
