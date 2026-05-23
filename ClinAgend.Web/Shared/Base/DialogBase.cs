using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ClinAgend.Web.Shared.Base
{
    public abstract class DialogBase : PageBase
    {
        [CascadingParameter]
        protected IMudDialogInstance MudDialog { get; set; } = default!;

        protected void Close()
        {
            MudDialog.Close();
        }

        protected void CloseOk()
        {
            MudDialog.Close(DialogResult.Ok(true));
        }

        protected void CloseOk<T>(T data)
        {
            MudDialog.Close(DialogResult.Ok(data));
        }

        protected void Cancel()
        {
            MudDialog.Cancel();
        }

        protected async Task ExecuteAsync(
            Func<Task<bool>> action,
            string? successMessage = null,
            string? errorMessage = null)
        {
            try
            {
                bool success = await action();

                if (success && !string.IsNullOrWhiteSpace(successMessage))
                    ShowSuccess(successMessage);
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrWhiteSpace(errorMessage))
                    ShowError(errorMessage);
                else
                    ShowError(ex.Message);
            }
        }
    }
}
