using Heron.MudCalendar;

namespace ClinAgend.Web.Models
{
    internal class AppointmentCalendarItem : CalendarItem
    {
        public int AppointmentId { get; set; }
        public string Color { get; set; }
        public bool isScheduleBlock { get; set; }
        public bool isInterval { get; set; }
    }
}
