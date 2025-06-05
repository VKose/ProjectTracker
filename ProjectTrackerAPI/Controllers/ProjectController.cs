using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectTrackerAPI.Data;
using ProjectTrackerAPI.Models;
using System.Security.Claims;

namespace ProjectTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // T�m endpointler i�in giri� (JWT) zorunludur
    public class ProjectController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectController(AppDbContext context)
        {
            _context = context;
        }

        // T�M PROJELER� GET�R
        // GET: /api/project
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Employee")] // Gerekirse bu rollerle s�n�rland�r�labilir
        public IActionResult GetAll()
        {
            try
            {
                var projects = _context.Projects
                    .Include(p => p.Customer) // Projeye ait m��teri bilgisi
                    .Include(p => p.Users)    // Projede g�revli kullan�c�lar
                    .ToList();

                return Ok(projects);
            }
            catch (Exception ex)
            {
                // Hata varsa 500 d�n
                return StatusCode(500, $"! Sunucu hatas�: {ex.Message} | {ex.InnerException?.Message}");
            }
        }

        // ID'YE G�RE PROJE GET�R
        // GET: /api/project/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,Employee")] // Gerekirse sadece belirli roller
        public IActionResult GetById(int id)
        {
            var project = _context.Projects
                .Include(p => p.Customer)
                .Include(p => p.Users)
                .FirstOrDefault(p => p.Id == id);

            if (project == null) return NotFound();

            return Ok(project);
        }

        // YEN� PROJE OLU�TUR
        // POST: /api/project
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Create(Project project)
        {
            _context.Projects.Add(project);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
        }

        // PROJEY� G�NCELLE
        // PUT: /api/project/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Update(int id, Project updated)
        {
            var project = _context.Projects.Find(id);
            if (project == null) return NotFound();

            project.Title = updated.Title;
            project.Description = updated.Description;
            project.Status = updated.Status;

            _context.SaveChanges();
            return NoContent();
        }

        // PROJE S�L (Sadece Admin)
        // DELETE: /api/project/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var project = _context.Projects.Find(id);
            if (project == null) return NotFound();

            _context.Projects.Remove(project);
            _context.SaveChanges();
            return Ok("Proje silindi.");
        }

        // PROJEYE KULLANICI ATA
        // POST: /api/project/{id}/assign-user/{userId}
        [HttpPost("{id}/assign-user/{userId}")]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult AssignUser(int id, int userId)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var currentUserIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(currentUserIdStr, out var currentUserId))
                return Unauthorized("Kullan�c� kimli�i al�namad�.");

            var project = _context.Projects
                .Include(p => p.Users)
                .FirstOrDefault(p => p.Id == id);

            var user = _context.Users.Find(userId);

            if (project == null || user == null)
                return NotFound("Proje veya kullan�c� bulunamad�.");

            // Manager sadece sorumlu oldu�u projelere kullan�c� atayabilir
            if (role == "Manager" && !project.Users.Any(u => u.Id == currentUserId))
                return Forbid("Bu projede kullan�c� atama yetkiniz yok.");

            if (!project.Users.Contains(user))
                project.Users.Add(user);

            _context.SaveChanges();
            return Ok("Kullan�c� projeye atand�.");
        }

        // PROJEYE M��TER� ATA (Sadece Admin)
        // POST: /api/project/{id}/assign-customer/{customerId}
        [HttpPost("{id}/assign-customer/{customerId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult AssignCustomer(int id, int customerId)
        {
            var project = _context.Projects.Find(id);
            var customer = _context.Customers.Find(customerId);

            if (project == null || customer == null)
                return NotFound("Proje veya m��teri bulunamad�.");

            project.CustomerId = customerId;

            _context.SaveChanges();
            return Ok("M��teri projeye atand�.");
        }
    }
}
