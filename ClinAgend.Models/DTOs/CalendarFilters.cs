using ClinAgend.Models.Enums;

namespace ClinAgend.Models.DTOs
{
    public class CalendarFilters
    {
        public IEnumerable<string> AppointmentOrScheduleBlock { get; set; } = new HashSet<string>();
        public IEnumerable<AppointmentType> AppointmentType { get; set; } = new HashSet<AppointmentType>();
        public IEnumerable<AppointmentStatus> AppointmentStatus { get; set; } = new HashSet<AppointmentStatus>();
        public IEnumerable<AppointmentPresenceStatus> AppointmentPresenceStatus { get; set; } = new HashSet<AppointmentPresenceStatus>();
        public IEnumerable<BillingType> BillingType { get; set; } = new HashSet<BillingType>();
        public bool OnlyCanceledAppointment { get; set; }
        public PatientLookupDTO Patient { get; set; }
    }
}
