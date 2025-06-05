using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTrackerAPI.Data;
using ProjectTrackerAPI.Models;

namespace ProjectTrackerAPI.Controllers
{
    [ApiController]

    // /api/customer �eklinde ula��labilir
    [Route("api/[controller]")]

    // Bu controller'daki t�m endpoint'ler i�in giri� yap�lm�� olmal� (JWT zorunlu)
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _context;

        // DbContext (veritaban� ba�lant�s�)
        public CustomerController(AppDbContext context)
        {
            _context = context;
        }

        // T�M M��TER�LER� GET�R
        // GET /api/customer
        [HttpGet]
        public ActionResult<IEnumerable<Customer>> GetAll()
        {
            // T�m m��terileri listele ve d�nd�r
            return Ok(_context.Customers.ToList());
        }

        // ID'YE G�RE M��TER� GET�R
        // GET /api/customer/{id}
        [HttpGet("{id}")]
        public ActionResult<Customer> GetById(int id)
        {
            // �lgili ID'deki m��teriyi bul
            var customer = _context.Customers.Find(id);

            // Yoksa 404 d�nd�r
            if (customer == null) return NotFound();

            // Varsa m��teriyi d�nd�r
            return Ok(customer);
        }

        // YEN� M��TER� EKLE (Sadece Admin ve Manager)
        // POST /api/customer
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Create(Customer customer)
        {
            // Yeni m��teriyi veritaban�na ekle
            _context.Customers.Add(customer);
            _context.SaveChanges();

            // 201 Created d�nd�r + yeni kayna��n adresini belirt
            return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
        }

        // M��TER� B�LG�LER�N� G�NCELLE (Sadece Admin ve Manager)
        // PUT /api/customer/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Update(int id, Customer updated)
        {
            // G�ncellenecek m��teri var m� kontrol et
            var customer = _context.Customers.Find(id);
            if (customer == null) return NotFound();

            // Yeni bilgilerle g�ncelle
            customer.Name = updated.Name;
            customer.ContactInfo = updated.ContactInfo;

            _context.SaveChanges();
            return NoContent(); // Ba�ar�l� ama i�erik yok
        }

        // M��TER�Y� S�L (Admin)
        // DELETE /api/customer/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            // Silinecek m��teriyi bul
            var customer = _context.Customers.Find(id);
            if (customer == null) return NotFound();

            // M��teriyi veritaban�ndan kald�r
            _context.Customers.Remove(customer);
            _context.SaveChanges();

            return Ok("M��teri silindi.");
        }
    }
}
