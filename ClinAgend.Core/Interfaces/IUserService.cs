using ClinAgend.Data.Models;
using ClinAgend.Models.DTOs;
using ClinAgend.Models.Enums;

namespace ClinAgend.Core.Interfaces
{
    public interface IUserService
    {
        Task<OperationResult> DeleteUserAsync(string userId);
        Task<OperationResult> UpdateUserNameAsync(string userId, string newName);
        Task<OperationResult> UpdateUserTypeAsync(string userId, UserType newType);
        Task<(List<ApplicationUser> Users, int TotalCount)> GetUsersPaginatedAsync(
            int clinicId,
            int page,
            int pageSize,
            string? search = null,
            string? sortBy = null,
            bool descending = false);
    }
}
