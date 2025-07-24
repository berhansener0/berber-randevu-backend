using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BerberRandevuAPI.Data;
using BerberRandevuAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BerberRandevuAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class BarberController : ControllerBase
    {
        private readonly BerberContext _context;
        public BarberController (BerberContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Barber>> CreateBarber([FromBody] Barber newBarber)
        {
            _context.Barbers.Add(newBarber);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBarbers), new { id = newBarber.BarberId }, newBarber);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Barber>>>GetBarbers()
        {
            var barbers=await _context.Barbers.ToListAsync();
            return Ok(barbers);
        }
    }
}
