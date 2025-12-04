using Project.Domain.DataFilters;

namespace Project.EntityFrameworkCore.DataFilters;

public class DataFilter : IDataFilter
{
    private static readonly AsyncLocal<Dictionary<Type, bool>> _filters = new();

    public DataFilter()
    {
        if (_filters.Value == null)
        {
            _filters.Value = new Dictionary<Type, bool>();
        }
    }

    public IDisposable Enable<TFilter>() where TFilter : class
    {
        return Change<TFilter>(true);
    }

    public IDisposable Disable<TFilter>() where TFilter : class
    {
        return Change<TFilter>(false);
    }

    public bool IsEnabled<TFilter>() where TFilter : class
    {
        if (_filters.Value != null && _filters.Value.TryGetValue(typeof(TFilter), out var isEnabled))
        {
            return isEnabled;
        }
        return true; // Default to enabled
    }

    private IDisposable Change<TFilter>(bool isEnabled) where TFilter : class
    {
        var previousState = IsEnabled<TFilter>();
        if (_filters.Value == null)
        {
            _filters.Value = new Dictionary<Type, bool>();
        }
        
        _filters.Value[typeof(TFilter)] = isEnabled;

        return new DisposeAction(() =>
        {
            if (_filters.Value != null)
            {
                _filters.Value[typeof(TFilter)] = previousState;
            }
        });
    }

    private class DisposeAction : IDisposable
    {
        private readonly Action _action;

        public DisposeAction(Action action)
        {
            _action = action;
        }

        public void Dispose()
        {
            _action();
        }
    }
}

public class DataFilter<TFilter> : IDataFilter<TFilter> where TFilter : class
{
    private readonly IDataFilter _dataFilter;

    public DataFilter(IDataFilter dataFilter)
    {
        _dataFilter = dataFilter;
    }

    public bool IsEnabled => _dataFilter.IsEnabled<TFilter>();
}
