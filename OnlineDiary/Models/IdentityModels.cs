using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OnlineDiary.Models.Diary;
using System.Data.Entity.ModelConfiguration.Conventions;
using OnlineDiary.Models.People;

namespace OnlineDiary.Models
{
    public class DiaryUser : IdentityUser
    {        
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<DiaryUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<DiaryUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<SchoolClass> SchoolClasses { get; set; }
        public DbSet<Mark> Marks { get; set; }
        public DbSet<ScheduleLesson> ScheduleLessons { get; set; }
        public DbSet<ChildrenData> ChildrenData { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<DiaryUser>().ToTable("User");
            modelBuilder.Entity<IdentityRole>().ToTable("Role");
            modelBuilder.Entity<IdentityUserRole>().ToTable("UserRole");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaim");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogin");
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}