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
             LoadDateSlots();
            SelectedDateSlot = DateSlots.FirstOrDefault() ?? new DateSlot();
            }


        [ObservableProperty]
        private ObservableCollection<DateSlot> dateSlots;


        [ObservableProperty]
        private DateSlot selectedDateSlot;


        private void LoadDateSlots()
            {
            DateTime today = DateTime.Today.Date;
            int slotId = 1;
                    
            if (today.Hour >= 21)
                {
            
                today = today.AddDays(1).Date;
                }
            else
                {

                today = today.Date;
                }


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
