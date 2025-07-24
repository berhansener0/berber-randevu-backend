﻿namespace BerberRandevuAPI.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public string Phone { get; set; }
        public string PasswordHash { get; set; }

        public DateTime CreatedAt { get; set; }
       
        public ICollection<Appointment> Appointments { get; set; }
    }
}
