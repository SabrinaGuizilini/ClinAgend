
namespace ClinAgend.Models.Enums
{
   public enum UserType
   {
       Default = 0,
       Master = 1,
       Doctor = 2,
       SuperAdmin = 3
   }

   public enum BillingType
   {
       Particular = 0,
       Convenio = 1,
       SUS = 2
   }

    public enum AppointmentType
    {
        Consulta = 0,
        Retorno = 1,
        Procedimento = 2,
        Exame = 3,
        Cirurgia = 4,
        Vacina = 5
    }

    public enum AppointmentStatus
    {
        Agendado,
        Confirmado,
        Cancelado
    }

    public enum AppointmentPresenceStatus
    {
        Indefinido,
        Presente,
        Ausente
    }

    public enum EntityStatusFilter
    {
        Active,
        Inactive,
        All
    }

    public enum AgendMode
    {
        Full,
        ReadOnly
    }

}
