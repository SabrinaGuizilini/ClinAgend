namespace ClinAgend.Core.Interfaces
{
    public interface ICurrentUserService
    {
        Task<string?> GetUserIdAsync();
        Task<string?> GetUserNameAsync();
        Task<int?> GetUserClinicIdAsync();
        Task<bool> IsAuthenticatedAsync();
        Task<string?> GetUserEmailAsync();
        Task<string?> GetUserTypeAsync();
    }

}
