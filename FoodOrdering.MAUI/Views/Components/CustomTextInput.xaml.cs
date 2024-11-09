namespace FoodOrdering.MAUI.Views.Components;

public partial class CustomTextInput : ContentView
    {
    public CustomTextInput()
        {
        InitializeComponent();
     
        }

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(CustomTextInput),
        string.Empty,
        BindingMode.TwoWay,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var control = (CustomTextInput)bindable;
            control.UpdateEntryText((string)newValue);
        });



    protected override void OnBindingContextChanged()
        {
        base.OnBindingContextChanged();
            EntryControl.BindingContext = BindingContext;
        }

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

    private void UpdateEntryText(string newValue)
        {
        if (EntryControl != null && EntryControl.Text != newValue)
            {
            EntryControl.Text = newValue;
            }
        }
    protected override void OnHandlerChanged()
        {
        base.OnHandlerChanged();
        if (EntryControl != null)
            {
            UpdateEntryText(Text);
            }
        }
    }
