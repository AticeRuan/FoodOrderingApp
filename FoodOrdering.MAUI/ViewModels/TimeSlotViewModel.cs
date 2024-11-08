using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using FoodOrdering.MAUI.Models;

namespace FoodOrdering.MAUI.ViewModels
    {
    public partial class TimeSlotViewModel : ObservableObject
        {
        public TimeSlotViewModel()
            {
            TimeSlots = new ObservableCollection<TimeSlot>();
            LoadTimeSlots(DateTime.Today);
            }

        [ObservableProperty]
        private ObservableCollection<TimeSlot> timeSlots = new();

        [ObservableProperty]
        private TimeSlot? selectedTimeSlot;

        public void LoadTimeSlots(DateTime selectedDate)
            {
    
            DateTime currentTime = DateTime.Now;
            DateTime startOfDay = selectedDate.Date.AddHours(11);
            DateTime endOfDay = selectedDate.Date.AddHours(21);

            DateTime slotTime;

            if (selectedDate.Date == currentTime.Date && currentTime.TimeOfDay > TimeSpan.FromHours(11))
                {
                int minutesToNextQuarter = 30 - (currentTime.Minute % 15);
                slotTime = currentTime.AddMinutes(minutesToNextQuarter).AddSeconds(-currentTime.Second);
                }
            else
                {
                slotTime = startOfDay;
                }

            // Generate time slots in 15-minute intervals within the working hours
            int slotId = 1;
            for (; slotTime < endOfDay; slotTime = slotTime.AddMinutes(15))
                {
                TimeSlots.Add(new TimeSlot
                    {
                    Id = slotId++,
                    StartTime = slotTime
                    });
                }
            SelectedTimeSlot = TimeSlots.FirstOrDefault();
            }
        }
    }
