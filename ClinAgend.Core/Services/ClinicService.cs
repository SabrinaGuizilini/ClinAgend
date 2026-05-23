using ClinAgend.Core.Interfaces;
using ClinAgend.Core.Utils;
using ClinAgend.Data.Interfaces;
using ClinAgend.Models.DTOs;
using ClinAgend.Models.Models;

namespace ClinAgend.Core.Services
{
    public class ClinicService : IClinicService
    {
        private readonly IGenericRepository<Clinic> _clinicRepository;
        private readonly IGenericRepository<ClinicSettings> _clinicSettingsRepository;

        public ClinicService(IGenericRepository<Clinic> clinicRepository, IGenericRepository<ClinicSettings> clinicSettingsRepository)
        {
            _clinicRepository = clinicRepository;
            _clinicSettingsRepository = clinicSettingsRepository;
        }

        public async Task<Clinic?> GetClinicByIdAsync(int clinicId)
        {
            return await _clinicRepository.GetByAsNoTrackingAsync(d => d.ClinicId == clinicId);
        }

        public async Task<string?> GetClinicNameByIdAsync(int clinicId)
        {
            var clinic = await _clinicRepository.GetByAsNoTrackingAsync(
                c => c.ClinicId == clinicId
            );

            return clinic?.Name;
        }

        public async Task<List<Clinic>> GetAllClinicsAsync()
        {
            return await _clinicRepository.GetAllAsNoTrackingAsync();
        }

        public async Task<OperationResult> SaveClinicAsync(Clinic clinic)
        {
            try
            {
                if (clinic == null)
                    return new OperationResult(false, "Dados da clínica inválidos.");

                if (string.IsNullOrWhiteSpace(clinic.Name))
                    return new OperationResult(false, "Nome da clínica é obrigatório.");

                if (string.IsNullOrWhiteSpace(clinic.CNPJ))
                    return new OperationResult(false, "CNPJ da clínica é obrigatório.");

                clinic.CNPJ = NormalizeUtils.NormalizeDigits(clinic.CNPJ);
                clinic.Phone = NormalizeUtils.NormalizeDigits(clinic.Phone);

                if (clinic.ClinicId == 0)
                {
                    await _clinicRepository.AddAsync(clinic);

                    var clinicSettings = new ClinicSettings()
                    {
                        ClinicId = clinic.ClinicId
                    };

                    await _clinicSettingsRepository.AddAsync(clinicSettings);
                }
                else
                {
                    var existing = await _clinicRepository.GetByAsync(p => p.ClinicId == clinic.ClinicId);

                    if (existing == null)
                        return new OperationResult(false, "Clínica não encontrada.");

                    existing.Name = clinic.Name;
                    existing.Phone = clinic.Phone;
                    existing.Acess = clinic.Acess;
                    existing.CNPJ = clinic.CNPJ;
                    existing.Address = clinic.Address;

                    await _clinicRepository.UpdateAsync(existing);
                }
                return new OperationResult(true, string.Empty);
            }
            catch (Exception ex)
            {
                return new OperationResult(
                    false,
                    "Ocorreu um erro inesperado ao salvar a clínica. Tente novamente mais tarde.");
            }
        }

        public async Task<OperationResult> DeleteClinicAsync(int clinicId)
        {
            try
            {
                var clinic = await _clinicRepository.GetByAsync(c => c.ClinicId == clinicId);

                if (clinic == null)
                    return new OperationResult(false, "Clínica não encontrada.");

                await _clinicRepository.DeleteAsync(clinic);

                return new OperationResult(true);
            }
            catch (Exception ex)
            {
                return new OperationResult(
                    false,
                    "Ocorreu um erro inesperado ao excluir a clínica. Tente novamente mais tarde.");
            }
        }

        public async Task<OperationResult> SaveClinicSettingsAsync(ClinicSettings clinicSettings)
        {
            try
            {
                if (clinicSettings == null)
                    return new OperationResult(false, "Dados da clínica inválidos.");

                var existing = await _clinicSettingsRepository.GetByAsync(p => p.ClinicSettingsId == clinicSettings.ClinicSettingsId);

                if (existing == null)
                    return new OperationResult(false, "Configurações não encontradas.");

                existing.UseConfirmationLink = clinicSettings.UseConfirmationLink;
                existing.WhatsAppAppointmentConfirmationMessage = clinicSettings.WhatsAppAppointmentConfirmationMessage;

                await _clinicSettingsRepository.UpdateAsync(existing);
                return new OperationResult(true, string.Empty);

            }
            catch (Exception ex)
            {
                return new OperationResult(
                    false,
                    "Ocorreu um erro inesperado ao salvar as configurações da clínica. Tente novamente mais tarde.");
            }
        }

        public async Task<ClinicSettings?> GetClinicSettingsByClinicIdAsync(int clinicId)
        {
            return await _clinicSettingsRepository.GetByAsNoTrackingAsync(d => d.ClinicId == clinicId);
        }

    }
}
