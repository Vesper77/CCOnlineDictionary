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
using Microsoft.AspNet.Identity.EntityFramework;

namespace OnlineDiary.DAL
{
    public class ApplicationDBInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        public override void InitializeDatabase(ApplicationDbContext context)
        {
            base.InitializeDatabase(context);
        }
        protected override void Seed(ApplicationDbContext context) {
            //Roles
            string[] roles = {
                "admin", "teacher", "parent", "children"
            };
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            foreach (var r in roles) {
                roleManager.Create(new IdentityRole(r));
            }
            //Users
            ApplicationUserManager manager = new ApplicationUserManager(new UserStore<DiaryUser>(context));
            string password = "123qwe";
            var user = new DiaryUser()
            {
                Email = "children@admin.com",
                UserName = "children@admin.com"
            };
            manager.Create(user, password);

            manager.AddToRole(user.Id, "children");
            var teacher = new DiaryUser()
            {
                Email = "teacher@admin.com",
                UserName = "teacher@admin.com"
            };
            manager.Create(teacher, password);
            manager.AddToRole(teacher.Id, "teacher");
            var parent = new DiaryUser()
            {
                Email = "parent@admin.com",
                UserName = "parent@admin.com"
            };
            manager.Create(parent, password);
            manager.AddToRole(parent.Id, "parent");

            var admin = new DiaryUser()
            {
                Email = "admin@admin.com",
                UserName = "admin@admin.com"
            };
            manager.Create(admin, password);
            manager.AddToRole(admin.Id, "admin");

            // Add Lessons
            var lesson = new Lesson { Title = "Lesson#1", TeacherId = teacher.Id };
            var lesson1 = new Lesson { Title = "Lesson#2", TeacherId = teacher.Id };
            var lesson2 = new Lesson { Title = "Lesson#3", TeacherId = teacher.Id };

            context.Lessons.Add(lesson);
            context.Lessons.Add(lesson1);
            context.Lessons.Add(lesson2);
            //Add schooles
            var sch = new SchoolClass { Title = "Class#1" };
            var sch1 = new SchoolClass { Title = "Class#2" };
            context.SchoolClasses.Add(sch);
            context.SchoolClasses.Add(sch1);
            //Children Data (withoud SchoolClass)
            context.ChildrenData.Add(new Models.People.ChildrenData() { ChildrenId = user.Id, ParentId = parent.Id, SchoolClassId = 1 });
            //Schelude lesonse 
            context.ScheduleLessons.Add(new ScheduleLesson() {LessonId = lesson.Id, SchoolClassId = sch.Id, Order = 2, DayNumber=2});
            context.ScheduleLessons.Add(new ScheduleLesson() {LessonId = lesson1.Id, SchoolClassId = sch.Id, Order = 1, DayNumber=1 });

            context.SaveChanges();
        }
    }
}
