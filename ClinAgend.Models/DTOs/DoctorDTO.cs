using ClinAgend.Models.Validations;
using System.ComponentModel.DataAnnotations;

namespace ClinAgend.Models.DTOs
{
    public class DoctorDTO
    {
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "O nome completo é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome completo não pode exceder 100 caracteres.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [Cpf(ErrorMessage = "CPF inválido.")]
        public string CPF { get; set; }

        [Required(ErrorMessage = "O CRM é obrigatório.")]
        [StringLength(10, ErrorMessage = "O CRM não pode exceder 10 caracteres.")]
        public string CRM { get; set; }

        [Required(ErrorMessage = "A especialidade é obrigatória.")]
        [StringLength(100, ErrorMessage = "A especialidade não pode exceder 100 caracteres.")]
        public string Specialty { get; set; }

        [Required(ErrorMessage = "Informe os dias de atendimento.")]
        public string WorkingDays { get; set; }

        [Range(0, 120)]
        public int? BreakBetweenAppointments { get; set; }

        [Range(1, 100)]
        public int? MaxAppointmentsPerDay { get; set; }

        [Required(ErrorMessage = "O horário de início é obrigatório.")]
        [RegularExpression(@"^([01]\d|2[0-3]):([0-5]\d)$", ErrorMessage = "Use o formato HH:mm.")]
        public string StartTime { get; set; }

        [Required(ErrorMessage = "O horário de término é obrigatório.")]
        [RegularExpression(@"^([01]\d|2[0-3]):([0-5]\d)$", ErrorMessage = "Use o formato HH:mm.")]
        [EndTimeAfterStartTime("StartTime")]
        public string EndTime { get; set; }

        [Required(ErrorMessage = "Informe o telefone.")]
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }

        [StringLength(200, ErrorMessage = "O endereço não pode exceder 200 caracteres.")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "A clínica é obrigatória.")]
        public int ClinicId { get; set; }
    }
}
