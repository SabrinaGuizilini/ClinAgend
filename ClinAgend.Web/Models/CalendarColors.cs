using ClinAgend.Models.Enums;

namespace ClinAgend.Web.Models
{
    internal static class CalendarColors
    {
        internal const string Booked = "#99D0F0";
        internal const string Confirmed = "#2DD4BF";
        internal const string Cancelled = "#E57373";
        internal const string Presence = "#1EBE8F";
        internal const string Absent = "#FFB74D";
        internal const string Blocked = "#8A90A0";

        internal static string GetAppointmentColor(bool blocked, AppointmentStatus appointment = AppointmentStatus.Agendado, AppointmentPresenceStatus presence = AppointmentPresenceStatus.Indefinido)
        {
            if (blocked)
                return CalendarColors.Blocked;

            return appointment switch
            {
                AppointmentStatus.Cancelado => CalendarColors.Cancelled,
                AppointmentStatus.Confirmado => presence switch
                {
                    AppointmentPresenceStatus.Presente => CalendarColors.Presence,
                    AppointmentPresenceStatus.Ausente => CalendarColors.Absent,
                    _ => CalendarColors.Confirmed
                },
                AppointmentStatus.Agendado => presence switch
                {
                    AppointmentPresenceStatus.Presente => CalendarColors.Presence,
                    AppointmentPresenceStatus.Ausente => CalendarColors.Absent,
                    _ => CalendarColors.Booked
                },
                _ => CalendarColors.Booked
            };

        }

    }

}
