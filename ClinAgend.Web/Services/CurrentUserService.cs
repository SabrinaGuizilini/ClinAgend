using System.Security.Claims;
using ClinAgend.Core.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;

namespace ClinAgend.Web.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public CurrentUserService(AuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider;
    }

    private async Task<ClaimsPrincipal?> GetUserAsync()
    {
        var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
        return state?.User;
    }

    public async Task<string?> GetUserIdAsync()
        => (await GetUserAsync())?.FindFirstValue(ClaimTypes.NameIdentifier);

    public async Task<string?> GetUserEmailAsync()
        => (await GetUserAsync())?.FindFirstValue(ClaimTypes.Email);

    public async Task<string?> GetUserNameAsync()
        => (await GetUserAsync())?.FindFirst("FullName")?.Value;

    public async Task<string?> GetUserTypeAsync()
        => (await GetUserAsync())?.FindFirst("UserType")?.Value;

    public async Task<int?> GetUserClinicIdAsync()
    {
        var claim = (await GetUserAsync())?.FindFirst("ClinicId")?.Value;
        return int.TryParse(claim, out var id) ? id : (int?)null;
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var user = await GetUserAsync();
        return user?.Identity?.IsAuthenticated == true;
    }
}
