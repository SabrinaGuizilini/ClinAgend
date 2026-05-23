using ClinAgend.Core.Exceptions;
using ClinAgend.Core.Interfaces;
using ClinAgend.Data.Interfaces;
using ClinAgend.Models.DTOs;
using ClinAgend.Models.Enums;
using ClinAgend.Models.Models;
using ClinAgend.Models.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ClinAgend.Core.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IGenericRepository<Appointment> _appointmentRepository;
        private readonly IScheduleValidatorService _scheduleValidatorService;
        private readonly AppSettings _appSettings;

        public AppointmentService(
            IGenericRepository<Appointment> appointmentRepository,
            IScheduleValidatorService scheduleValidatorService,
            IOptions<AppSettings> appSettings
            )
        {
            _appointmentRepository = appointmentRepository;
            _scheduleValidatorService = scheduleValidatorService;
            _appSettings = appSettings.Value;
        }

        public async Task<OperationResult> SaveAppointmentAsync(Appointment appointment)
        {
            try
            {
                if (appointment == null)
                    return new OperationResult(false, "Dados do agendamento inválidos.");

                if (appointment.StartTime == default || appointment.EndTime == default)
                    return new OperationResult(false, "Data e hora do agendamento são obrigatórias.");

                if (appointment.EndTime <= appointment.StartTime)
                    return new OperationResult(false, "A data final deve ser maior que a data inicial.");

                if (appointment.PatientId <= 0)
                    return new OperationResult(false, "Paciente inválido.");

                if (appointment.DoctorId <= 0)
                    return new OperationResult(false, "Médico inválido.");

                if (appointment.ClinicId <= 0)
                    return new OperationResult(false, "Clínica inválida.");

                await _scheduleValidatorService.ValidateAppointmentTimeAsync(appointment);

                if (appointment.AppointmentId == 0)
                {
                    await _appointmentRepository.AddAsync(appointment);
                }
                else
                {
                    var existing = await _appointmentRepository.GetByAsync(p => p.AppointmentId == appointment.AppointmentId);
                    if (existing == null)
                        return new OperationResult(false, "Agendamento não encontrado");

                    existing.StartTime = appointment.StartTime;
                    existing.EndTime = appointment.EndTime;
                    existing.Observations = appointment.Observations;
                    existing.PatientId = appointment.PatientId;
                    existing.AppointmentPresenceStatus = appointment.AppointmentPresenceStatus;
                    existing.AppointmentStatus = appointment.AppointmentStatus;
                    existing.AppointmentType = appointment.AppointmentType;
                    existing.BillingType = appointment.BillingType;
                    existing.HealthInsuranceName = appointment.HealthInsuranceName;

                    await _appointmentRepository.UpdateAsync(existing);
                }

                return new OperationResult(true, string.Empty);
            }
            catch (DomainValidationException ex)
            {
                return new OperationResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                return new OperationResult(
                  false,
                  "Ocorreu um erro inesperado ao salvar o agendamento. Tente novamente mais tarde.");
            }
        }

        public async Task<Appointment> GetAppointmentById(int appointmentId)
        {
            return await _appointmentRepository.GetByAsNoTrackingAsync(
                a => a.AppointmentId == appointmentId,
                a => a.Patient
             );
        }

        public async Task<bool> PatientHasAppointmentsAsync(int patientId)
        {
            return await _appointmentRepository
            .AnyAsync(a => a.PatientId == patientId);
        }

        public async Task<bool> DoctorHasAppointmentsAsync(int doctorId)
        {
            return await _appointmentRepository
            .AnyAsync(a => a.DoctorId == doctorId);
        }

        public async Task<List<Appointment>> GetDoctorAppointmentsByDateRangeAsync(int doctorId, DateTime start, DateTime end, CalendarFilters filter)
        {
            if (end < start)
                throw new ArgumentException("A data final não pode ser menor que a data inicial.");

            var query = _appointmentRepository
              .Query()
              .AsNoTracking()
              .Include(a => a.Patient)
              .Where(a => a.DoctorId == doctorId && a.StartTime >= start && a.EndTime <= end);

            if (filter is not null)
            {
                if (filter.OnlyCanceledAppointment)
                    query = query.Where(a => a.AppointmentStatus == AppointmentStatus.Cancelado);
                else
                {
                    if (filter.AppointmentStatus.Any())
                        query = query.Where(a => filter.AppointmentStatus.Contains(a.AppointmentStatus));
                    else
                        query = query.Where(a => a.AppointmentStatus != AppointmentStatus.Cancelado);
                }

                if (filter.AppointmentType.Any())
                    query = query.Where(a => filter.AppointmentType.Contains(a.AppointmentType));

                if (filter.BillingType.Any())
                    query = query.Where(a => filter.BillingType.Contains(a.BillingType));

                if (filter.AppointmentPresenceStatus.Any())
                    query = query.Where(a => filter.AppointmentPresenceStatus.Contains(a.AppointmentPresenceStatus));

                if (filter.Patient != null)
                    query = query.Where(a => a.PatientId == filter.Patient.PatientId);
            }

            return await query.ToListAsync();
        }

        public async Task<List<Appointment>> GetClinicAppointmentsByDateRangeAsync(int clinicId, DateTime start, DateTime end)
        {
            if (end < start)
                throw new ArgumentException("A data final não pode ser menor que a data inicial.");

            return await _appointmentRepository.GetManyAsNoTrackingByAsync(a => a.ClinicId == clinicId && a.StartTime >= start && a.EndTime <= end,
                a => a.Patient, a => a.Doctor);
        }

        public async Task<OperationResult> DeleteAppointmentAsync(int appointmentId)
        {
            try
            {
                var appointment = await _appointmentRepository.GetByAsync(p => p.AppointmentId == appointmentId);
                if (appointment == null)
                    return new OperationResult(false, "Agendamento não encontrado");

                await _appointmentRepository.DeleteAsync(appointment);
                return new OperationResult(true, string.Empty);
            }
            catch (Exception ex)
            {
                return new OperationResult(
                    false,
                    "Ocorreu um erro inesperado ao excluir o agendamento. Tente novamente mais tarde.");
            }
        }

        public async Task<string> GenerateConfirmationLinkAsync(int appointmentId)
        {
            try
            {
                var appointment = await _appointmentRepository.GetByAsync(p => p.AppointmentId == appointmentId);
                if (appointment == null)
                    throw new Exception("Agendamento não encontrado.");

                appointment.ConfirmationToken = Guid.NewGuid().ToString();
                appointment.IsConfirmationLinkUsed = false;
                await _appointmentRepository.UpdateAsync(appointment);

                return $"{_appSettings.BaseUrl}/confirmar-agendamento/{appointment.ConfirmationToken}";
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public async Task<Appointment?> GetAppointmentByConfirmationToken(string confirmationToken)
        {
            return await _appointmentRepository.GetByAsNoTrackingAsync(a => a.ConfirmationToken == confirmationToken, a => a.Doctor);
        }

        public async Task ConfirmOrCancelAppointment(int appointmentId, AppointmentStatus status)
        {
            var appointment = await _appointmentRepository.GetByAsync(p => p.AppointmentId == appointmentId);
            if (appointment == null)
                throw new Exception("Agendamento não encontrado.");

            appointment.AppointmentStatus = status;
            appointment.IsConfirmationLinkUsed = true;
            await _appointmentRepository.UpdateAsync(appointment);
        }

    }
}
