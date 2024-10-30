namespace FoodOrdering.MAUI.Pages;

public partial class DeliveryPage : ContentPage
{
	public DeliveryPage()
	{
		InitializeComponent();
	}

    private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
        await Shell.Current.GoToAsync(nameof(Pages.PickUpPage));
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