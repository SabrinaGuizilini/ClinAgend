using ClinAgend.Core.Interfaces;
using ClinAgend.Core.Utils;
using ClinAgend.Data.Interfaces;
using ClinAgend.Models.DTOs;
using ClinAgend.Models.Enums;
using ClinAgend.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinAgend.Core.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IGenericRepository<Doctor> _doctorRepository;
        private readonly IGenericRepository<Appointment> _appointmentRepository;

        public DoctorService(IGenericRepository<Doctor> doctorRepository, IGenericRepository<Appointment> appointmentRepository)
        {
            _doctorRepository = doctorRepository;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<Doctor?> GetByIdAsync(int doctorId)
        {
            return await _doctorRepository.GetByAsNoTrackingAsync(d => d.DoctorId == doctorId);
        }

        public async Task<(List<Doctor> Doctors, int TotalCount)> GetPaginatedAsync(
           int clinicId, int page, int pageSize, EntityStatusFilter statusFilter = EntityStatusFilter.Active, string? search = null, string? sortBy = null, bool descending = false)
        {
            var query = _doctorRepository.Query()
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
                    p.CRM.Contains(search) ||
                    p.Specialty.Contains(search));
            }

            var total = await query.CountAsync();

            query = sortBy switch
            {
                nameof(Doctor.FullName) => descending ? query.OrderByDescending(p => p.FullName) : query.OrderBy(p => p.FullName),
                nameof(Doctor.CRM) => descending ? query.OrderByDescending(p => p.CRM) : query.OrderBy(p => p.CRM),
                nameof(Doctor.Specialty) => descending ? query.OrderByDescending(p => p.Specialty) : query.OrderBy(p => p.Specialty),
                _ => query.OrderBy(p => p.FullName)
            };

            var doctors = await query
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (doctors, total);
        }

        public async Task<OperationResult> SaveDoctorAsync(Doctor doctor)
        {
            try
            {
                if (doctor == null)
                    return new OperationResult(false, "Dados do médico inválidos.");

                if (string.IsNullOrWhiteSpace(doctor.FullName))
                    return new OperationResult(false, "Nome do médico é obrigatório.");

                if (string.IsNullOrWhiteSpace(doctor.CRM))
                    return new OperationResult(false, "CRM do médico é obrigatório.");

                if (string.IsNullOrWhiteSpace(doctor.CPF))
                    return new OperationResult(false, "CPF do médico é obrigatório.");

                doctor.CPF = NormalizeUtils.NormalizeDigits(doctor.CPF);
                doctor.PhoneNumber = NormalizeUtils.NormalizeDigits(doctor.PhoneNumber);

                var duplicatedCPF = await _doctorRepository
                  .AnyAsync(d => d.CPF == doctor.CPF && d.DoctorId != doctor.DoctorId);
                if (duplicatedCPF)
                    return new OperationResult(false, "Já existe um médico cadastrado com este CPF.");

                var duplicatedCRM = await _doctorRepository
                  .AnyAsync(d => d.CRM == doctor.CRM && d.DoctorId != doctor.DoctorId);
                if (duplicatedCRM)
                    return new OperationResult(false, "Já existe um médico cadastrado com este CRM.");

                if (doctor.DoctorId == 0)
                {
                    await _doctorRepository.AddAsync(doctor);
                }
                else
                {
                    var existing = await _doctorRepository.GetByAsync(p => p.DoctorId == doctor.DoctorId);
                    if (existing == null)
                        return new OperationResult(false, "Médico não encontrado");

                    if (existing.UserId is not null && !doctor.IsActive)
                        return new OperationResult(false, "Não é possível inativar o médico, pois existe um usuário vinculado à ele. Exclua o usuário para depois inativar o médico.");

                    existing.FullName = doctor.FullName;
                    existing.PhoneNumber = doctor.PhoneNumber;
                    existing.CPF = doctor.CPF;
                    existing.Address = doctor.Address;
                    existing.CRM = doctor.CRM;
                    existing.Specialty = doctor.Specialty;
                    existing.WorkingDays = doctor.WorkingDays;
                    existing.StartTime = doctor.StartTime;
                    existing.EndTime = doctor.EndTime;
                    existing.BreakBetweenAppointments = doctor.BreakBetweenAppointments;
                    existing.MaxAppointmentsPerDay = doctor.MaxAppointmentsPerDay;
                    existing.IsActive = doctor.IsActive;

                    await _doctorRepository.UpdateAsync(existing);
                }

                return new OperationResult(true, string.Empty);
            }
            catch (Exception ex)
            {
                return new OperationResult(
                  false,
                  "Ocorreu um erro inesperado ao salvar o médico. Tente novamente mais tarde.");
            }
        }

        public async Task<OperationResult> DeleteDoctorAsync(int doctorId)
        {
            try
            {
                var doctor = await _doctorRepository.GetByAsync(p => p.DoctorId == doctorId);
                if (doctor == null)
                    return new OperationResult(false, "Médico não encontrado");

                var hasAppointments = await _appointmentRepository.AnyAsync(a => a.DoctorId == doctorId);

                if (hasAppointments)
                    return new OperationResult(false, "Não é possível excluir o médico, pois existem agendamentos vinculados.");

                if (doctor.UserId is not null)
                    return new OperationResult(false, "Não é possível excluir o médico, pois existe um usuário vinculado à ele. Exclua primeiro o usuário e depois o médico.");

                await _doctorRepository.DeleteAsync(doctor);
                return new OperationResult(true, string.Empty);
            }
            catch (Exception ex)
            {
                return new OperationResult(
                    false,
                    "Ocorreu um erro inesperado ao excluir o médico. Tente novamente mais tarde.");
            }
        }

        public async Task<IEnumerable<DoctorLookupDTO>> SearchByNameAsync(string? name, CancellationToken cancellationToken = default)
        {
            name = name?.Trim();

            var query = _doctorRepository.Query()
                .AsNoTracking()
                .Where(d => d.IsActive && d.UserId == null);

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(d =>
                   EF.Functions.ILike(d.FullName, $"{name}%"));
            }

            return await query
                .OrderBy(d => d.FullName)
                .Take(10)
                .Select(d => new DoctorLookupDTO
                {
                    DoctorId = d.DoctorId,
                    FullName = d.FullName,
                    CRM = d.CRM
                })
                .ToListAsync(cancellationToken);
        }

        public async Task LinkUserAsync(int doctorId, string? userId)
        {
            var doctor = await _doctorRepository.GetByAsync(d => d.DoctorId == doctorId)
                ?? throw new Exception("Médico não encontrado");

            if (doctor.UserId != null)
                throw new Exception("Médico já possui usuário vinculado");

            doctor.UserId = userId;

            await _doctorRepository.UpdateAsync(doctor);
        }

        public async Task<Doctor?> GetDoctorByUserId(string userId)
        {
            return await _doctorRepository.GetByAsNoTrackingAsync(d => d.UserId == userId);
        }

    }
}
