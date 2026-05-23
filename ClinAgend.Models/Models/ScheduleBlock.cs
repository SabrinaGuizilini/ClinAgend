using System.ComponentModel.DataAnnotations;

namespace ClinAgend.Models.Models
{
    public class ScheduleBlock
    {
        [Key]
        public int ScheduleBlockId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        [Required]
        public int DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; }

        [Required]
        public int ClinicId { get; set; }
        public virtual Clinic Clinic { get; set; }
    }
}
