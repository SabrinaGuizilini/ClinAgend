
namespace ClinAgend.Models.DTOs
{
    public class PatientLookupDTO
    {
        public int PatientId { get; set; }
        public string FullName { get; set; }
        public string CPF { get; set; }
        public string PhoneNumber { get; set; }
    }
}
