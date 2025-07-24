using System;
namespace BerberRandevuAPI.Models
{
    public class WorkingHour
    {
        public int Id { get; set; }
        public int BarberId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }    
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public Barber Barber { get; set; }
    }
}
