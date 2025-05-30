using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;

namespace DataAccessLayer
{
    public static class MatrixIncDbInitializer
    {
        public static void Initialize(MatrixIncDbContext context)
        {
            if (context.Customers.Any())
            {
                return;
            }

            Randomizer.Seed = new Random(28052025);

            #region Seeding Customers
            var startDate = new DateTime(2025, 1, 1);
            var endDate = DateTime.Today;

            var customerFaker = new Faker<Customer>()
                .RuleFor(c => c.Name, f => f.Person.FullName)
                .RuleFor(c => c.Address, f => f.Address.FullAddress())
                .RuleFor(c => c.DateRegistered, f => f.Date.Between(startDate, endDate).Date);

            var customers = customerFaker.Generate(50);
            context.Customers.AddRange(customers);
            context.SaveChanges();
            #endregion

            #region Seeding Parts
            var partFaker = new Faker<Part>()
                .RuleFor(p => p.Name, f => f.Commerce.ProductAdjective() + " " + f.Commerce.Product())
                .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
                .RuleFor(p => p.Price, f => decimal.Parse(f.Commerce.Price(3, 1000, 2))) // Minimum price is 3
                .RuleFor(p => p.BuyInPrice, (f, p) => Math.Round(p.Price - 2, 2))
                .RuleFor(p => p.ImageUrl, f => f.Image.PicsumUrl(400, 300))
                .RuleFor(p => p.Stock, f => f.Random.Int(0, 100));

            var parts = partFaker.Generate(50);
            context.Parts.AddRange(parts);
            context.SaveChanges();
            #endregion

            #region Seeding Orders
            var deliveryOptions = new[] { "Standard", "Express", "Next-Day", "Pickup" };

            var orderFaker = new Faker<Order>()
                .RuleFor(o => o.CustomerId, f => f.PickRandom(customers).Id)
                .RuleFor(o => o.OrderDate, (f, o) =>
                {
                    var associatedCustomer = customers.First(c => c.Id == o.CustomerId);
                    return f.Date.Between(associatedCustomer.DateRegistered, DateTime.Today);
                })
                .RuleFor(o => o.DeliveryOption, f => f.PickRandom(deliveryOptions))
                .RuleFor(o => o.Delivered, f => f.Random.Bool());

            var orders = orderFaker.Generate(50);
            context.Orders.AddRange(orders);
            context.SaveChanges();
            #endregion

            #region Seeding OrderParts (Many-to-Many with Quantity)
            Random rand = new Random(28052025); // Make sure this line is above the loop

            var orderParts = new List<OrderPart>();

            foreach (var order in orders)
            {
                int numberOfPartsForOrder = rand.Next(1, 6);
                int partsToTake = Math.Min(numberOfPartsForOrder, parts.Count);
                var selectedParts = parts.OrderBy(p => rand.Next()).Take(partsToTake).ToList();

                foreach (var part in selectedParts)
                {
                    var quantity = rand.Next(1, 10); // Quantity between 1 and 9

                    orderParts.Add(new OrderPart
                    {
                        Order = order,
                        Part = part,
                        Quantity = quantity
                    });
                }
            }

            context.OrderParts.AddRange(orderParts);
            context.SaveChanges();
            #endregion


            context.Database.EnsureCreated();
        }
    }
}
