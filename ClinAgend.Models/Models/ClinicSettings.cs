using System.ComponentModel.DataAnnotations;

namespace ClinAgend.Models.Models;

public class ClinicSettings
{
    [Key]
    public int ClinicSettingsId { get; set; }

    public int ClinicId { get; set; }

    public virtual Clinic Clinic { get; set; }

    [MaxLength(2000)]
    public string? WhatsAppAppointmentConfirmationMessage { get; set; }

    [Required]
    public bool UseConfirmationLink { get; set; }
}
