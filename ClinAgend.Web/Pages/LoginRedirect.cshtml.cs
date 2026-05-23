using ClinAgend.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class LoginRedirectModel : PageModel
{
    private readonly AuthService _authService;

    public LoginRedirectModel(AuthService authService)
    {
        _authService = authService;
    }

    public async Task<IActionResult> OnPostAsync(string clinic, string email, string password, string? returnUrl)
    {
        try
        {
            var result = await _authService.LoginAsync(clinic, email, password);

            if (!result.Success)
                return Redirect($"/login?error={Uri.EscapeDataString(result.Message)}");

            return !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)
                ? Redirect(returnUrl)
                : Redirect("/");
        }
        catch (Exception ex)
        {
            return Redirect("/login?error=Erro inesperado. Tente novamente.");
        }
    }
}
