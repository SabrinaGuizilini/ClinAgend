using System.ComponentModel.DataAnnotations;

namespace ClinAgend.Models.DTOs;

public class ClinicSettingsDTO
{
    public int ClinicSettingsId { get; set; }

    public int ClinicId { get; set; }

    [StringLength(2000,
        ErrorMessage = "A mensagem não pode exceder 2000 caracteres.")]
    public string? WhatsAppAppointmentConfirmationMessage { get; set; }

    [Required(ErrorMessage = "Informe se utilizará link de confirmação.")]
    public bool UseConfirmationLink { get; set; }
}