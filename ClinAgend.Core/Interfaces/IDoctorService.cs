using ClinAgend.Models.DTOs;
using ClinAgend.Models.Enums;
using ClinAgend.Models.Models;

namespace ClinAgend.Core.Interfaces
{
    public interface IDoctorService
    {
        Task<Doctor?> GetByIdAsync(int doctorId);
        Task<(List<Doctor> Doctors, int TotalCount)> GetPaginatedAsync(
           int clinicId, int page, int pageSize, EntityStatusFilter statusFilter = EntityStatusFilter.Active, string? search = null, string? sortBy = null, bool descending = false);
        Task<OperationResult> SaveDoctorAsync(Doctor doctor);
        Task<OperationResult> DeleteDoctorAsync(int doctorId);
        Task<IEnumerable<DoctorLookupDTO>> SearchByNameAsync(string? name, CancellationToken cancellationToken = default);
        Task LinkUserAsync(int doctorId, string? userId);
        Task<Doctor?> GetDoctorByUserId(string userId);
    }
}
