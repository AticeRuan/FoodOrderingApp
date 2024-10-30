using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using System.Windows.Input;

namespace FoodOrdering.MAUI.Views.Components
    {
    public partial class CustomButton : ContentView
        {
        public CustomButton()
            {
            InitializeComponent();
       
            UpdateCornerShape();
            }

        // Public Clicked event for external handling
        public event EventHandler? Clicked;

       

        // Bindable property for Command
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(
            nameof(Command),
            typeof(ICommand),
            typeof(CustomButton),
            null);

        public ICommand? Command
            {
            get => (ICommand?)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
            }
        // Bindable property for CommandParameter
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(
            nameof(CommandParameter),
            typeof(object),
            typeof(CustomButton),
            null);

        public object? CommandParameter
            {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
            }

        // Method to handle tap event
        private void OnButtonTapped(object sender, EventArgs e)
            {
            // Invoke the Clicked event for external subscribers
            Clicked?.Invoke(this, e);

            // Execute the command if it's set and can execute
            if (Command != null && Command.CanExecute(CommandParameter))
                {
                Console.WriteLine("Executing command");
                Command.Execute(CommandParameter);
                }
            else
                {
                Console.WriteLine("Command is null or cannot execute");
                }
            }

        // Bindable property for Text
        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            nameof(Text),
            typeof(string),
            typeof(CustomButton),
            string.Empty);

        public string Text
            {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
            }

        // Bindable property for TextColor
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
            nameof(TextColor),
            typeof(Color),
            typeof(CustomButton),
            Colors.Black);

        public Color TextColor
            {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
            }

        // Bindable property for BackgroundColor
        public new static readonly BindableProperty BackgroundColorProperty = BindableProperty.Create(
            nameof(BackgroundColor),
            typeof(Color),
            typeof(CustomButton),
            Colors.Transparent);





        public new static readonly BindableProperty WidthProperty = BindableProperty.Create(
           nameof(Width),
           typeof(double),
           typeof(CustomButton),
           default(double));




        public new double Width
            {
            get => (double)GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
            }

        public new Color BackgroundColor
            {
            get => (Color)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
            }

        // Bindable property for Icon
        public static readonly BindableProperty IconProperty = BindableProperty.Create(
            nameof(Icon),
            typeof(ImageSource),
            typeof(CustomButton),
            default(ImageSource));

        public ImageSource Icon
            {
            get => (ImageSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
            }

        // Bindable property for CornerRadius to accept four corner values
        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(CustomButton),
            new CornerRadius(0), // Default value
            propertyChanged: OnCornerRadiusChanged);

        public CornerRadius CornerRadius
            {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
            }

        // Method to update the corner shape based on CornerRadius
        private void UpdateCornerShape()
            {
            CornerShape = new RoundRectangle
                {
                CornerRadius = CornerRadius
                };
            }

        private static void OnCornerRadiusChanged(BindableObject bindable, object oldValue, object newValue)
            {
            if (bindable is CustomButton button)
                {
                button.UpdateCornerShape();
                }
            }

        // Property to bind CornerShape in XAML
        public RoundRectangle CornerShape { get; set; } = new RoundRectangle();




        }
    }
