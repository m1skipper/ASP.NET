using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PromoCodeFactory.WebHost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // Очистим базу данных и зальем начальные данные из FakeDataFactory
            using (var scope = host.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                db.Database.EnsureDeletedAsync();
                db.Database.Migrate();
                Seed(scope.ServiceProvider);
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });

        public static void Seed(IServiceProvider serviceProvider)
        {
            // Зачем создавать ещё один вложенный scope??
            //using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            //{
            // var context = scope.ServiceProvider.GetService<DataContext>();
            var dataContext = serviceProvider.GetService<DataContext>();
            var mapper = serviceProvider.GetService<IMapper>();

            foreach (BaseEntity ent in FakeDataFactory.Preferences)
                dataContext.Add(ent);
            foreach (BaseEntity ent in FakeDataFactory.Roles)
                dataContext.Add(ent);
            foreach (Employee ent in FakeDataFactory.Employees)
            {
                // Копирование не работает, возвращает ту же entity.
                // Поэтому мы подменяем хранилище в памяти(((. Правда на такие же объекты)))
                var newEnt = mapper.Map<Employee, Employee>(ent);
                ent.Role = dataContext.Roles.Local.AsQueryable().FirstOrDefault(r=>r.Id == ent.Role.Id);
                dataContext.Add(ent);
            }
            foreach (Customer ent in FakeDataFactory.Customers)
            {
                // Копирование не работает, возвращает ту же entity.
                // Поэтому мы подменяем хранилище в памяти(((. Правда на такие же объекты)))
                var newEnt = mapper.Map<Customer, Customer>(ent);
                var oldPrefs = ent.Preferences;
                var prefIds = ent.Preferences.Select(p => p.Id).ToList();
                ent.Preferences = new List<Preference>() { };
                Customer attached = dataContext.Add(ent).Entity;
                foreach (var id in prefIds)
                    attached.Preferences.Add(dataContext.Preferences.Local.FirstOrDefault(p => p.Id == id));
            }

            dataContext.SaveChanges();
        }

    }
}