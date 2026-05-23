using ClinAgend.Models.Models;

namespace ClinAgend.Core.Interfaces
{
    public interface IScheduleValidatorService
    {
        Task ValidateAppointmentTimeAsync(Appointment appointment);
        Task ValidateScheduleBlockTimeAsync(ScheduleBlock scheduleBlock);
    }
}
