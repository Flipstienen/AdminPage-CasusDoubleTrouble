using System.Data.SqlTypes;
using DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace KE03_INTDEV_SE_2_Base.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly MatrixIncDbContext _context;

        public StatisticsController(MatrixIncDbContext context)
        {
            _context = context;
        }

        private decimal GetAllInkom()
        {
            decimal money = 0;
            var orderPart = _context.OrderParts.Include(op => op.Part);
            foreach(var item in orderPart)
            {
                money += item.Part.Price * item.Quantity;
            }

            return money;
        }
        private decimal GetAllSpend()
        {
            decimal money = 0;
            var parts = _context.Parts.AsEnumerable();
            foreach (var item in parts)
            {
                money += item.Stock * item.BuyInPrice;
            }

            return money;
        }
        private decimal GetAllGained()
        {
            return (GetAllInkom() - GetAllSpend());
        }

        private int GetAllBought()
        {
            int bought = 0;
            var orderParts = _context.OrderParts.Include(op => op.Part);
            foreach(var item in orderParts)
            {
                bought += item.Quantity;
                if(item.Part.Stock >= 0)
                {
                    bought += item.Part.Stock;
                }
            }
            return bought;
        }

        private int GetAllSold()
        {
            int sold = 0;
            var orderParts = _context.OrderParts;
            foreach(var item in orderParts)
            {
                sold += item.Quantity;
            }

            return sold;
        }
        private int GetAllLeft()
        {
            return (GetAllBought()-GetAllSold());
        }
        public IActionResult Index()
        {
            ViewData["income"] = GetAllInkom();
            ViewData["spent"] = GetAllSpend();
            ViewData["gained"] = GetAllGained();
            ViewData["bought"] = GetAllBought();
            ViewData["sold"] = GetAllSold();
            ViewData["leftover"] = GetAllLeft(); 
            if (GetAllGained() >= 0)
            {
                ViewData["winorloss"] = "Gained";
            }

            else
            {
                ViewData["winorloss"] = "Losses";
            }
            return View();
        }
    }
}
