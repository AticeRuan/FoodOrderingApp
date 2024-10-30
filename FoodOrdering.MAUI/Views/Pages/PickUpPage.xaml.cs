using System.Collections.ObjectModel;

namespace FoodOrdering.MAUI.Pages;

public partial class PickUpPage : ContentPage
{

    public PickUpPage()
	{
		InitializeComponent();  
       
    
	}

    private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
        await Shell.Current.GoToAsync(nameof(Pages.DeliveryPage));
        }

 
    private async void StartOrderButton_Clicked(object sender, EventArgs e)
        {
        await Shell.Current.GoToAsync(nameof(Pages.MenuPage));
        }

    private void DatePicker_SelectedItemChanged(object sender, string e)
        {

        }

    private void TimePicker_SelectedItemChanged(object sender, string e)
        {

        }
    }