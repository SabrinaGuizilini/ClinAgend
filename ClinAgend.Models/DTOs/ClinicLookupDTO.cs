
namespace ClinAgend.Models.DTOs
{
    public record ClinicLookupDTO(int ClinicId, string Name, string CNPJ)
    {
        public string FormatedCNPJ
        => string.IsNullOrWhiteSpace(CNPJ)
            ? string.Empty
            : Convert.ToUInt64(CNPJ).ToString(@"00\.000\.000\/0000\-00");
    }

}
