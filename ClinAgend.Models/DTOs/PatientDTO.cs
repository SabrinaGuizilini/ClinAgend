using System.ComponentModel.DataAnnotations;
using ClinAgend.Models.Validations;

namespace ClinAgend.Models.DTOs
{
    public class PatientDTO
    {
        public int PatientId { get; set; }

        [Required(ErrorMessage = "O nome completo é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome completo não pode exceder 100 caracteres.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [Cpf(ErrorMessage = "CPF inválido.")]
        public string CPF { get; set; }

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        [RegularExpression(@"^\d{2}/\d{2}/\d{4}$", ErrorMessage = "Formato de data inválido. Use dd/MM/yyyy.")]
        [BirthDateString(minAge: 0, maxAge: 120, ErrorMessage = "A idade deve estar entre 0 e 120 anos.")]
        public string BirthDate { get; set; }

        [Required(ErrorMessage = "O sexo é obrigatório.")]
        [StringLength(1, ErrorMessage = "O campo sexo não pode exceder 1 caractere.")]
        public string Sex { get; set; }

        [Required(ErrorMessage = "O telefone é obrigatório.")]
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }

        [StringLength(200, ErrorMessage = "O endereço não pode exceder 200 caracteres.")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "A clínica é obrigatória.")]
        public int ClinicId { get; set; }
    }
}
