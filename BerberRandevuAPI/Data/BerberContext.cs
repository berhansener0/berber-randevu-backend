using Microsoft.EntityFrameworkCore;
using BerberRandevuAPI.Models;

namespace BerberRandevuAPI.Data
{
    public class BerberContext : DbContext
    {
        public BerberContext(DbContextOptions<BerberContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Barber> Barbers { get; set; }
        public DbSet<WorkingHour> WorkingHours { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
                entity.Property(e => e.Phone).HasColumnName("phone");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            });

            modelBuilder.Entity<Barber>(entity =>
            {
                entity.ToTable("barbers");
                entity.HasKey(e => e.BarberId);
                entity.Property(e => e.BarberId).HasColumnName("barber_id");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
                entity.Property(e => e.Phone).HasColumnName("phone");
                entity.Property(e => e.Address).HasColumnName("address");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            });

            modelBuilder.Entity<WorkingHour>(entity =>
            {
                entity.ToTable("working_hours");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.BarberId).HasColumnName("barber_id");
                entity.Property(e => e.DayOfWeek).HasColumnName("day_of_week");
                entity.Property(e => e.StartTime).HasColumnName("start_time");
                entity.Property(e => e.EndTime).HasColumnName("end_time");

                entity.HasOne(e => e.Barber)
                      .WithMany(b => b.WorkingHours)
                      .HasForeignKey(e => e.BarberId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.BarberId, e.DayOfWeek }).IsUnique();
            });

            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.ToTable("appointments");
                entity.HasKey(e => e.AppointmentId);
                entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.BarberId).HasColumnName("barber_id");
                entity.Property(e => e.AppointmentDate).HasColumnName("appointment_date");

                entity.Property(e => e.StartTime)
                      .HasColumnName("start_time")
                      .HasColumnType("time");  

                entity.Property(e => e.Status).HasColumnName("status");

                entity.HasOne(e => e.User)
                      .WithMany(u => u.Appointments)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Barber)
                      .WithMany(b => b.Appointments)
                      .HasForeignKey(e => e.BarberId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.BarberId, e.AppointmentDate, e.StartTime }).IsUnique();
            });
        }
    }
}
