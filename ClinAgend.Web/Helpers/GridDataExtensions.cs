using MudBlazor;

namespace ClinAgend.Web.Helpers
{
    internal static class GridDataExtensions
    {
        internal static GridData<T> Empty<T>() =>
            new() { Items = Enumerable.Empty<T>(), TotalItems = 0 };
    }

}
