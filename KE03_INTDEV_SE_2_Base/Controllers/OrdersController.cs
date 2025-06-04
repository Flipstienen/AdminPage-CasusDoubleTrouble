using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer;
using DataAccessLayer.Models;
using KE03_INTDEV_SE_2_Base.Models;
using SQLitePCL;

namespace KE03_INTDEV_SE_2_Base.Controllers
{
    public class OrdersController : Controller
    {
        private readonly MatrixIncDbContext _context;

        public OrdersController(MatrixIncDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index(int id)
        {
            var matrixIncDbContext = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderParts)
                .ThenInclude(op => op.Part);
            if (id == 1)
            {
                var matrixIncDbContextOrder = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderParts)
                .ThenInclude(op => op.Part).OrderByDescending(o => o.OrderDate); // <-- Include actual part data
                return View(await matrixIncDbContextOrder.ToListAsync());
            }
            

            return View(await matrixIncDbContext.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderParts)
                .ThenInclude(op => op.Part)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            var model = new OrderCreateViewModel
            {
                OrderParts = new List<OrderPartViewModel>
        {
            new OrderPartViewModel() // Initialize with one entry
        }
            };
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Address");
            ViewData["DeliveryOption"] = new SelectList(new List<string> { "Standard", "Express", "Pickup", "Next-Day" });
            ViewData["partId"] = new SelectList(_context.Parts, "Id", "Name");
            return View(model);
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool hasStockError = false;
                ModelState.Clear();
                var order = new Order
                {
                    OrderDate = DateTime.Now,
                    CustomerId = model.CustomerId,
                    DeliveryOption = model.DeliveryOption,
                    Delivered = model.Delivered,
                    OrderParts = model.OrderParts.Select(op => new OrderPart
                    {
                        PartId = op.ProductId,
                        Quantity = op.Quantity
                    }).ToList()
                };
                foreach (var orderPart in order.OrderParts)
                {
                    var part = await _context.Parts.FindAsync(orderPart.PartId);
                    if (part == null)
                    {
                        ModelState.AddModelError("", $"Part with ID {orderPart.PartId} not found.");
                        hasStockError = true;
                    }
                    else if (orderPart.Quantity <= 0)
                    {
                        ModelState.AddModelError("", $"Part '{part.Name}' must have a quantity greater than 0.");
                        hasStockError = true;
                    }
                    else if (part.Stock < orderPart.Quantity)
                    {
                        ModelState.AddModelError("", $"Not enough stock for part '{part.Name}'. Available: {part.Stock}, requested: {orderPart.Quantity}.");
                        hasStockError = true;
                    }
                    else
                    {
                        // Reduce stock only if everything is valid
                        part.Stock -= orderPart.Quantity;
                    }
                }

                if (hasStockError)
                {
                    ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Address", order.CustomerId);
                    ViewData["DeliveryOption"] = new SelectList(new List<string> { "Standard", "Express", "Pickup", "Next-Day" }, order.DeliveryOption);
                    ViewData["partId"] = new SelectList(_context.Parts, "Id", "Name");
                    return View(model);
                }

                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Address");
            ViewData["DeliveryOption"] = new SelectList(new List<string> { "Standard", "Express", "Pickup", "Next-Day" });
            ViewData["partId"] = new SelectList(_context.Parts, "Id", "Name");
            return View(model);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Address", order.CustomerId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderDate,CustomerId")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Address", order.CustomerId);
            return View(order);

        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
