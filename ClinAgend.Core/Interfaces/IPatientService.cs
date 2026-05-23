using ClinAgend.Models.DTOs;
using ClinAgend.Models.Enums;
using ClinAgend.Models.Models;

namespace ClinAgend.Core.Interfaces
{
    public interface IPatientService
    {
        Task<Patient?> GetByIdAsync(int patientId);
        Task<OperationResult> SavePatientAsync(Patient patient);
        Task<(List<Patient> Patients, int TotalCount)> GetPaginatedAsync(
            int clinicId, int page, int pageSize, EntityStatusFilter statusFilter = EntityStatusFilter.Active, string? search = null, string? sortBy = null, bool descending = false);
        Task<OperationResult> DeletePatientAsync(int patientId);
        Task<IEnumerable<PatientLookupDTO>> SearchByNameAsync(string? name, CancellationToken cancellationToken = default);
    }
}

