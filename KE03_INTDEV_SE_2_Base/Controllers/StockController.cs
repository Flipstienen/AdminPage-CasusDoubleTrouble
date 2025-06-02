using DataAccessLayer;
using Microsoft.AspNetCore.Mvc;

namespace KE03_INTDEV_SE_2_Base.Controllers
{ 
public class StockController : Controller
{
    private readonly MatrixIncDbContext _context;

    public StockController(MatrixIncDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var parts = _context.Parts.ToList();
        return View(parts);
    }
}
    }
