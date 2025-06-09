using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer;
using DataAccessLayer.Models;
using System.Linq;

namespace KE03_INTDEV_SE_2_Base.Controllers
{
    public class StockController : Controller
    {
        private readonly MatrixIncDbContext _context;

        public StockController(MatrixIncDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string sortOrder)
        {
            var parts = _context.Parts.AsQueryable();

            switch (sortOrder)
            {
                case "low-high":
                    parts = parts.OrderBy(p => p.Stock);
                    break;
                case "high-low":
                    parts = parts.OrderByDescending(p => p.Stock);
                    break;
                case "medium":
                    parts = parts
                        .Where(p => p.Stock > 0 && p.Stock < 10)
                        .OrderBy(p => p.Stock);
                    break;
                case "none":
                    parts = parts.Where(p => p.Stock == 0);
                    break;
                default:
                    parts = parts.OrderBy(p => p.Name);
                    break;
            }

            return View(await parts.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var part = await _context.Parts.FirstOrDefaultAsync(p => p.Id == id);
            if (part == null) return NotFound();

            return View(part);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var part = await _context.Parts.FindAsync(id);
            if (part == null) return NotFound();

            return View(part);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int amountToAdd)
        {
            var part = await _context.Parts.FindAsync(id);
            if (part == null) return NotFound();

            if (amountToAdd > 0 && (part.Stock + amountToAdd) > 0)
            {
                part.Stock += amountToAdd;
                _context.Update(part);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
