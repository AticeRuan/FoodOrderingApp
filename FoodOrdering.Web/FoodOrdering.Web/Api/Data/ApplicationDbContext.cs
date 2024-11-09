using FoodOrdering.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodOrdering.Web.Api.Data
    {
    public class ApplicationDbContext : DbContext
        {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<FoodMenuItem> MenuItems => Set<FoodMenuItem>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
            base.OnModelCreating(modelBuilder);

            // Configure FoodMenuItem
            modelBuilder.Entity<FoodMenuItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).IsRequired();
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Category).IsRequired();
            });

            // Configure Order
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OrderDate).IsRequired();
                entity.Property(e => e.IsDelivery).IsRequired();
                entity.Property(e => e.ScheduledDateTime).IsRequired();
                entity.Property(e => e.CustomerPhone).HasMaxLength(20);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");

                entity.OwnsOne(e => e.CustomerAddress, address =>
                {
                    address.Property(a => a.Unit).HasColumnName("CustomerUnit");
                    address.Property(a => a.StreetNumber).HasColumnName("CustomerStreetNumber");
                    address.Property(a => a.StreetName).HasColumnName("CustomerStreetName");
                    address.Property(a => a.Suburb).HasColumnName("CustomerSuburb");
                   
                });

                entity.OwnsOne(e => e.CustomerName, name =>
                {
                    name.Property(a => a.FirstName);
                    name.Property(a => a.LastName);
                });

                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);

               
                entity.Property(e => e.FullAddress).HasMaxLength(200);

                entity.HasMany(e => e.Items)
                    .WithOne(e => e.Order)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure OrderItem
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Quantity).IsRequired();

                entity.OwnsMany(e => e.Extras, extra =>
                {
                    extra.WithOwner().HasForeignKey(e => e.OrderItemId);
                    extra.Property(x => x.Name).IsRequired();  // Keep Name required when an extra exists
                    extra.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
                    extra.Property(x => x.Quantity).IsRequired();
                    extra.HasKey(e => e.Id);
                });

                // Make sure Extras collection exists but can be empty
                entity.Navigation(e => e.Extras)
                    .AutoInclude()
                    .UsePropertyAccessMode(PropertyAccessMode.Property);

                entity.Property(e => e.Spice)
                    .HasConversion(
                        v => string.Join(',', v),
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                    );
            });
            }
        }
    }
