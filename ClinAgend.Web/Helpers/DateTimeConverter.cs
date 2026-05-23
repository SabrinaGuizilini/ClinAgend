using System.Globalization;

namespace ClinAgend.Web.Helpers
{
    internal static class DateTimeConverter
    {
        private const string DateFormat = "dd/MM/yyyy";
        private const string TimeFormat = "HH:mm";
        private static readonly CultureInfo Culture = new("pt-BR");

        internal static string ToDateString(DateTime dateTime)
        {
            return dateTime.ToString(DateFormat, Culture);
        }

        internal static string ToTimeString(DateTime dateTime)
        {
            return dateTime.ToString(TimeFormat, Culture);
        }

        internal static DateTime ParseDateTime(string? dateString, string? timeString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                throw new ArgumentException("A data não pode ser nula ou vazia.", nameof(dateString));

            if (string.IsNullOrWhiteSpace(timeString))
                timeString = "00:00"; // padrão se horário não informado

            var dateTimeString = $"{dateString} {timeString}";
            return DateTime.ParseExact(dateTimeString, $"{DateFormat} {TimeFormat}", Culture);
        }
    }
}
