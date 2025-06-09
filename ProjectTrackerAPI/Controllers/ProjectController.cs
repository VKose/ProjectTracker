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
    [Authorize] // Tüm endpointler için giriþ (JWT) zorunludur
    public class ProjectController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectController(AppDbContext context)
        {
            _context = context;
        }

        // TÜM PROJELERÝ GETÝR
        // GET: /api/project
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Employee")] 
        public IActionResult GetAll()
        {
            try
            {
                var projects = _context.Projects
                    .Include(p => p.Customer) // Projeye ait müþteri bilgisi
                    .Include(p => p.Users)    // Projede görevli kullanýcýlar
                    .ToList();

                return Ok(projects);
            }
            catch (Exception ex)
            {
                // Hata varsa 500 dön
                return StatusCode(500, $"! Sunucu hatasý: {ex.Message} | {ex.InnerException?.Message}");
            }
        }

        // ID'YE GÖRE PROJE GETÝR
        // GET: /api/project/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,Employee")]
        public IActionResult GetById(int id)
        {
            var project = _context.Projects
                .Include(p => p.Customer)
                .Include(p => p.Users)
                .FirstOrDefault(p => p.Id == id);

            if (project == null) return NotFound();

            return Ok(project);
        }

        // YENÝ PROJE OLUÞTUR
        // POST: /api/project
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Create(Project project)
        {
            _context.Projects.Add(project);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
        }

        // PROJEYÝ GÜNCELLE
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

        // PROJE SÝL (Sadece Admin)
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
                return Unauthorized("Kullanýcý kimliði alýnamadý.");

            var project = _context.Projects
                .Include(p => p.Users)
                .FirstOrDefault(p => p.Id == id);

            var user = _context.Users.Find(userId);

            if (project == null || user == null)
                return NotFound("Proje veya kullanýcý bulunamadý.");

            // Manager sadece sorumlu olduðu projelere kullanýcý atayabilir
            if (role == "Manager" && !project.Users.Any(u => u.Id == currentUserId))
                return Forbid("Bu projede kullanýcý atama yetkiniz yok.");

            if (!project.Users.Contains(user))
                project.Users.Add(user);

            _context.SaveChanges();
            return Ok("Kullanýcý projeye atandý.");
        }

        // PROJEYE MÜÞTERÝ ATA (Sadece Admin)
        // POST: /api/project/{id}/assign-customer/{customerId}
        [HttpPost("{id}/assign-customer/{customerId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult AssignCustomer(int id, int customerId)
        {
            var project = _context.Projects.Find(id);
            var customer = _context.Customers.Find(customerId);

            if (project == null || customer == null)
                return NotFound("Proje veya müþteri bulunamadý.");

            project.CustomerId = customerId;

            _context.SaveChanges();
            return Ok("Müþteri projeye atandý.");
        }
    }
}
