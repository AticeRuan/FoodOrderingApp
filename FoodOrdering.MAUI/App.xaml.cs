#if WINDOWS
using Microsoft.UI.Windowing;
using Microsoft.UI;
using Windows.Graphics;
using WinRT.Interop;
#endif

namespace FoodOrdering.MAUI


    {
    public partial class App : Application
        {
        public App()
            {
            InitializeComponent();

            MainPage = new AppShell();
            }
        protected override Window CreateWindow(IActivationState? activationState)
            {
            var window = base.CreateWindow(activationState);

#if WINDOWS
                       const int newWidth = 450;
                       const int newHeight = 1000;

                       
            window.Width = newWidth;
            window.Height = newHeight;

            window.MaximumHeight = newHeight;
            window.MinimumHeight = newHeight;
            window.MaximumWidth = newWidth;
            window.MinimumWidth = newWidth;
#endif

            return window;
            }
        }

    }
