using Microsoft.AspNet.Identity;
using OnlineDiary.Models;
using OnlineDiary.Models.Diary;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using OnlineDiary.Controllers;

namespace OnlineDiary.DAL
{
    public class ApplicationDBInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        public override void InitializeDatabase(ApplicationDbContext context)
        {
            base.InitializeDatabase(context);
        }
        protected override void Seed(ApplicationDbContext context) {
            //Add roles
            string[] roles = { "admin", "children", "teacher", "parent" };
            foreach (var r in roles) {
                context.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole() { Name = r });
            }
            // Add Lessons
            var lessons = new List<Lesson> {
                new Lesson { Title = "Test" }
            };
            lessons.ForEach(l => context.Lessons.Add(l));

            // Add schoolClasses
            var schoolClasses = new List<SchoolClass> {
                new SchoolClass { Title = "TestClass"}
            };
            schoolClasses.ForEach(sch => context.SchoolClasses.Add(sch));
        }
    }
}
