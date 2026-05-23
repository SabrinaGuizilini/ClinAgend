
namespace ClinAgend.Models.DTOs
{
    public class AgendSummary
    {
        public int Total { get; set; }
        public int Confirmed { get; set; }
        public int Canceled { get; set; }
        public int Pending { get; set; }
    }
}
