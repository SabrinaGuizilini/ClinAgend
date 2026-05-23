using ClinAgend.Models.Validations;
using System.ComponentModel.DataAnnotations;

namespace ClinAgend.Models.DTOs
{
    public class ScheduleBlockDTO
    {
        public int ScheduleBlockId { get; set; }

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

        [StringLength(200, ErrorMessage = "A descrição não pode exceder 200 caracteres.")]
        public string? Description { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public int ClinicId { get; set; }
    }
}
