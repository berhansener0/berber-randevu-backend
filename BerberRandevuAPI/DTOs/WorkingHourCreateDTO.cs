namespace BerberRandevuAPI.DTOs
{
    public class WorkingHourCreateDTO
    {
        public int BarberId { get; set; }
        public int DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
