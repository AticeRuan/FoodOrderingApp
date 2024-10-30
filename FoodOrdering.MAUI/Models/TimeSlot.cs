using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.MAUI.Models
    {
    public partial class TimeSlot
        {
        public  int Id  { get; set; }
        public  DateTime StartTime { get; set; }

        public override string ToString() => StartTime.ToString("hh:mm tt"); 
        }
    }
