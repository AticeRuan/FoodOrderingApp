using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FoodOrdering.MAUI.Services;

public class MenuNavigationService : INotifyPropertyChanged
    {
    private string _selectedCategory = string.Empty;
    private bool _isUserScrolling;
    private bool _isProgrammaticScroll;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string SelectedCategory
        {
        get => _selectedCategory;
        set
            {
            if (_selectedCategory != value)
                {
                _selectedCategory = value;
                OnPropertyChanged();
                if (!_isProgrammaticScroll)
                    {
                    RequestScrollToCategory?.Invoke(this, value);
                    }
                }
            }
        }

    public event EventHandler<string>? RequestScrollToCategory;
    public event EventHandler<string>? RequestNavScroll;

    public void NotifyContentScrolled(string category)
        {
        if (_isUserScrolling) return;

        _isProgrammaticScroll = true;
        SelectedCategory = category;
        RequestNavScroll?.Invoke(this, category);
        _isProgrammaticScroll = false;
        }

    public void BeginUserScroll()
        {
        _isUserScrolling = true;
        }

    public void EndUserScroll()
        {
        _isUserScrolling = false;
        }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }