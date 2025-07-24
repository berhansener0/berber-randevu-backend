using BerberRandevuAPI.DTOs;

namespace BerberRandevuAPI.Models.DTOs
{
    public class AppointmentReadDTO
    {
        public int AppointmentId { get; set; }
        public int UserId { get; set; }
        public int BarberId { get; set; }
        public string UserName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public string Status { get; set; }
        public UserDTO? User { get; set; }
        public BarberDTO? Barber { get; set; }
    }
}
