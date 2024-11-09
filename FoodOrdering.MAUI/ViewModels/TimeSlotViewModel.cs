using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using FoodOrdering.MAUI.Models;

namespace FoodOrdering.MAUI.ViewModels
    {
    public partial class TimeSlotViewModel : ObservableObject
        {


        [ObservableProperty]
        private ObservableCollection<TimeSlot> timeSlots = new();

        [ObservableProperty]
        private TimeSlot? selectedTimeSlot;
        public TimeSlotViewModel()
            {
      
            LoadTimeSlots(DateTime.Today);
            TimeSlots = new ObservableCollection<TimeSlot>();
            }



        public void LoadTimeSlots(DateTime selectedDate)
            {
            TimeSlots.Clear();

            DateTime currentTime = DateTime.Now;
            DateTime openingTime = selectedDate.Date.AddHours(11); // 11 AM
            DateTime closingTime = selectedDate.Date.AddHours(21); // 9 PM

   
            DateTime startTime;
            if (selectedDate.Date == currentTime.Date)
                {
                if (currentTime.TimeOfDay < TimeSpan.FromHours(11))
                    {
                    
                    startTime = openingTime;
                    }
                else
                    {
                  
                    int minutes = currentTime.Minute;
                    int minutesToAdd = 15 - (minutes % 15);
                    startTime = currentTime.AddMinutes(minutesToAdd).AddSeconds(-currentTime.Second);

                 
                    if (startTime >= closingTime)
                        {
                        return;
                        }
                    }
                }
            else
                {
         
                startTime = openingTime;
                }

            int slotId = 1;
            for (DateTime time = startTime; time < closingTime; time = time.AddMinutes(15))
                {
                TimeSlots.Add(new TimeSlot
                    {
                    Id = slotId++,
                    StartTime = time
                    });
                }

 
            SelectedTimeSlot = TimeSlots.FirstOrDefault();
            }
        }
    }
