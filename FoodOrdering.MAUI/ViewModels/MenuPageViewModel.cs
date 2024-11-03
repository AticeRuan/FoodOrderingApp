using CommunityToolkit.Mvvm.ComponentModel;
using FoodOrdering.MAUI.Services;
using FoodOrdering.MAUI.Views.Components;
using FoodOrdering.Shared.Models;
using System.Collections.ObjectModel;

namespace FoodOrdering.MAUI.ViewModels;

public partial class MenuPageViewModel : ObservableObject
    {
    private readonly MenuNavigationService _navigationService;
    private ScrollView? _scrollView;
    private CollectionView? _menuItemsCollection;
    private MenuNavComponent? _menuNav;
    private double _lastKnownScrollPosition;

    [ObservableProperty]
    private ObservableCollection<KeyValuePair<string, List<FoodMenuItem>>> _categoryGroups;

    public IEnumerable<string> Categories => CategoryGroups.Select(g => g.Key);

    public MenuPageViewModel(MenuNavigationService navigationService)
        {
        _navigationService = navigationService;
        _categoryGroups = new ObservableCollection<KeyValuePair<string, List<FoodMenuItem>>>();
        LoadDummyData();

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

    public void OnPageAppearing()
        {
        _menuNav?.SetCategories(Categories);
        }

    public void OnContentScrolled(double scrollY)
        {
        _lastKnownScrollPosition = scrollY;
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

    private void LoadDummyData()
        {
        var dummyData = new[]
        {
        new KeyValuePair<string, List<FoodMenuItem>>("Appetizers", new List<FoodMenuItem>
        {
            new()
            {
                Id = 1,
                Name = "Spring Rolls",
                Description = "Crispy vegetable spring rolls served with sweet chili sauce",
                Price = 8.99m,
                Category = "Appetizers",
                IsAvailable = true
            },
            new()
            {
                Id = 2,
                Name = "Dumplings",
                Description = "Steamed pork dumplings with ginger soy dipping sauce",
                Price = 9.99m,
                Category = "Appetizers",
                IsAvailable = true
            },
            new()
            {
                Id = 3,
                Name = "Hot & Sour Soup",
                Description = "Traditional Chinese soup with tofu, bamboo shoots, and mushrooms",
                Price = 7.99m,
                Category = "Appetizers",
                IsAvailable = true
            }
        }),

        new KeyValuePair<string, List<FoodMenuItem>>("Main Dishes", new List<FoodMenuItem>
        {
            new()
            {
                Id = 4,
                Name = "Kung Pao Chicken",
                Description = "Spicy diced chicken with peanuts, vegetables, and chili peppers",
                Price = 15.99m,
                Category = "Main Dishes",
                IsAvailable = true
            },
            new()
            {
                Id = 5,
                Name = "Beef with Broccoli",
                Description = "Tender beef and fresh broccoli in savory brown sauce",
                Price = 16.99m,
                Category = "Main Dishes",
                IsAvailable = true
            },
            new()
            {
                Id = 6,
                Name = "Sweet and Sour Pork",
                Description = "Crispy pork with pineapple and vegetables in sweet and sour sauce",
                Price = 14.99m,
                Category = "Main Dishes",
                IsAvailable = true
            }
        }),

        new KeyValuePair<string, List<FoodMenuItem>>("Rice & Noodles", new List<FoodMenuItem>
        {
            new()
            {
                Id = 7,
                Name = "Special Fried Rice",
                Description = "Wok-fried rice with shrimp, chicken, eggs, and mixed vegetables",
                Price = 12.99m,
                Category = "Rice & Noodles",
                IsAvailable = true
            },
            new()
            {
                Id = 8,
                Name = "Chow Mein",
                Description = "Stir-fried noodles with bean sprouts, onions, and celery",
                Price = 13.99m,
                Category = "Rice & Noodles",
                IsAvailable = true
            },
            new()
            {
                Id = 9,
                Name = "Singapore Noodles",
                Description = "Curry-flavored rice noodles with shrimp, chicken, and vegetables",
                Price = 14.99m,
                Category = "Rice & Noodles",
                IsAvailable = true
            }
        }),

        new KeyValuePair<string, List<FoodMenuItem>>("Beverages", new List<FoodMenuItem>
        {
            new()
            {
                Id = 10,
                Name = "Chinese Tea",
                Description = "Traditional Chinese jasmine tea (per person)",
                Price = 3.99m,
                Category = "Beverages",
                IsAvailable = true
            },
            new()
            {
                Id = 11,
                Name = "Soft Drinks",
                Description = "Coke, Sprite, Fanta, Diet Coke (330ml)",
                Price = 2.99m,
                Category = "Beverages",
                IsAvailable = true
            },
            new()
            {
                Id = 12,
                Name = "Fresh Fruit Smoothie",
                Description = "Mango, strawberry, or mixed berry smoothie",
                Price = 5.99m,
                Category = "Beverages",
                IsAvailable = true
            }
        }),

        new KeyValuePair<string, List<FoodMenuItem>>("Desserts", new List<FoodMenuItem>
        {
            new()
            {
                Id = 13,
                Name = "Fortune Cookies",
                Description = "Classic fortune cookies (3 pieces)",
                Price = 1.99m,
                Category = "Desserts",
                IsAvailable = true
            },
            new()
            {
                Id = 14,
                Name = "Fried Ice Cream",
                Description = "Crispy coated vanilla ice cream with honey drizzle",
                Price = 6.99m,
                Category = "Desserts",
                IsAvailable = true
            },
            new()
            {
                Id = 15,
                Name = "Mango Pudding",
                Description = "Fresh mango pudding with condensed milk",
                Price = 5.99m,
                Category = "Desserts",
                IsAvailable = true
            }
        })
    };

        CategoryGroups.Clear();
        foreach (var item in dummyData)
            {
            CategoryGroups.Add(item);
            }
        OnPropertyChanged(nameof(Categories));
        }
    public void Cleanup()
        {
        if (_navigationService != null)
            {
            _navigationService.RequestScrollToCategory -= OnRequestScrollToCategory;
            }
        }
    }