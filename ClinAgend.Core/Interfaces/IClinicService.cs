using ClinAgend.Models.DTOs;
using ClinAgend.Models.Models;

namespace ClinAgend.Core.Interfaces
{
    public interface IClinicService
    {
        Task<string?> GetClinicNameByIdAsync(int clinicId);
        Task<Clinic?> GetClinicByIdAsync(int clinicId);
        Task<ClinicSettings?> GetClinicSettingsByClinicIdAsync(int clinicId);
        Task<List<Clinic>> GetAllClinicsAsync();
        Task<OperationResult> SaveClinicAsync(Clinic clinic);
        Task<OperationResult> SaveClinicSettingsAsync(ClinicSettings clinicSettings);
        Task<OperationResult> DeleteClinicAsync(int clinicId);
    }
}
