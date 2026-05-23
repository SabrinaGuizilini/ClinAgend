using ClinAgend.Models.Enums;

namespace ClinAgend.Web.Helpers
{
    internal static class UserTypeExtensions
    {
        internal static string ToDisplayText(this UserType type)
            => type switch
            {
                UserType.Default => "Padrão",
                UserType.Master => "Master",
                UserType.Doctor => "Médico",
                UserType.SuperAdmin => "Administrador",
                _ => "Desconhecido"
            };
    }

}
