using ClinAgend.Models.Enums;

namespace ClinAgend.Models.DTOs
{
    public record UserDTO(string Id, string Name, string Email, UserType Type);
}
