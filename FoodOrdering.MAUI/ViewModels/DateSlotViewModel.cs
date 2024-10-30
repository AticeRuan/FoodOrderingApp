using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using FoodOrdering.MAUI.Models;

namespace FoodOrdering.MAUI.ViewModels
    {
    public partial class DateSlotViewModel : ObservableObject
        {
        public DateSlotViewModel()
            {
            DateSlots = new ObservableCollection<DateSlot>();
            SelectedDateSlot = new DateSlot();
            LoadDateSlots();
            SelectedDateSlot = DateSlots.FirstOrDefault() ?? new DateSlot();
            }


        [ObservableProperty]
        private ObservableCollection<DateSlot> dateSlots;


        [ObservableProperty]
        private DateSlot selectedDateSlot;


        private void LoadDateSlots()
            {
            DateTime today = DateTime.UtcNow.ToLocalTime();
            int slotId = 1;

            // Check if the current time is after 9 PM
            if (today.Hour >= 21)
                {
                // Start from tomorrow if current time is past 9 PM
                today = today.AddDays(1).Date;
                }
            else
                {
                // Otherwise, start from today
                today = today.Date;
                }

            // Generate date slots for the next 7 days
            for (int i = 0; i < 7; i++)
                {
                DateTime date = today.AddDays(i);
                DateSlots.Add(new DateSlot
                    {
                    Id = slotId++,
                    Date = date
                    });
                }
            }

        }
    }
