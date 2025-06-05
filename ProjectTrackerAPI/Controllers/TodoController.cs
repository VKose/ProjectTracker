using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectTrackerAPI.Data;
using ProjectTrackerAPI.Models;
using ProjectTrackerAPI.Services;

namespace ProjectTrackerAPI.Controllers
{
    [ApiController]

    // /api/todo ile eri�ilir
    [Route("api/[controller]")]
    [Authorize]
    public class TodoController : ControllerBase
    {
        private readonly AppDbContext _context;
        public TodoController(AppDbContext context)
        {
            _context = context;
        }

        // T�M G�REVLER� GET�R
        // GET /api/todo
        [HttpGet]
        public IActionResult GetAll()
        {
            // G�revlerle birlikte atanan kullan�c� ve projeyi dahil et
            var todos = _context.Todos
                .Include(t => t.AssignedUser)
                .Include(t => t.Project)
                .ToList();

            return Ok(todos);
        }

        // YEN� G�REV OLU�TUR (Sadece Admin ve Manager)
        // POST /api/todo
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Create(Todo todo)
        {
            _context.Todos.Add(todo);
            _context.SaveChanges();

            // Olu�turulan g�revi geri d�nd�r
            return CreatedAtAction(nameof(GetAll), todo);
        }

        // G�REV� G�NCELLE (Sadece Admin ve Manager)
        // PUT /api/todo/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Update(int id, Todo updated)
        {
            var todo = _context.Todos.Find(id);
            if (todo == null) return NotFound();

            // G�rev bilgilerini g�ncelle
            todo.Title = updated.Title;
            todo.Description = updated.Description;
            todo.Status = updated.Status;
            todo.DueDate = updated.DueDate;

            _context.SaveChanges();
            return NoContent(); // Ba�ar�l� ama i�erik yok
        }

        // G�REV� S�L (Sadece Admin)
        // DELETE /api/todo/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var todo = _context.Todos.Find(id);
            if (todo == null) return NotFound();

            _context.Todos.Remove(todo);
            _context.SaveChanges();

            return Ok("G�rev silindi.");
        }

        // G�REVE KULLANICI ATA (Sadece Admin ve Manager)
        // POST /api/todo/{id}/assign-user/{userId}
        [HttpPost("{id}/assign-user/{userId}")]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult AssignUser(int id, int userId)
        {
            var todo = _context.Todos.Find(id);
            var user = _context.Users.Find(userId);

            if (todo == null || user == null)
                return NotFound();

            // Kullan�c�y� g�reve ata
            todo.AssignedUserId = userId;
            _context.SaveChanges();

            return Ok("Kullan�c� g�reve atand�.");
        }

        // G�REV DURUMUNU G�NCELLE
        // PUT /api/todo/{id}/status
        [HttpPut("{id}/status")]
        [Authorize] // EKS�KT�: Giri� yapmam�� biri g�rev g�ncelleyebilirdi!
        public IActionResult UpdateStatus(int id, [FromBody] string newStatus)
        {
            var todo = _context.Todos.Find(id);
            if (todo == null) return NotFound();

            // Durumu g�ncelle (Bekliyor, Devam Ediyor, Tamamland�)
            todo.Status = newStatus;
            _context.SaveChanges();

            return Ok("Durum g�ncellendi.");
        }

        // G�REVE ATANAN KULLANICIYA E-POSTA G�NDER (Sadece Admin ve Manager)
        // POST /api/todo/{id}/notify
        [HttpPost("{id}/notify")]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult NotifyUser(int id, [FromServices] EmailService mail)
        {
            // G�revi, atanan kullan�c�y� ve projeyi getir
            var todo = _context.Todos
                .Include(t => t.AssignedUser)
                .Include(t => t.Project)
                .FirstOrDefault(t => t.Id == id);

            if (todo == null || todo.AssignedUser == null)
                return NotFound("G�rev veya kullan�c� bulunamad�.");

            // HTML e-posta i�eri�ini haz�rla
            var htmlBody = $@"
                <h3>Yakla�an G�rev Hat�rlatmas�</h3>
                <p><strong>Proje:</strong> {todo.Project?.Title ?? "Bilinmiyor"}</p>
                <p><strong>G�rev:</strong> {todo.Title}</p>
                <p><strong>Termin:</strong> {todo.DueDate:dd.MM.yyyy}</p>
                <p><strong>Sorumlu:</strong> {todo.AssignedUser.Name}</p>
            ";

            // E-postay� g�nder
            mail.Send(
                to: todo.AssignedUser.Email,
                subject: $"G�rev Hat�rlatmas�: {todo.Title}",
                body: htmlBody,
                isHtml: true
            );

            return Ok("HTML formatl� e-posta g�nderildi.");
        }
    }
}
