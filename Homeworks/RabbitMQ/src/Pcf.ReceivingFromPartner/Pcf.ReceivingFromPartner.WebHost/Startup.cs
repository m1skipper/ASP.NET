using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using Pcf.ReceivingFromPartner.Core.Abstractions.Repositories;
using Pcf.ReceivingFromPartner.Core.Abstractions.Gateways;
using Pcf.ReceivingFromPartner.DataAccess;
using Pcf.ReceivingFromPartner.DataAccess.Repositories;
using Pcf.ReceivingFromPartner.DataAccess.Data;
using MassTransit;
using Pcf.ReceivingFromPartner.Integration;

namespace Pcf.ReceivingFromPartner.WebHost
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddMvcOptions(x =>
                x.SuppressAsyncSuffixInActionNames = false);
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            
            services.AddScoped<IDbInitializer, EfDbInitializer>();

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    //cfg.Host("rabbitmq://localhost:15672/", h =>
                    //    {
                    //    h.Username("admin");
                    //    h.Password("docker");
                    //});
                    cfg.Host("localhost", h =>
                    {
                        h.Username("admin");
                        h.Password("docker");
                    });
                });
            });
            services.Configure<MassTransitHostOptions>(options =>
            {
                options.WaitUntilStarted = true;
                options.StartTimeout = TimeSpan.FromSeconds(30);
                options.StopTimeout = TimeSpan.FromMinutes(1);
            });

            // 1.
            services.AddScoped(typeof(IGivingPromoCodeToCustomerGateway), typeof(GivingPromoCodeToCustomerMassTransitGateway));
            //services.AddHttpClient<IGivingPromoCodeToCustomerGateway, GivingPromoCodeToCustomerGateway>(c =>
            //{
            //    c.BaseAddress = new Uri(Configuration["IntegrationSettings:GivingToCustomerApiUrl"]);
            //});

            // 2.
            //services.AddScoped(typeof(IAdministrationGateway), typeof(AdministrationMasstransitGateway));
            services.AddHttpClient<IAdministrationGateway, AdministrationGateway>(c =>
            {
                c.BaseAddress = new Uri(Configuration["IntegrationSettings:AdministrationApiUrl"]);
            });

            // 3.
            //services.AddScoped<INotificationGateway, NotificationMasstransitGateway>();
            services.AddScoped<INotificationGateway, NotificationGateway>();

            services.AddDbContext<DataContext>(x =>
            {
                //x.UseSqlite("Filename=PromocodeFactoryReceivingFromPartnerDb.sqlite");
                x.UseNpgsql(Configuration.GetConnectionString("PromocodeFactoryReceivingFromPartnerDb"));
                x.UseSnakeCaseNamingConvention();
                x.UseLazyLoadingProxies();
            });

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.AddOpenApiDocument(options =>
            {
                options.Title = "PromoCode Factory Receiving From Partner API Doc";
                options.Version = "1.0";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbInitializer dbInitializer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseOpenApi();
            app.UseSwaggerUi(x =>
            {
                x.DocExpansion = "list";
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            dbInitializer.InitializeDb();
        }
    }
}