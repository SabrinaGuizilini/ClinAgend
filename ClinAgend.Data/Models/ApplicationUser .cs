using ClinAgend.Models.Models;
using ClinAgend.Models.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ClinAgend.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        public int? ClinicId { get; set; }
        public virtual Clinic? Clinic { get; set; }
        public UserType Type { get; set; }
    }
}
