using ClinAgend.Models.DTOs;
using ClinAgend.Models.Enums;
using ClinAgend.Models.Models;

namespace ClinAgend.Core.Interfaces
{
    public interface IAppointmentService
    {
        Task<OperationResult> SaveAppointmentAsync(Appointment appointment);
        Task<List<Appointment>> GetDoctorAppointmentsByDateRangeAsync(int doctorId, DateTime start, DateTime end, CalendarFilters filter);
        Task<Appointment> GetAppointmentById(int appointmentId);
        Task<OperationResult> DeleteAppointmentAsync(int appointmentId);
        Task<string> GenerateConfirmationLinkAsync(int appointmentId);
        Task<Appointment?> GetAppointmentByConfirmationToken(string confirmationToken);
        Task ConfirmOrCancelAppointment(int appointmentId, AppointmentStatus status);
        Task<bool> PatientHasAppointmentsAsync(int patientId);
        Task<bool> DoctorHasAppointmentsAsync(int doctorId);
        Task<List<Appointment>> GetClinicAppointmentsByDateRangeAsync(int clinicId, DateTime start, DateTime end);
    }
}
