using ClinAgend.Models.Enums;
using ClinAgend.Models.Validations;
using System.ComponentModel.DataAnnotations;

namespace ClinAgend.Models.DTOs
{
    public class AppointmentDTO
    {
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "A data de início é obrigatória.")]
        [RegularExpression(@"^([0-2]\d|3[0-1])\/(0[1-9]|1[0-2])\/\d{4}$", ErrorMessage = "Use o formato dd/MM/yyyy.")]
        public string StartDate { get; set; }

        [Required(ErrorMessage = "A data de término é obrigatória.")]
        [RegularExpression(@"^([0-2]\d|3[0-1])\/(0[1-9]|1[0-2])\/\d{4}$", ErrorMessage = "Use o formato dd/MM/yyyy.")]
        [EndDateAfterStartDate("StartDate")]
        public string EndDate { get; set; }

        [Required(ErrorMessage = "O horário de início é obrigatório.")]
        [RegularExpression(@"^([01]\d|2[0-3]):([0-5]\d)$", ErrorMessage = "Use o formato HH:mm.")]
        public string StartTime { get; set; }

        [Required(ErrorMessage = "O horário de término é obrigatório.")]
        [RegularExpression(@"^([01]\d|2[0-3]):([0-5]\d)$", ErrorMessage = "Use o formato HH:mm.")]
        [EndTimeAfterStartTime("StartTime")]
        public string EndTime { get; set; }

        [StringLength(200, ErrorMessage = "As observações não podem exceder 200 caracteres.")]
        public string? Observations { get; set; }

        [StringLength(100, ErrorMessage = "O nome do convênio não pode exceder 100 caracteres.")]
        public string? HealthInsuranceName { get; set; }

        [Required(ErrorMessage = "O convênio é obrigatório.")]
        public BillingType BillingType { get; set; }

        [Required(ErrorMessage = "O tipo de consulta é obrigatório.")]
        public AppointmentType AppointmentType { get; set; }

        [Required(ErrorMessage = "O status da consulta é obrigatório.")]
        public AppointmentStatus AppointmentStatus { get; set; }

        [Required(ErrorMessage = "O status de presença da consulta é obrigatório.")]
        public AppointmentPresenceStatus AppointmentPresenceStatus { get; set; }

        [Required(ErrorMessage = "O paciente é obrigatório.")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "O paciente é obrigatório.")]
        public PatientLookupDTO Patient {  get; set; }

        [Required(ErrorMessage = "O médico é obrigatório.")]
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public int ClinicId { get; set; }
    }
}
