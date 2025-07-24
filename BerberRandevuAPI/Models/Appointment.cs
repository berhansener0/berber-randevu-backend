using System;
namespace BerberRandevuAPI.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public int UserId { get; set; }
        public int BarberId { get; set; }
        public DateTime AppointmentDate { get; set; }

        public TimeSpan StartTime { get; set; }
        public string Status { get; set; } = "beklemede";

        public User? User { get; set; }
        public Barber? Barber { get; set; }

    }
}
