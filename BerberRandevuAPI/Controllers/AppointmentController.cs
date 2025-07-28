using BerberRandevuAPI.Controllers;
using BerberRandevuAPI.Data;
using BerberRandevuAPI.DTOs;
using BerberRandevuAPI.Models;
using BerberRandevuAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using System.Security.Claims;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using AutoMapper;

namespace BerberRandevuAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly BerberContext _context;
        private readonly IMapper _mapper;

        public AppointmentController(BerberContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentReadDTO>>> GetAll()
        {
            var appointments = await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Barber)
                .Select(a => new AppointmentReadDTO
                {
                    AppointmentId = a.AppointmentId,
                    UserId = a.UserId,
                    BarberId = a.BarberId,
                    AppointmentDate = a.AppointmentDate,
                    StartTime = a.StartTime,
                    Status = a.Status,
                    User = new UserDTO
                    {
                        UserId = a.User.UserId,
                        Name = a.User.Name
                    },
                    Barber = new BarberDTO
                    {
                        BarberId = a.Barber.BarberId,
                        Name = a.Barber.Name
                    }
                })
                .ToListAsync();

            return Ok(appointments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentReadDTO>> GetById(int id)
        {
            var appointment = await _context.Appointments
            .Include(a => a.User)
            .Include(a => a.Barber)
            .Where(a => a.AppointmentId == id)
            .Select(a => new AppointmentReadDTO
            {
                AppointmentId = a.AppointmentId,
                UserId = a.UserId,
                BarberId = a.BarberId,
                AppointmentDate = a.AppointmentDate,
                StartTime = a.StartTime,
                Status = a.Status,
                User = new UserDTO
                {
                    UserId = a.User.UserId,
                    Name = a.User.Name,
                },
                Barber = new BarberDTO
                {
                    BarberId = a.Barber.BarberId,
                    Name = a.Barber.Name,
                }
            })
            .SingleOrDefaultAsync();
            if (appointment == null)
            {
                return NotFound();
            }
            return Ok(appointment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, AppointmentUpdateDTO dto)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }
            appointment.Status = dto.Status;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "User")]
        [HttpGet("user/myappointments")]
        public async Task<ActionResult<IEnumerable<AppointmentReadDTO>>> GetUserAppointments()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userIdClaim == null)
                return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var appointments = await _context.Appointments
                .Where(a => a.UserId == userId)
                .Include(a => a.Barber)
                .Select(a => new AppointmentReadDTO
                {
                    AppointmentId = a.AppointmentId,
                    UserId = a.UserId,
                    BarberId = a.BarberId,
                    AppointmentDate = a.AppointmentDate,
                    StartTime = a.StartTime,
                    Status = a.Status,
                    Barber = new BarberDTO
                    {
                        BarberId = a.Barber.BarberId,
                        Name = a.Barber.Name
                    }
                })
                .ToListAsync();

            return Ok(appointments);
        }

        [Authorize("Roles=Barber")]
        [HttpGet("barber/myappointments")]
        public async Task<ActionResult<IEnumerable<AppointmentReadDTO>>> GetBarberAppointments()
        {
            var barberIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (barberIdClaim == null)
                return Unauthorized();
            int barberId = int.Parse(barberIdClaim.Value);

            var appointments = await _context.Appointments
                .Where(a => a.BarberId == barberId)
                .Include(a => a.User)
                .Select(a => new AppointmentReadDTO
                {
                    AppointmentId = a.AppointmentId,
                    UserId = a.UserId,
                    BarberId = a.BarberId,
                    AppointmentDate = a.AppointmentDate,
                    StartTime = a.StartTime,
                    Status = a.Status,
                    User = new UserDTO
                    {
                        UserId = a.User.UserId,
                        Name = a.User.Name
                    }
                })
                .ToListAsync();

            return Ok(appointments);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserAppointments(int userId)
        {
            var appointments = await _context.Appointments
                .Where(a => a.UserId == userId)
                .ToListAsync();
            return Ok(appointments);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AppointmentCreateDTO dto)
        {
            try
            {
                dto.AppointmentDate = DateTime.SpecifyKind(dto.AppointmentDate, DateTimeKind.Utc);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (string.IsNullOrEmpty(dto.Status))
                {
                    dto.Status = "beklemede";
                }

                TimeSpan startTime;
                try
                {
                    startTime = TimeSpan.Parse(dto.StartTime);
                }
                catch (FormatException)
                {
                    return BadRequest("Saat formatı hatalı. Lütfen HH:mm formatında yazınız. (Örnek: 13:00)");
                }

                bool conflict = await _context.Appointments.AnyAsync(a =>
                    a.BarberId == dto.BarberId &&
                    a.AppointmentDate == dto.AppointmentDate &&
                    a.StartTime == startTime
                );

                if (conflict)
                {
                    return Conflict("Bu saat için bu berbere ait zaten bir randevu var.");
                }

                var appointment = new Appointment
                {
                    UserId = dto.UserId,
                    BarberId = dto.BarberId,
                    AppointmentDate = dto.AppointmentDate,
                    StartTime = startTime,
                    Status = dto.Status ?? "beklemede"
                };

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                var readDto = new AppointmentReadDTO
                {
                    AppointmentId = appointment.AppointmentId,
                    UserId = appointment.UserId,
                    BarberId = appointment.BarberId,
                    AppointmentDate = appointment.AppointmentDate,
                    StartTime = appointment.StartTime,
                    Status = appointment.Status,
                    User = null,
                    Barber = null
                };

                return CreatedAtAction(nameof(GetAll), new { id = appointment.AppointmentId }, readDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Beklenmeyen hata: " + ex);

                return StatusCode(500, $"Beklenmeyen hata: {ex.Message} \n Detay: {ex.InnerException?.Message}");
            }



        }

        [HttpGet("barber/{barberId}/appointments")]
        public async Task<ActionResult<IEnumerable<AppointmentReadDTO>>> GetAppointmentsForBarber(int barberId)
        {
            var appointments = await _context.Appointments
                .Where(a => a.BarberId == barberId)
                .Include(a => a.User)
                .ToListAsync();

            var result = _mapper.Map<List<AppointmentReadDTO>>(appointments);

            //var result = appointments.Select(a => new AppointmentReadDTO
            //{
            //    AppointmentId = a.AppointmentId,
            //    UserName = a.User.Name,
            //    UserId = a.UserId,
            //    AppointmentDate = a.AppointmentDate,
            //    StartTime = a.StartTime,
            //    Status = a.Status,
            //});

            return Ok(result);
        }

    }

}

