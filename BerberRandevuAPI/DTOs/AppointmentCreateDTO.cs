namespace BerberRandevuAPI.DTOs
{
    public class AppointmentCreateDTO
    {
        public int UserId { get; set; }
        public int BarberId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string StartTime { get; set; }
        public string? Status { get; set; }
    }
}
