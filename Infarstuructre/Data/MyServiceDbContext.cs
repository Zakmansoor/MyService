using Domin.Entity;
using Infarstuructre.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Infarstuructre.Data
{
   public class MyServiceDbContext :IdentityDbContext<ApplicationUser>
    {
       
        public MyServiceDbContext(DbContextOptions<MyServiceDbContext> options ) : base (options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

        base.OnModelCreating(builder);
            builder.Entity<Service>().HasData(
                new Service
                {
                    ServiceID = 1,
                    Name = "Electrical Maintenance",
                    NameAr = "صيانة كهربائية",
                    Description = "إصلاح الأعطال الكهربائية وتركيب الأنظمة",
                    CreatedAt = DateTime.Now,
                    IsActive = true
                },
                new Service
                {
                    ServiceID = 2,
                    Name = "Plumbing",
                    NameAr = "سباكة",
                    Description = "إصلاح تسربات المياه وتركيب المواسير",
                    CreatedAt = DateTime.Now,
                    IsActive = true
                },
                new Service
                {
                    ServiceID = 3,
                    Name = "AC Maintenance",
                    NameAr = "صيانة تكييف",
                    Description = "تنظيف وصيانة أنظمة التبريد",
                    CreatedAt = DateTime.Now,
                    IsActive = true
                },
                new Service
                {
                    ServiceID = 4,
                    Name = "Home Appliances",
                    NameAr = "أجهزة منزلية",
                    Description = "إصلاح الغسالات والثلاجات",
                    CreatedAt = DateTime.Now,
                    IsActive = true
                },
                new Service
                {
                    ServiceID = 5,
                    Name = "Painting",
                    NameAr = "دهان",
                    Description = "خدمات الدهان الداخلي والخارجي",
                    CreatedAt = DateTime.Now,
                    IsActive = true
                },
                new Service
                {
                    ServiceID = 6,
                    Name = "Carpentry",
                    NameAr = "نجارة",
                    Description = "تصليح الأثاث وتركيب الأرضيات",
                    CreatedAt = DateTime.Now,
                    IsActive = true
                },
                new Service
                {
                    ServiceID = 7,
                    Name = "Glass Installation",
                    NameAr = "تركيب زجاج",
                    Description = "إصلاح النوافذ المكسورة",
                    CreatedAt = DateTime.Now,
                    IsActive = true
                },
                new Service
                {
                    ServiceID = 8,
                    Name = "Tiling",
                    NameAr = "تركيب بلاط",
                    Description = "تركيب البلاط الجديد وإصلاح التالف",
                    CreatedAt = DateTime.Now,
                    IsActive = true
                }, 
                new Service
                {
                    ServiceID = 10,
                    Name = "Cleaning Services",
                    NameAr = "خدمات تنظيف",
                    Description = "تنظيف المنازل والمكاتب",
                    CreatedAt = DateTime.Now,
                    IsActive = true
                }
);
builder.Entity<Employees>()
    .HasKey(e => e.Id);

            builder.Entity<Employees>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();
            builder.Entity<Request>()
        .HasOne(r => r.Mapping)
        .WithOne(l => l.Request)
        .HasForeignKey<Mapping>(l => l.RequestId);

            builder.Entity<Category>().Property(x => x.Id).HasDefaultValueSql("(newid())");
            builder.Entity<LogCategory>().Property(x => x.Id).HasDefaultValueSql("(newid())");
            builder.Entity<SubCategory>().Property(x => x.Id).HasDefaultValueSql("(newid())");
            builder.Entity<LogSubCategory>().Property(x => x.Id).HasDefaultValueSql("(newid())");
            builder.Entity<LogBook>().Property(x => x.Id).HasDefaultValueSql("(newid())");
            //builder.Entity<VwUser>().HasNoKey().ToView("VwUsers");
            builder.Entity<VwUser>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("VwUsers");
            });
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<note> notes { get; set; }
        public DbSet<historypaid> historypaids { get; set; }
        public DbSet<LogCategory> LogCategories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<LogSubCategory> LogSubCategories { get; set; }
        public DbSet<Paid> paids { get; set; }
        public DbSet<LogBook> LogBooks { get; set; }
        public DbSet<VwUser> VwUsers { get; set; }
        public DbSet<EmployeeRequest> employeeRequests { get; set; }
        public DbSet<Employees> employees { get; set; }
        public DbSet<Customer> customers { get; set; }
        public DbSet<Mapping> Mappings { get; set; }
        public DbSet<Notification> notifications { get; set; }
        public DbSet<Request> requests { get; set; }
        public DbSet<Service> services { get; set; }
        public DbSet<Contact> contacts { get; set; }
    }
}
