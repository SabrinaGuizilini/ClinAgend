using ClinAgend.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ClinAgend.Models.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [MaxLength(200)]
        public string? Observations { get; set; }

        [MaxLength(100)]
        public string? HealthInsuranceName { get; set; }
        public BillingType BillingType { get; set; }
        public AppointmentType AppointmentType { get; set; }
        public AppointmentStatus AppointmentStatus { get; set; }
        public AppointmentPresenceStatus AppointmentPresenceStatus { get; set; }
        public string? ConfirmationToken { get; set; }
        public bool IsConfirmationLinkUsed { get; set; }
        public int PatientId { get; set; }
        public virtual Patient Patient { get; set; }
        public int DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; }
        public int ClinicId { get; set; }
        public virtual Clinic Clinic { get; set; }
    }
}
