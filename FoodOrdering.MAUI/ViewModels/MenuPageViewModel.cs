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

        // Subscribe to navigation service events
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

    public async void OnPageAppearing()
        {
        await LoadMenuItems();
        _menuNav?.SetCategories(Categories);
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

            // Log the fetched data
            Debug.WriteLine($"Fetched {menuItems.Count} menu items:");
            foreach (var item in menuItems)
                {
                Debug.WriteLine($"ID: {item.Id}, Name: {item.Name}, Category: {item.Category}, Price: {item.Price}");
                }

            // Group items by category
            var groupedItems = menuItems
                .GroupBy(item => item.Category)
                .Select(group => new KeyValuePair<string, List<FoodMenuItem>>(
                    group.Key, group.ToList()))
                .OrderBy(kvp => kvp.Key);

            // Log the grouped structure
            Debug.WriteLine("\nGrouped Structure:");
            foreach (var group in groupedItems)
                {
                Debug.WriteLine($"Category: {group.Key}, Items: {group.Value.Count}");
                foreach (var item in group.Value)
                    {
                    Debug.WriteLine($"  - {item.Name}");
                    }
                }

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
            }
        }
    public void OnContentScrolled(double scrollY)
        {
        UpdateVisibleCategory(scrollY);
        }

    private void UpdateVisibleCategory(double scrollY)
        {
        if (_menuItemsCollection == null) return;

        var categories = Categories.ToList();
        var approximateItemHeight = 150; // Approximate height of each category section

        // Calculate which category should be visible based on scroll position
        int visibleIndex = (int)(scrollY / approximateItemHeight);
        visibleIndex = Math.Min(visibleIndex, categories.Count - 1);
        visibleIndex = Math.Max(0, visibleIndex);

        var visibleCategory = categories[visibleIndex];
        _navigationService.NotifyContentScrolled(visibleCategory);
        }

    private async void OnRequestScrollToCategory(object? sender, string category)
        {
        if (_scrollView == null || string.IsNullOrEmpty(category)) return;

        var categories = Categories.ToList();
        var categoryIndex = categories.IndexOf(category);
        if (categoryIndex == -1) return;

        var approximateItemHeight = 150; // Approximate height of each category section
        var targetScrollPosition = categoryIndex * approximateItemHeight;

        await _scrollView.ScrollToAsync(0, targetScrollPosition, true);
        }


    public void Cleanup()
        {
        if (_navigationService != null)
            {
            _navigationService.RequestScrollToCategory -= OnRequestScrollToCategory;
            }
        }
    }