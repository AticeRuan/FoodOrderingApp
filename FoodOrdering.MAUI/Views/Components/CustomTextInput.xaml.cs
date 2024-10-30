namespace FoodOrdering.MAUI.Views.Components;

public partial class CustomTextInput : ContentView
    {
    public CustomTextInput()
        {
        InitializeComponent();
     
        }
    // Bindable property for Text (input value)
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(CustomTextInput),
        string.Empty,
        BindingMode.TwoWay);

    public string Text
        {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
        }

    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
        nameof(Placeholder),
        typeof(string),
        typeof(CustomTextInput),
        string.Empty);

    public string Placeholder
        {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
        }

    // Bindable property for WidthRequest
    public new static readonly BindableProperty WidthProperty = BindableProperty.Create(
        nameof(Width),
        typeof(double),
        typeof(CustomTextInput),
        (double)100);

    public new double Width
        {
        get => (double)GetValue(WidthProperty);
        set => SetValue(WidthProperty, value);
        }

    // Event to pass input back to the parent
    public event EventHandler<string>? TextChangedEvent;

    // Handle TextChanged to raise the event
    private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
        // Update the bindable Text property
        Text = e.NewTextValue;

        // Trigger the custom event for the parent component
        TextChangedEvent?.Invoke(this, e.NewTextValue);
        }
    }
