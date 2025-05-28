using System.Diagnostics;
using KE03_INTDEV_SE_2_Base.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DataAccessLayer;
using DataAccessLayer.Models;
using System.Linq;
using System.Collections.Generic;
using System;

namespace KE03_INTDEV_SE_2_Base.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MatrixIncDbContext _context;

        public HomeController(ILogger<HomeController> logger, MatrixIncDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        #region Get Monthly registration data
        private (List<string> Months, List<int> Counts) GetMonthlyRegistrationData()
        {
            var allCustomers = _context.Customers.ToList();

            if (!allCustomers.Any())
            {
                return (new List<string>(), new List<int>());
            }

            var groupedData = allCustomers
                .GroupBy(c => new { c.DateRegistered.Year, c.DateRegistered.Month })
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month)
                .Select(g => new
                {
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy"),
                    Count = g.Count()
                })
                .ToList();

            var months = groupedData.Select(g => g.Month).ToList();
            var counts = groupedData.Select(g => g.Count).ToList();

            return (months, counts);
        }
        #endregion
        
        #region Get Monthly order data
        private (List<string> Months, List<int> Counts) GetMonthlyOrderData()
        {
            var allOrders = _context.Orders.ToList();

            if (!allOrders.Any())
            {
                return (new List<string>(), new List<int>());
            }

            var groupedData = allOrders
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month)
                .Select(g => new
                {
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy"),
                    Count = g.Count()
                })
                .ToList();

            var months = groupedData.Select(g => g.Month).ToList();
            var counts = groupedData.Select(g => g.Count).ToList();

            return (months, counts);
        }
        #endregion
        public IActionResult Index()
        {
            var (customerMonths, registeredCustomersCount) = GetMonthlyRegistrationData();
            var (orderMonths, orderCounts) = GetMonthlyOrderData();

            ViewData["CustomerChartTitle"] = customerMonths.Any()
                ? "Users"
                : "No User Registration Data Available";

            ViewData["OrderChartTitle"] = orderMonths.Any()
                ? "Orders"
                : "No Order Data Available";

            ViewData["CustomerMonths"] = customerMonths;
            ViewData["RegisteredCustomersCount"] = registeredCustomersCount;

            ViewData["OrderMonths"] = orderMonths;
            ViewData["OrderCounts"] = orderCounts;

            return View();
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}