using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTrackerAPI.Data;
using ProjectTrackerAPI.Models;

namespace ProjectTrackerAPI.Controllers
{
    [ApiController]

    // /api/customer þeklinde ulaþýlabilir
    [Route("api/[controller]")]

    // Bu controller'daki tüm endpoint'ler için giriþ yapýlmýþ olmalý (JWT zorunlu)
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _context;

        // DbContext (veritabaný baðlantýsý)
        public CustomerController(AppDbContext context)
        {
            _context = context;
        }

        // TÜM MÜÞTERÝLERÝ GETÝR
        // GET /api/customer
        [HttpGet]
        public ActionResult<IEnumerable<Customer>> GetAll()
        {
            // Tüm müþterileri listele ve döndür
            return Ok(_context.Customers.ToList());
        }

        // ID'YE GÖRE MÜÞTERÝ GETÝR
        // GET /api/customer/{id}
        [HttpGet("{id}")]
        public ActionResult<Customer> GetById(int id)
        {
            // Ýlgili ID'deki müþteriyi bul
            var customer = _context.Customers.Find(id);

            // Yoksa 404 döndür
            if (customer == null) return NotFound();

            // Varsa müþteriyi döndür
            return Ok(customer);
        }

        // YENÝ MÜÞTERÝ EKLE (Sadece Admin ve Manager)
        // POST /api/customer
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Create(Customer customer)
        {
            // Yeni müþteriyi veritabanýna ekle
            _context.Customers.Add(customer);
            _context.SaveChanges();

            // 201 Created döndür + yeni kaynaðýn adresini belirt
            return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
        }

        // MÜÞTERÝ BÝLGÝLERÝNÝ GÜNCELLE (Sadece Admin ve Manager)
        // PUT /api/customer/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Update(int id, Customer updated)
        {
            // Güncellenecek müþteri var mý kontrol et
            var customer = _context.Customers.Find(id);
            if (customer == null) return NotFound();

            // Yeni bilgilerle güncelle
            customer.Name = updated.Name;
            customer.ContactInfo = updated.ContactInfo;

            _context.SaveChanges();
            return NoContent(); // Baþarýlý ama içerik yok
        }

        // MÜÞTERÝYÝ SÝL (Admin)
        // DELETE /api/customer/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            // Silinecek müþteriyi bul
            var customer = _context.Customers.Find(id);
            if (customer == null) return NotFound();

            // Müþteriyi veritabanýndan kaldýr
            _context.Customers.Remove(customer);
            _context.SaveChanges();

            return Ok("Müþteri silindi.");
        }
    }
}
