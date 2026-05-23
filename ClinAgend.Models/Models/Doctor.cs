using System.ComponentModel.DataAnnotations;

namespace ClinAgend.Models.Models
{
    public class Doctor
    {
        [Key]
        public int DoctorId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(11)]
        public string CPF { get; set; }

        [Required]
        [MaxLength(10)]
        public string CRM { get; set; }

        [Required]
        [MaxLength(100)]
        public string Specialty { get; set; }

        [Required]
        [MaxLength(100)]
        public string WorkingDays { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        public int? BreakBetweenAppointments { get; set; }

        public int? MaxAppointmentsPerDay { get; set; }

        [Required]
        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }
        public int ClinicId { get; set; }
        public virtual Clinic Clinic { get; set; }

        [MaxLength(450)]
        public string? UserId { get; set; }
    }
}
