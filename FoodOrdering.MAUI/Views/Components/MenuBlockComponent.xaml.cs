using FoodOrdering.Shared.Models;

namespace FoodOrdering.MAUI.Views.Components;

public partial class MenuBlockComponent : ContentView
    {
    public MenuBlockComponent()
        {
        InitializeComponent();
        BindingContext = this;
        }

    public static readonly BindableProperty CategoryNameProperty = BindableProperty.Create(
        nameof(CategoryName),
        typeof(string),
        typeof(MenuBlockComponent),
        string.Empty);

    public string CategoryName
        {
        get => (string)GetValue(CategoryNameProperty);
        set => SetValue(CategoryNameProperty, value);
        }

    public static readonly BindableProperty MenuItemsProperty = BindableProperty.Create(
        nameof(MenuItems),
        typeof(IEnumerable<FoodMenuItem>),
        typeof(MenuBlockComponent),
        defaultValue: Enumerable.Empty<FoodMenuItem>());

    public IEnumerable<FoodMenuItem> MenuItems
        {
        get => (IEnumerable<FoodMenuItem>)GetValue(MenuItemsProperty);
        set => SetValue(MenuItemsProperty, value);
        }
    }