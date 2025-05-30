using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer
{
    public class MatrixIncDbContext : DbContext
    {
        public MatrixIncDbContext(DbContextOptions<MatrixIncDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Part> Parts { get; set; }
        public DbSet<OrderPart> OrderParts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.CustomerId).IsRequired();
            
            modelBuilder.Entity<OrderPart>()
                .HasKey(op => new { op.OrderId, op.PartId });

            modelBuilder.Entity<OrderPart>()
                .HasOne(op => op.Order)
                .WithMany(o => o.OrderParts)
                .HasForeignKey(op => op.OrderId);

            modelBuilder.Entity<OrderPart>()
                .HasOne(op => op.Part)
                .WithMany(p => p.OrderParts)
                .HasForeignKey(op => op.PartId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
