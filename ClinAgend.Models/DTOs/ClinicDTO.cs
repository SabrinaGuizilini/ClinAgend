using ClinAgend.Models.Validations;
using System.ComponentModel.DataAnnotations;

namespace ClinAgend.Models.DTOs
{
    public class ClinicDTO
    {
        public int ClinicId { get; set; }

        [Required(ErrorMessage = "O nome da clínica é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome da clínica não pode exceder 100 caracteres.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "O acesso da clínica é obrigatório.")]
        [StringLength(50, ErrorMessage = "O acesso da clínica não pode exceder 50 caracteres.")]
        public string Acess { get; set; }

        [Required(ErrorMessage = "O CNPJ da clínica é obrigatório.")]
        [Cnpj(ErrorMessage = "CNPJ inválido.")]
        public string CNPJ { get; set; }

        [StringLength(200, ErrorMessage = "O endereço da clínica não pode exceder 200 caracteres.")]
        public string? Address { get; set; }

        [StringLength(20, ErrorMessage = "O telefone da clínica não pode exceder 20 caracteres.")]
        public string? Phone { get; set; }
    }
}
