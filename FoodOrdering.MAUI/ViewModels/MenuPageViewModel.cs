using CommunityToolkit.Mvvm.ComponentModel;
using FoodOrdering.MAUI.Services;
using FoodOrdering.MAUI.Views.Components;
using FoodOrdering.Shared.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FoodOrdering.MAUI.ViewModels;

public partial class MenuPageViewModel : ObservableObject
    {
    private readonly MenuNavigationService _navigationService;
    private readonly IApiService _apiService;
    private ScrollView? _scrollView;
    private CollectionView? _menuItemsCollection;
    private MenuNavComponent? _menuNav;
    private Dictionary<string, CategoryMetrics> _categoryMetrics = new();

    private const double CATEGORY_HEADER_HEIGHT = 44;
    private const double MENU_ITEM_HEIGHT = 120;     
    private const double CATEGORY_MARGIN = 30;

    [ObservableProperty]
    private bool isBusy;


    [ObservableProperty]
    private ObservableCollection<KeyValuePair<string, List<FoodMenuItem>>> _categoryGroups;


    public IEnumerable<string> Categories => CategoryGroups.Select(g => g.Key);

    public MenuPageViewModel(MenuNavigationService navigationService , IApiService apiService)
        {
        _navigationService = navigationService;
        _apiService = apiService;
        _categoryGroups = new ObservableCollection<KeyValuePair<string, List<FoodMenuItem>>>();

       
        _navigationService.RequestScrollToCategory += OnRequestScrollToCategory;
        }

    public void SetScrollView(ScrollView scrollView)
        {
        _scrollView = scrollView;
        }

    public void SetMenuItemsCollection(CollectionView menuItems)
        {
        _menuItemsCollection = menuItems;
        }

    public void SetMenuNav(MenuNavComponent menuNav)
        {
        _menuNav = menuNav;
        }

    private void CalculateCategoryMetrics()
        {
        _categoryMetrics.Clear();
        double currentOffset = 0;

        foreach (var group in CategoryGroups)
            {
            var itemCount = group.Value.Count;
            var categoryHeight = CATEGORY_HEADER_HEIGHT + (itemCount * MENU_ITEM_HEIGHT);

            _categoryMetrics[group.Key] = new CategoryMetrics
                {
                StartOffset = currentOffset,
                Height = categoryHeight,
                ItemCount = itemCount
                };

            currentOffset += categoryHeight + CATEGORY_MARGIN;
            }
        }

    public async void OnPageAppearing()
        {
        await LoadMenuItems();
        _menuNav?.SetCategories(Categories);
        CalculateCategoryMetrics();
        }
    private async Task LoadMenuItems()
        {
        if (IsBusy) return;
        try
            {
            IsBusy = true;
            CategoryGroups.Clear();

            // Fetch menu items from API
            var menuItems = await _apiService.GetMenuItemsAsync();

       

            // Group items by category
            var groupedItems = menuItems
                .GroupBy(item => item.Category)
                .Select(group => new KeyValuePair<string, List<FoodMenuItem>>(
                    group.Key, group.ToList()))
                .OrderBy(kvp => kvp.Key);

       

            foreach (var group in groupedItems)
                {
                CategoryGroups.Add(group);
                }

            OnPropertyChanged(nameof(Categories));
            }
        catch (Exception ex)
            {
            Debug.WriteLine($"Error loading menu items: {ex.Message}");
            Debug.WriteLine($"Stack trace: {ex.StackTrace}");

            if (Application.Current?.MainPage != null)
                {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Unable to load menu items. Please try again later.",
                    "OK");
                }
            }
        finally
            {
            IsBusy = false;
            CalculateCategoryMetrics();
            }
        }
    public void OnContentScrolled(double scrollY)
        {
        UpdateVisibleCategory(scrollY);
        }

    private void UpdateVisibleCategory(double scrollY)
        {
        if (_menuItemsCollection == null || !_categoryMetrics.Any()) return;

        // Find the category that contains the current scroll position
        var visibleCategory = _categoryMetrics
            .Where(kvp => scrollY >= kvp.Value.StartOffset)
            .OrderByDescending(kvp => kvp.Value.StartOffset)
            .FirstOrDefault();

        if (!string.IsNullOrEmpty(visibleCategory.Key))
            {
            _navigationService.NotifyContentScrolled(visibleCategory.Key);
            }
        }

    private async void OnRequestScrollToCategory(object? sender, string category)
        {
        if (_scrollView == null || string.IsNullOrEmpty(category)) return;

        if (_categoryMetrics.TryGetValue(category, out var metrics))
            {
            // Add a small offset to ensure the category header is visible
            var targetPosition = metrics.StartOffset;
            await _scrollView.ScrollToAsync(0, targetPosition, true);
            }
        }


    public void Cleanup()
        {
        if (_navigationService != null)
            {
            _navigationService.RequestScrollToCategory -= OnRequestScrollToCategory;
            }
        }
    private class CategoryMetrics
        {
        public double StartOffset { get; set; }
        public double Height { get; set; }
        public int ItemCount { get; set; }
        }
    }