using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer;
using DataAccessLayer.Models;

namespace KE03_INTDEV_SE_2_Base.Controllers
{
    public class StockController : Controller
    {
        private readonly MatrixIncDbContext _context;

        public StockController(MatrixIncDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var parts = await _context.Parts.ToListAsync();
            return View(parts);
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
