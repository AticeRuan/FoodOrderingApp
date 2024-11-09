using FoodOrdering.MAUI.Models;
using System.Collections;
using Microsoft.Maui.Controls;

namespace FoodOrdering.MAUI.Views.Components;

public partial class CustomPicker : ContentView
    {
    public CustomPicker()
        {
        InitializeComponent();
        }


    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
        nameof(Placeholder),
        typeof(string),
        typeof(CustomPicker),
        string.Empty);

    public string Placeholder
        {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
        }


    public new static readonly BindableProperty WidthProperty = BindableProperty.Create(
        nameof(Width),
        typeof(double),
        typeof(CustomPicker),
        (double)200);

    public new double Width
        {
        get => (double)GetValue(WidthProperty);
        set => SetValue(WidthProperty, value);
        }


    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource),
        typeof(IList),
        typeof(CustomPicker),
        null);

    public IList ItemsSource
        {
        get => (IList)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
        }


    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
        nameof(SelectedItem),
        typeof(object),
        typeof(CustomPicker),
        null,
        BindingMode.TwoWay,
        propertyChanged: OnSelectedItemChanged);

    public object SelectedItem
        {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
        }


    public event EventHandler<object>? SelectedItemChanged;


    public static readonly BindableProperty IsForDateProperty = BindableProperty.Create(
        nameof(IsForDate),
        typeof(bool),
        typeof(CustomPicker),
        false,
        propertyChanged: OnIsForDateChanged);

    public bool IsForDate
        {
        get => (bool)GetValue(IsForDateProperty);
        set => SetValue(IsForDateProperty, value);
        }


    private static void OnIsForDateChanged(BindableObject bindable, object oldValue, object newValue)
        {
        var picker = (CustomPicker)bindable;
        picker.UpdateItemDisplayBinding();
        }

    private void UpdateItemDisplayBinding()
        {
        if (IsForDate && ItemsSource is IList<DateSlot> dateSource)
            {
            PickerControl.ItemsSource = (IList)dateSource;
            }
        else if (!IsForDate && ItemsSource is IList<TimeSlot> timeSource)
            {
            PickerControl.ItemsSource = (IList)timeSource;
            }
        }


    private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
        if (sender is Picker picker)
            {
            SelectedItem = picker.SelectedItem; 
            SelectedItemChanged?.Invoke(this, picker.SelectedItem);  
            }
        }


    private static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
        var customPicker = (CustomPicker)bindable;
        if (customPicker.PickerControl.SelectedItem != newValue)
            {
            customPicker.PickerControl.SelectedItem = newValue;  
            }
        }
    }