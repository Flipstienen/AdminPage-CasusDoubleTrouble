using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Bogus; // Make sure to have Bogus installed via NuGet

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
            var customerFaker = new Faker<Customer>()
                .RuleFor(c => c.Name, f => f.Person.FullName)
                .RuleFor(c => c.Address, f => f.Address.FullAddress())
                .RuleFor(c => c.DateRegistered, f => f.Date.Past(2, DateTime.Today).Date); // Past 2 years, date only

            var customers = customerFaker.Generate(10);
            context.Customers.AddRange(customers);
            context.SaveChanges();
            #endregion

            #region Seeding Parts
            var partFaker = new Faker<Part>()
                .RuleFor(p => p.Name, f => f.Commerce.ProductAdjective() + " " + f.Commerce.Product()) 
                .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
                .RuleFor(p => p.Price, f => decimal.Parse(f.Commerce.Price(1, 1000, 2))) 
                .RuleFor(p => p.ImageUrl, f => f.Image.PicsumUrl(400, 300));

            var parts = partFaker.Generate(50);
            context.Parts.AddRange(parts);
            context.SaveChanges();
            #endregion
            
            #region Seeding orders
            var orderFaker = new Faker<Order>()
                .RuleFor(o => o.CustomerId, f => f.PickRandom(customers).Id) // Assign existing Customer's ID
                .RuleFor(o => o.OrderDate, (f, o) =>
                {
                    var associatedCustomer = customers.First(c => c.Id == o.CustomerId);
                    return f.Date.Between(associatedCustomer.DateRegistered, DateTime.Today);
                });

            var orders = orderFaker.Generate(50);
            context.Orders.AddRange(orders);
            context.SaveChanges();
            #endregion

            #region Seeding order parts (many-to-many relationship)
            Random rand = new Random(28052025);
            foreach (var order in orders)
            {
                int numberOfPartsForOrder = rand.Next(1, 6);
                int partsToTake = Math.Min(numberOfPartsForOrder, parts.Count);
                var selectedParts = parts.OrderBy(p => rand.Next()) // Randomize parts list
                                         .Take(partsToTake) // Take the desired number
                                         .ToList();

                foreach (var part in selectedParts)
                {
                    order.Parts.Add(part);
                }
            }
            context.SaveChanges();
            #endregion
            
            context.Database.EnsureCreated();
            
        }
    }
}