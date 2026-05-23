using System.ComponentModel.DataAnnotations;

namespace ClinAgend.Models.Models
{
    public class Clinic
    {
        [Key]
        public int ClinicId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string Acess { get; set; }

        [Required]
        [MaxLength(18)]
        public string CNPJ { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }
        public ClinicSettings? Settings { get; set; }
    }
}
