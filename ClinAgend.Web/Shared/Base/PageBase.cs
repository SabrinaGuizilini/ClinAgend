using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ClinAgend.Web.Shared.Base
{
    public abstract class PageBase : ComponentBase
    {
        [Inject] protected ISnackbar Snackbar { get; set; } = default!;
        [Inject] protected IDialogService DialogService { get; set; } = default!;

        protected DialogOptions DefaultDialogOptions => new()
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };

        protected void ShowError(string message)
        {
            Snackbar.Add(message, Severity.Error);
        }

        protected void ShowSuccess(string message)
        {
            Snackbar.Add(message, Severity.Success);
        }

        protected void ShowInfo(string message)
        {
            Snackbar.Add(message, Severity.Info);
        }

        protected async Task<bool> ShowDialogAsync<TDialog>(
          string title,
          DialogParameters parameters,
          DialogOptions? options = null)
          where TDialog : IComponent
        {
            var dialogOptions = options ?? DefaultDialogOptions;

            var dialog = await DialogService.ShowAsync<TDialog>(
                title,
                parameters,
                dialogOptions);

            var result = await dialog.Result;

            return !result.Canceled;
        }


    }
}
