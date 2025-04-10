﻿using Domin.Entity;
using Infarstuructre.Data;
using Infarstuructre.IRepository;
using Infarstuructre.IRepository.ServicesRepository;
using Infarstuructre.Seeds;
using Infarstuructre.ViewModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MyService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var context = scope.ServiceProvider.GetRequiredService<MyServiceDbContext>();
            context.Database.EnsureCreated();
           

            
            try
            {
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                if (!context.services.Any())
                {
                    context.services.Add(new Service { Name = "Sabak", CreatedAt = DateTime.Now, Description="This is Avalible Service" });
                    context.services.Add(new Service { Name = "electrocity" ,CreatedAt=DateTime.Now, Description = "This is Avalible Service" });
                    context.SaveChanges();
                }
                

                await DefaultRole.SeedAsync(roleManager);
                await DefaultUser.SeedSuperAdminAsync(userManager, roleManager);
                await DefaultUser.SeedBasicUserAsync(userManager, roleManager);
            }
            catch (Exception) { throw; }

            host.Run();
        }


        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddLocalization(options => options.ResourcesPath = "Resource");
            services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);



            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new List<CultureInfo>
        {
            new CultureInfo("en-US"),
            new CultureInfo("ar-SA")
        };

                // تحديد اللغة الافتراضية
                options.DefaultRequestCulture = new RequestCulture("en-US");
                // تحديد اللغات المدعومة على مستوى واجهة المستخدم والبيانات
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                // يمكنك أيضًا استخدام CookieRequestCultureProvider للسماح بحفظ اختيار اللغة لدى المستخدم
                options.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());
            });
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddSession();
            services.AddDbContext<MyServiceDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("BookConnection")));
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequiredLength = 5;
                options.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<MyServiceDbContext>();
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Admin";
                options.AccessDeniedPath = "/Admin/Home/Denied";
            });

            services.AddScoped<IServicesRepository<Category>, ServicesCategory>();
            services.AddScoped<IServicesRepositoryLog<LogCategory>, ServicesLogCategory>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<RequestLocalizationOptions> localizationOptions)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // تفعيل Middleware الخاص بالتوطين باستخدام الإعدادات السابقة
            app.UseRequestLocalization(localizationOptions.Value);

            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Accounts}/{action=Login}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
