using System;
using System.Collections.Generic;

namespace BerberRandevuAPI.Models
{
    public class Barber
    {
        public int BarberId { get; set; }
        public string Name { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public string Phone { get; set; }
        public string Address { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<Appointment>Appointments { get; set; }
        public ICollection<WorkingHour>WorkingHours { get; set; }

    }
}
