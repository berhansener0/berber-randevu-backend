using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BerberRandevuAPI.Data;
using BerberRandevuAPI.Models;
using BerberRandevuAPI.DTOs;
using System.Linq.Expressions;

namespace BerberRandevuAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class WorkingHourController : ControllerBase
    {
        private readonly BerberContext _context;

        public WorkingHourController(BerberContext context)
        {
            _context = context;
        }
        [HttpGet]

        public async Task<ActionResult<IEnumerable<WorkingHour>>> GetAll()
        {
            var workingHours = await _context.WorkingHours
            .Include(w => w.Barber)
            .ToListAsync();

            return Ok(workingHours);
        }
        [HttpPost]
        public async Task<ActionResult> Create(WorkingHourCreateDTO dto)
        {
            try
            {
                var workingHour = new WorkingHour
                {
                    BarberId = dto.BarberId,
                    DayOfWeek = (DayOfWeek)dto.DayOfWeek,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                };

                _context.WorkingHours.Add(workingHour);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetAll), new { id = workingHour.Id }, workingHour);
            }
            catch (Exception)
            {
                return BadRequest("Bu kuaför için bugün zaten çalışma saati tanımlı!!");
            }

        }

    }
}
