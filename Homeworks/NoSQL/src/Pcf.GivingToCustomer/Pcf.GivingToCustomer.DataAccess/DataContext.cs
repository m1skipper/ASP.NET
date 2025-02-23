using Microsoft.EntityFrameworkCore;
using Pcf.GivingToCustomer.Core.Domain;

namespace Pcf.GivingToCustomer.DataAccess
{
    public class DataContext
        : DbContext
    {
        public DbSet<PromoCode> PromoCodes { get; set; }

        public DbSet<Customer> Customers { get; set; }
        
        public DbSet<Preference> Preferences { get; set; }

        public DataContext()
        {
            
        }
        
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
            // 'The MongoDB EF Core Provider now uses transactions to ensure all updates in a SaveChanges operation are applied together or not at all.
            // Your current MongoDB server configuration does not support transactions and you should consider switching to a replica set or load
            // balanced configuration. If you are sure you do not need save consistency or optimistic concurrency you can disable transactions by setting
            // 'Database.AutoTransactionBehavior = AutoTransactionBehavior.Never' on your DbContext.'
            Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerPreference>()
                .HasKey(bc => bc.Id);
            modelBuilder.Entity<CustomerPreference>()
                .HasAlternateKey(bc => new { bc.CustomerId, bc.PreferenceId });
            modelBuilder.Entity<CustomerPreference>()
                .HasOne(bc => bc.Customer)
                .WithMany(b => b.Preferences)
                .HasForeignKey(bc => bc.CustomerId);
            modelBuilder.Entity<CustomerPreference>()
                .HasOne(bc => bc.Preference)
                .WithMany()
                .HasForeignKey(bc => bc.PreferenceId);

            modelBuilder.Entity<PromoCodeCustomer>()
                .HasKey(bc => bc.Id);
            modelBuilder.Entity<PromoCodeCustomer>()
                .HasAlternateKey(bc => new { bc.CustomerId, bc.PromoCodeId });
            modelBuilder.Entity<PromoCodeCustomer>()
                .HasOne(bc => bc.Customer)
                .WithMany(b => b.PromoCodes)
                .HasForeignKey(bc => bc.CustomerId);
            modelBuilder.Entity<PromoCodeCustomer>()
                .HasOne(bc => bc.PromoCode)
                .WithMany(bc => bc.Customers)
                .HasForeignKey(bc => bc.PromoCodeId);
        }
    }
}