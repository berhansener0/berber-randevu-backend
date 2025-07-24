using Microsoft.AspNetCore.Mvc;
using BerberRandevuAPI.Data;
using System;

namespace BerberRandevuAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly BerberContext _context;

        public TestController(BerberContext context)
        {
            _context = context;
        }

        [HttpGet("test-db")]
        public IActionResult TestDbConnection()
        {
            try
            {
                var canConnect = _context.Database.CanConnect();

                if (canConnect)
                    return Ok(" Veritabanı bağlantısı başarılı.");
                else
                    return StatusCode(500, " Veritabanı bağlantısı başarısız.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Hata: {ex.Message}");
            }
        }
        [HttpGet("berber-varmi")]
        public IActionResult CheckBerberCount()
        {
            var count = _context.Barbers.Count();
            return Ok($"Toplam {count} berber kayıtlı.");
        }

    }
}
