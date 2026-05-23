namespace ClinAgend.Web.Helpers
{
    internal static class TimeSpanConverter
    {
        internal static TimeSpan ParseOrDefault(string? timeString)
        {
            return TimeSpan.TryParse(timeString, out var time)
                ? time
                : TimeSpan.Zero;
        }

        internal static string ToStringFormat(TimeSpan time)
        {
            return time.ToString(@"hh\:mm");
        }
    }
}
