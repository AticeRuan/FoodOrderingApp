using FoodOrdering.MAUI.Services;
using System.Collections.ObjectModel;

namespace FoodOrdering.MAUI.Views.Components;

public partial class MenuNavComponent : ContentView
    {
    private readonly MenuNavigationService _navigationService;
    public ObservableCollection<CategoryItem> Categories { get; } = new();

    public MenuNavComponent()
        {
        InitializeComponent();
        _navigationService = Application.Current?.Handler?.MauiContext?.Services.GetService<MenuNavigationService>()
                           ?? throw new InvalidOperationException("MenuNavigationService not found");

        BindableLayout.SetItemsSource(CategoriesStack, Categories);
        _navigationService.RequestNavScroll += OnRequestNavScroll;
        }

    public void SetCategories(IEnumerable<string> categories)
        {
        Categories.Clear();
        foreach (var category in categories)
            {
            Categories.Add(new CategoryItem { Category = category });
            }

        if (Categories.Any())
            {
            UpdateSelectedCategory(Categories.First().Category);
            }
        }

    private void OnCategoryTapped(object sender, TappedEventArgs e)
        {
        if (e.Parameter is string category)
            {
            UpdateSelectedCategory(category);
            _navigationService.SelectedCategory = category;
            }
        }

    private void UpdateSelectedCategory(string category)
        {
        foreach (var item in Categories)
            {
            item.IsSelected = item.Category == category;
            }
        }

    private async void OnRequestNavScroll(object? sender, string category)
        {
        var categoryItem = Categories.FirstOrDefault(c => c.Category == category);
        if (categoryItem == null) return;

        UpdateSelectedCategory(category);

   
        var index = Categories.IndexOf(categoryItem);
        if (index >= 0)
            {
          
            var itemWidth = 100; 
            var spacing = 10;
            var totalWidth = NavScrollView.Width;
            var scrollPosition = (itemWidth + spacing) * index - (totalWidth / 2) + (itemWidth / 2);

            await NavScrollView.ScrollToAsync(Math.Max(0, scrollPosition), 0, true);
            }
        }
    }

public class CategoryItem : BindableObject
    {
    private string _category = string.Empty;
    private bool _isSelected;

    public string Category
        {
        get => _category;
        set
            {
            _category = value;
            OnPropertyChanged();
            }
        }

    public bool IsSelected
        {
        get => _isSelected;
        set
            {
            _isSelected = value;
            OnPropertyChanged();
            }
        }
    }