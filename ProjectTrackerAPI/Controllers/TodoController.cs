using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectTrackerAPI.Data;
using ProjectTrackerAPI.Models;
using ProjectTrackerAPI.Services;

namespace ProjectTrackerAPI.Controllers
{
    [ApiController]

    // /api/todo ile eriþilir
    [Route("api/[controller]")]
    [Authorize]
    public class TodoController : ControllerBase
    {
        private readonly AppDbContext _context;
        public TodoController(AppDbContext context)
        {
            _context = context;
        }

        // TÜM GÖREVLERÝ GETÝR
        // GET /api/todo
        [HttpGet]
        public IActionResult GetAll()
        {
            // Görevlerle birlikte atanan kullanýcý ve projeyi dahil et
            var todos = _context.Todos
                .Include(t => t.AssignedUser)
                .Include(t => t.Project)
                .ToList();

            return Ok(todos);
        }

        // YENÝ GÖREV OLUÞTUR (Sadece Admin ve Manager)
        // POST /api/todo
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Create(Todo todo)
        {
            _context.Todos.Add(todo);
            _context.SaveChanges();

            // Oluþturulan görevi geri döndür
            return CreatedAtAction(nameof(GetAll), todo);
        }

        // GÖREVÝ GÜNCELLE (Sadece Admin ve Manager)
        // PUT /api/todo/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Update(int id, Todo updated)
        {
            var todo = _context.Todos.Find(id);
            if (todo == null) return NotFound();

            // Görev bilgilerini güncelle
            todo.Title = updated.Title;
            todo.Description = updated.Description;
            todo.Status = updated.Status;
            todo.DueDate = updated.DueDate;

            _context.SaveChanges();
            return NoContent(); // Baþarýlý ama içerik yok
        }

        // GÖREVÝ SÝL (Sadece Admin)
        // DELETE /api/todo/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var todo = _context.Todos.Find(id);
            if (todo == null) return NotFound();

            _context.Todos.Remove(todo);
            _context.SaveChanges();

            return Ok("Görev silindi.");
        }

        // GÖREVE KULLANICI ATA (Sadece Admin ve Manager)
        // POST /api/todo/{id}/assign-user/{userId}
        [HttpPost("{id}/assign-user/{userId}")]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult AssignUser(int id, int userId)
        {
            var todo = _context.Todos.Find(id);
            var user = _context.Users.Find(userId);

            if (todo == null || user == null)
                return NotFound();

            // Kullanýcýyý göreve ata
            todo.AssignedUserId = userId;
            _context.SaveChanges();

            return Ok("Kullanýcý göreve atandý.");
        }

        // GÖREV DURUMUNU GÜNCELLE
        // PUT /api/todo/{id}/status
        [HttpPut("{id}/status")]
        [Authorize]
        public IActionResult UpdateStatus(int id, [FromBody] string newStatus)
        {
            var todo = _context.Todos.Find(id);
            if (todo == null) return NotFound();

            // Durumu güncelle (Bekliyor, Devam Ediyor, Tamamlandý)
            todo.Status = newStatus;
            _context.SaveChanges();

            return Ok("Durum güncellendi.");
        }

        // GÖREVE ATANAN KULLANICIYA E-POSTA GÖNDER (Sadece Admin ve Manager)
        // POST /api/todo/{id}/notify
        [HttpPost("{id}/notify")]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult NotifyUser(int id, [FromServices] EmailService mail)
        {
            // Görevi, atanan kullanýcýyý ve projeyi getir
            var todo = _context.Todos
                .Include(t => t.AssignedUser)
                .Include(t => t.Project)
                .FirstOrDefault(t => t.Id == id);

            if (todo == null || todo.AssignedUser == null)
                return NotFound("Görev veya kullanýcý bulunamadý.");

            // HTML e-posta içeriðini hazýrla
            var htmlBody = $@"
                <h3>Yaklaþan Görev Hatýrlatmasý</h3>
                <p><strong>Proje:</strong> {todo.Project?.Title ?? "Bilinmiyor"}</p>
                <p><strong>Görev:</strong> {todo.Title}</p>
                <p><strong>Termin:</strong> {todo.DueDate:dd.MM.yyyy}</p>
                <p><strong>Sorumlu:</strong> {todo.AssignedUser.Name}</p>
            ";

            // E-postayý gönder
            mail.Send(
                to: todo.AssignedUser.Email,
                subject: $"Görev Hatýrlatmasý: {todo.Title}",
                body: htmlBody,
                isHtml: true
            );

            return Ok("HTML formatlý e-posta gönderildi.");
        }
    }
}
