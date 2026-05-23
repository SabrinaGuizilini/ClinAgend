using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClinAgend.Core.Services;

public class LogoutModel : PageModel
{
    private readonly AuthService _authService;

    public LogoutModel(AuthService authService)
    {
        _authService = authService;
    }

    public async Task<IActionResult> OnGet()
    {
        await _authService.LogoutAsync();
        return Redirect("/login");
    }
}
