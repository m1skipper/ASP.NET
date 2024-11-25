using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using Microsoft.EntityFrameworkCore.Proxies;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PromoCodeFactory.DataAccess.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {            
        }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Preference> Preferences { get; set; }

        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            const int MAX_STRING_LENGTH = 1024;

            base.OnModelCreating(modelBuilder);

            // Employe
            modelBuilder.Entity<Employee>()
                       .HasOne(c => c.Role);
            modelBuilder.Entity<Employee>().Property(p => p.FirstName).HasMaxLength(MAX_STRING_LENGTH);
            modelBuilder.Entity<Employee>().Property(p => p.LastName).HasMaxLength(MAX_STRING_LENGTH);
            modelBuilder.Entity<Employee>().Property(p => p.Email).HasMaxLength(MAX_STRING_LENGTH);

            // Role
            modelBuilder.Entity<Role>();
            modelBuilder.Entity<Role>().Property(p => p.Name).HasMaxLength(MAX_STRING_LENGTH);
            modelBuilder.Entity<Role>().Property(p => p.Description).HasMaxLength(MAX_STRING_LENGTH);

            // Preference
            modelBuilder.Entity<Preference>();
            modelBuilder.Entity<Preference>().Property(p => p.Name).HasMaxLength(MAX_STRING_LENGTH);

            // PromoCode
            modelBuilder.Entity<PromoCode>()
                .HasOne(c => c.Preference);
            modelBuilder.Entity<PromoCode>()
                .HasOne(c => c.PartnerManager);
            modelBuilder.Entity<PromoCode>().Property(p => p.Code).HasMaxLength(MAX_STRING_LENGTH);
            modelBuilder.Entity<PromoCode>().Property(p => p.ServiceInfo).HasMaxLength(MAX_STRING_LENGTH);
            modelBuilder.Entity<PromoCode>().Property(p => p.PartnerName).HasMaxLength(MAX_STRING_LENGTH);

            // Customer
            // Promocode реализовать через One-To-Many,
            // будем считать, что в данном примере промокод может быть выдан только одному клиенту из базы.
            // Если customer удаляем удаляем и выписанные для него промокоды
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.PromoCodes).WithOne().IsRequired();

            modelBuilder.Entity<Customer>().Property(p => p.FirstName).HasMaxLength(MAX_STRING_LENGTH);
            modelBuilder.Entity<Customer>().Property(p => p.LastName).HasMaxLength(MAX_STRING_LENGTH);
            modelBuilder.Entity<Customer>().Property(p => p.Email).HasMaxLength(MAX_STRING_LENGTH);

            // Многие ко многим через таблицу CustomerPreference
            modelBuilder.Entity<Customer>()
                            .HasMany(c => c.Preferences)
                            .WithMany()
                            .UsingEntity<CustomerPreference>();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                // .UseLazyLoadingProxies() в итоге сделал через "Explicit loading" 
                .UseSqlite("Data Source=promocode.db");

            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
        }
    }
}
