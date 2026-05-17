using BusinessRegistrationSystem.Services;
using BusinessRegistrationSystem.Data;
using BusinessRegistrationSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BusinessRegistrationSystem.Controllers
{
    public class BusinessController : Controller
    {
        private readonly IBusinessService _businessService;
        private readonly AppDbContext _dbContext;
        private readonly PdfGeneratorService _pdfGeneratorService;

        public BusinessController(IBusinessService businessService, AppDbContext dbContext, PdfGeneratorService pdfGeneratorService)
        {
            _businessService = businessService;
            _dbContext = dbContext;
            _pdfGeneratorService = pdfGeneratorService;
        }

        [HttpGet]
        public IActionResult SearchName()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SearchName(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                ViewBag.Error = "Please enter a business name to search.";
                return View();
            }

            var jsonResponse = await _businessService.SearchNameAsync(searchText);
            
            try 
            {
                var result = JsonSerializer.Deserialize<JsonElement>(jsonResponse);
                
                if (result.TryGetProperty("error", out var errorProp))
                {
                    ViewBag.Error = errorProp.GetString();
                }
                else
                {
                    ViewBag.SearchResult = result;
                }
            }
            catch
            {
                ViewBag.Error = "Error parsing the response from the name search service.";
            }

            ViewBag.SearchText = searchText;
            return View();
        }

        [HttpGet]
        [Authorize]
        public IActionResult ReserveName(string name)
        {
            if (string.IsNullOrEmpty(name)) return RedirectToAction("SearchName");
            var model = new BusinessRegistration { ReservationName = name };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ReserveName(BusinessRegistration registration)
        {
            var userIdStr = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (registration.Directors != null)
            {
                foreach (var director in registration.Directors)
                {
                    director.DateOfBirth = DateTime.SpecifyKind(director.DateOfBirth, DateTimeKind.Utc);
                }
            }

            var existingApp = registration.Id != Guid.Empty 
                ? await _dbContext.BusinessRegistrations
                    .Include(b => b.Directors)
                    .Include(b => b.Shareholders)
                    .Include(b => b.Secretary)
                    .FirstOrDefaultAsync(b => b.Id == registration.Id)
                : null;

            if (existingApp != null)
            {
                if (existingApp.OwnerId != userId) return Unauthorized();

                existingApp.Objectives = registration.Objectives;
                existingApp.ReservationName = registration.ReservationName;
                existingApp.CompanyEmail = registration.CompanyEmail;
                existingApp.CompanyPhoneNumber = registration.CompanyPhoneNumber;
                existingApp.TotalCapital = registration.TotalCapital;
                
                _dbContext.Directors.RemoveRange(existingApp.Directors);
                _dbContext.Shareholders.RemoveRange(existingApp.Shareholders);
                if (existingApp.Secretary != null) _dbContext.Secretaries.Remove(existingApp.Secretary);

                existingApp.Directors.Clear();
                if (registration.Directors != null)
                {
                    foreach (var d in registration.Directors) 
                    { 
                        d.Id = Guid.Empty; 
                        existingApp.Directors.Add(d); 
                    }
                }

                existingApp.Shareholders.Clear();
                if (registration.Shareholders != null)
                {
                    foreach (var s in registration.Shareholders) 
                    { 
                        s.Id = Guid.Empty; 
                        existingApp.Shareholders.Add(s); 
                    }
                }

                if (registration.Secretary != null)
                {
                    registration.Secretary.Id = Guid.Empty;
                    existingApp.Secretary = registration.Secretary;
                }
                else
                {
                    existingApp.Secretary = null;
                }
            }
            else
            {
                registration.Id = Guid.NewGuid();
                registration.OwnerId = userId;
                registration.SubmittedAt = DateTime.UtcNow;
                registration.Status = "Pending";
                _dbContext.BusinessRegistrations.Add(registration);
            }

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("LoginSuccess", "Auth");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ViewApplication(Guid id)
        {
            var application = await _dbContext.BusinessRegistrations
                .Include(b => b.Directors)
                .Include(b => b.Shareholders)
                .Include(b => b.Secretary)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (application == null) return NotFound();

            var userIdStr = User.FindFirst("UserId")?.Value;
            if (application.OwnerId.ToString() != userIdStr && !User.IsInRole("Admin")) return Unauthorized();

            return View(application);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditApplication(Guid id)
        {
            var application = await _dbContext.BusinessRegistrations
                .Include(b => b.Directors)
                .Include(b => b.Shareholders)
                .Include(b => b.Secretary)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (application == null) return NotFound();

            var userIdStr = User.FindFirst("UserId")?.Value;
            if (application.OwnerId.ToString() != userIdStr && !User.IsInRole("Admin")) return Unauthorized();

            return View("ReserveName", application);
        }

        [HttpPost]
        [Authorize]
        [RequestSizeLimit(100_000_000)] // allow large forms if needed
        public IActionResult PreviewAoa([FromForm] BusinessRegistration registration)
        {
            if (registration == null) return BadRequest();
            var pdfBytes = _pdfGeneratorService.GenerateArticlesOfAssociation(registration);
            return File(pdfBytes, "application/pdf");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DownloadAoa(Guid id)
        {
            var application = await _dbContext.BusinessRegistrations
                .Include(b => b.Directors)
                .Include(b => b.Shareholders)
                .Include(b => b.Secretary)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (application == null) return NotFound();

            var userIdStr = User.FindFirst("UserId")?.Value;
            if (application.OwnerId.ToString() != userIdStr && !User.IsInRole("Admin")) return Unauthorized();

            var pdfBytes = _pdfGeneratorService.GenerateArticlesOfAssociation(application);
            return File(pdfBytes, "application/pdf", $"{application.ReservationName}_Articles.pdf");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(Guid id, string status)
        {
            var application = await _dbContext.BusinessRegistrations.FindAsync(id);
            if (application != null)
            {
                // Only allow valid statuses to prevent tampering
                if (status == "Pending" || status == "Approved" || status == "Rejected")
                {
                    application.Status = status;
                    await _dbContext.SaveChangesAsync();
                }
            }
            return RedirectToAction("LoginSuccess", "Auth");
        }
    }
}
