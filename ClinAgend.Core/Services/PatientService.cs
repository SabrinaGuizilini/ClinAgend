using ClinAgend.Core.Interfaces;
using ClinAgend.Core.Utils;
using ClinAgend.Data.Interfaces;
using ClinAgend.Models.DTOs;
using ClinAgend.Models.Enums;
using ClinAgend.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinAgend.Core.Services
{
    public class PatientService : IPatientService
    {
        private readonly IGenericRepository<Patient> _patientRepository;
        private readonly IGenericRepository<Appointment> _appointmentRepository;

        public PatientService(IGenericRepository<Patient> patientRepository, IGenericRepository<Appointment> appointmentRepository)
        {
            _patientRepository = patientRepository;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<Patient?> GetByIdAsync(int patientId)
        {
            return await _patientRepository.GetByAsNoTrackingAsync(d => d.PatientId == patientId);
        }

        public async Task<(List<Patient> Patients, int TotalCount)> GetPaginatedAsync(
            int clinicId, int page, int pageSize, EntityStatusFilter statusFilter = EntityStatusFilter.Active, string? search = null, string? sortBy = null, bool descending = false)
        {
            var query = _patientRepository.Query()
                        .Where(p => p.ClinicId == clinicId).AsNoTracking();

            query = statusFilter switch
            {
                EntityStatusFilter.Active => query.Where(p => p.IsActive),
                EntityStatusFilter.Inactive => query.Where(p => !p.IsActive),
                _ => query
            };

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p =>
                    p.FullName.Contains(search) ||
                    p.CPF.Contains(search) ||
                    p.PhoneNumber.Contains(search));
            }

            var total = await query.CountAsync();

            query = sortBy switch
            {
                nameof(Patient.FullName) => descending ? query.OrderByDescending(p => p.FullName) : query.OrderBy(p => p.FullName),
                nameof(Patient.PhoneNumber) => descending ? query.OrderByDescending(p => p.PhoneNumber) : query.OrderBy(p => p.PhoneNumber),
                nameof(Patient.CPF) => descending ? query.OrderByDescending(p => p.CPF) : query.OrderBy(p => p.CPF),
                _ => query.OrderBy(p => p.FullName)
            };

            var patients = await query
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (patients, total);
        }

        public async Task<OperationResult> SavePatientAsync(Patient patient)
        {
            try
            {
                if (patient == null)
                    return new OperationResult(false, "Dados do paciente inválidos.");

                if (string.IsNullOrWhiteSpace(patient.FullName))
                    return new OperationResult(false, "Nome do paciente é obrigatório.");

                if (string.IsNullOrWhiteSpace(patient.CPF))
                    return new OperationResult(false, "CPF do paciente é obrigatório.");

                patient.CPF = NormalizeUtils.NormalizeDigits(patient.CPF);
                patient.PhoneNumber = NormalizeUtils.NormalizeDigits(patient.PhoneNumber);

                var duplicatedCPF = await _patientRepository
                    .AnyAsync(p => p.CPF == patient.CPF && p.PatientId != patient.PatientId);
                if (duplicatedCPF)
                    return new OperationResult(false, "Já existe um paciente cadastrado com este CPF.");

                if (patient.PatientId == 0)
                {
                    await _patientRepository.AddAsync(patient);
                }
                else
                {
                    var existing = await _patientRepository.GetByAsync(p => p.PatientId == patient.PatientId);
                    if (existing == null)
                        return new OperationResult(false, "Paciente não encontrado");

                    existing.FullName = patient.FullName;
                    existing.PhoneNumber = patient.PhoneNumber;
                    existing.CPF = patient.CPF;
                    existing.BirthDate = patient.BirthDate;
                    existing.Address = patient.Address;
                    existing.Sex = patient.Sex;
                    existing.IsActive = patient.IsActive;

                    await _patientRepository.UpdateAsync(existing);
                }

                return new OperationResult(true, string.Empty);
            }
            catch (Exception ex)
            {
                return new OperationResult(
                  false,
                  "Ocorreu um erro inesperado ao salvar o paciente. Tente novamente mais tarde.");
            }

        }

        public async Task<OperationResult> DeletePatientAsync(int patientId)
        {
            try
            {
                var patient = await _patientRepository.GetByAsync(p => p.PatientId == patientId);
                if (patient == null)
                    return new OperationResult(false, "Paciente não encontrado");

                var hasAppointments = await _appointmentRepository.AnyAsync(a => a.PatientId == patientId);

                if (hasAppointments)
                    return new OperationResult(false, "Não é possível excluir o paciente, pois existem agendamentos vinculados.");

                await _patientRepository.DeleteAsync(patient);
                return new OperationResult(true, string.Empty);
            }
            catch (Exception ex)
            {
                return new OperationResult(
                   false,
                   "Ocorreu um erro inesperado ao excluir o paciente. Tente novamente mais tarde.");
            }
        }

        public async Task<IEnumerable<PatientLookupDTO>> SearchByNameAsync(string? name, CancellationToken cancellationToken = default)
        {
            name = name?.Trim();

            var query = _patientRepository.Query()
                .AsNoTracking()
                .Where(d => d.IsActive);

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(d =>
                    EF.Functions.ILike(d.FullName, $"{name}%"));
            }

            return await query
                .OrderBy(d => d.FullName)
                .Take(10)
                .Select(d => new PatientLookupDTO
                {
                    PatientId = d.PatientId,
                    FullName = d.FullName,
                    CPF = d.CPF
                })
                .ToListAsync(cancellationToken);
        }
    }
}
