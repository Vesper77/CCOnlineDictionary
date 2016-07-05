using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using OnlineDiary.DAL;
using OnlineDiary.Models;
using OnlineDiary.Models.Diary;
using OnlineDiary.Models.People;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OnlineDiary.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        private ApplicationUserManager _userManager = null;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ActionResult Index(int page = 0)
        {

            var viewModel = new EditUserViewModel();
            viewModel.page = page;
            if (viewModel == null)
            {
                return HttpNotFound();
            }
            return View(viewModel);
        }
        public ActionResult Create()
        {
            var viewModel = new EditUserViewModel();
            
            if (viewModel == null)
            {
                return HttpNotFound();
            }
            return View(viewModel);
        }
        public ActionResult Edit(string id)
        {
            var user = context.Users.Find(id);
            var viewModel = new EditUserViewModel(user);
            if (viewModel == null)
            {
                return HttpNotFound();
            }
            return View(viewModel);
        }
        public ActionResult Details(string id)
        {
            var user = context.Users.Find(id);
            var dUser = new EditUserViewModel(user);
            if (dUser == null)
            {
                return HttpNotFound();
            }
            return View(dUser);
        }
        public ActionResult Delete(string id)
        {
            var user = context.Users.Find(id);
            var dUser = new EditUserViewModel(user);
            if (dUser == null)
            {
                return HttpNotFound();
            }
            return View(dUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(EditUserViewModel dUser)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.CreateAsync(dUser.GetUser(), dUser.Password);

                if (result.Succeeded)
                {
                    var user = dUser.GetUser();
                    if (dUser.Role == "admin")
                    {
                        var roles = UserManager.GetRoles(user.Id);
                        foreach (var r in roles) {
                            UserManager.RemoveFromRole(user.Id,  r);
                        }
                        UserManager.AddToRole(user.Id, dUser.Role);
                    }

                    if (dUser.Role == "parent")
                    {
                        var roles = UserManager.GetRoles(user.Id);
                        foreach (var r in roles)
                        {
                            UserManager.RemoveFromRole(user.Id, r);
                        }
                        UserManager.AddToRole(user.Id, dUser.Role);
                    }

                    if (dUser.Role == "children")
                    {
                        var roles = UserManager.GetRoles(user.Id);
                        foreach (var r in roles)
                        {
                            UserManager.RemoveFromRole(user.Id, r);
                        }
                        UserManager.AddToRole(user.Id, dUser.Role);
                        ChildrenData data = new ChildrenData();
                        data.ChildrenId = user.Id;
                        data.ParentId = dUser.ParentId;
                        data.SchoolClassId = dUser.ClassId;
                        context.ChildrenData.Add(data);
                        
                    }

                    if (dUser.Role == "teacher")
                    {
                        var roles = UserManager.GetRoles(user.Id);
                        foreach (var r in roles)
                        {
                            UserManager.RemoveFromRole(user.Id, r);
                        }
                        UserManager.AddToRole(user.Id, dUser.Role);
                    }

                    context.SaveChanges();
                    return RedirectToAction("Details", new { id = dUser.GetUser().Id });
                }
            }
            return HttpNotFound();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditUserViewModel dUser)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(dUser.Id);
                if (user != null)
                {
                    user.FirstName = dUser.FirstName;
                    user.LastName = dUser.LastName;
                    user.ParentName = dUser.ParentName;
                    user.Email = dUser.Email;
                    user.PhoneNumber = dUser.PhoneNumber;

                    if (dUser.Role == "admin")
                    {
                        var roles = UserManager.GetRoles(user.Id);
                        foreach (var r in roles)
                        {
                            UserManager.RemoveFromRole(user.Id, r);
                        }
                        UserManager.AddToRole(user.Id, dUser.Role);
                    }

                    if (dUser.Role == "parent")
                    {
                        var roles = UserManager.GetRoles(user.Id);
                        foreach (var r in roles)
                        {
                            UserManager.RemoveFromRole(user.Id, r);
                        }
                        UserManager.AddToRole(user.Id, dUser.Role);
                    }

                    if (dUser.Role == "children")
                    {
                        context.ChildrenData.Where(c => c.ChildrenId == dUser.Id).ToArray()[0].ParentId = dUser.ParentId;
                        context.ChildrenData.Where(c => c.ChildrenId == dUser.Id).ToArray()[0].SchoolClassId = dUser.ClassId;
                        UserManager.AddToRole(user.Id, dUser.Role);
                        context.SaveChanges();
                    }

                    if (dUser.Role == "teacher")
                    {
                        var roles = UserManager.GetRoles(user.Id);
                        foreach (var r in roles)
                        {
                            UserManager.RemoveFromRole(user.Id, r);
                        }
                        UserManager.AddToRole(user.Id, dUser.Role);
                        for (int i = 0; i < context.Lessons.ToArray().Count(); i++)
                        {
                            if (context.Lessons.ToArray()[i].TeacherId == dUser.Id)
                            {
                                context.Lessons.ToArray()[i].TeacherId = null;
                            }
                        }

                        for (int i = 0; i < dUser.LessonIds.Count(); i++)
                        {
                            var lessonId = dUser.LessonIds.ToArray()[i];
                            context.Lessons.Where(model => model.Id == lessonId).ToArray()[0].TeacherId = dUser.Id;
                        }                        

                    }
                    await UserManager.UpdateAsync(user);

                    if (dUser.Password != null && dUser.Password.Length > 0)
                    {
                        var result = await UserManager.AddPasswordAsync(dUser.Id, dUser.Password);
                        if (!result.Succeeded)
                        {
                            //TODO : Add notif error
                        }
                    }

                    context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(dUser);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var dUser = context.Users.Find(id);
            context.Users.Remove(dUser);
            context.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<ActionResult> CreateLesson()
        {
            var model = new LessonViewModel();
            model.Teachers = model.GetAllTeachers();
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> CreateLesson(string teacherId, string name)
        {
            if (!String.IsNullOrWhiteSpace(name))
            {
                var lesson = new Lesson() { TeacherId = teacherId, Title = name };
                var identLesson = context.Lessons.Where(x => x.TeacherId == teacherId && x.Title.
                                                       ToLower() == name.ToLower()).FirstOrDefault();
                if (identLesson == null)
                {
                    context.Lessons.Add(lesson);
                    context.SaveChanges();
                }
            }
            return await CreateLesson();
        }
        [HttpGet]
        public async Task<ActionResult> SelectClass()
        {
            EditScheduleViewModel model = new EditScheduleViewModel();
            model.AllClasses = model.GetAllClasses();
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> SelectClass(int classId, int day)
        {
            return await EditSchedule(classId, day);
        }
        [HttpGet]
        public async Task<ActionResult> EditSchedule(int classId, int day)
        {
            EditScheduleViewModel model = new EditScheduleViewModel();
            model.classiD = classId;
            model.Day = day;
            var l = model.GetCurrentLesson(model.classiD, 1);
            model.Lessons = model.GetAllLessons();
            ViewBag.ClassName = context.SchoolClasses.Where(x => x.Id == classId).First().Title;
            return View("EditSchedule", model);
        }
        [HttpPost]
        public async Task<ActionResult> EditSchedule(int classId, int day, int[] lesson)
        {
            EditScheduleViewModel model = new EditScheduleViewModel();
            model.classiD = classId;
            model.Day = day;
            model.EditSchedule(lesson);
            return RedirectToAction("SelectClass", "Admin");
        }
        protected override void Dispose(bool disposing)
        {
            context.Dispose();
            base.Dispose(disposing);
        }
        /// <summary>
        /// Возвращает все пользователей из таблицы User
        /// </summary>
        /// <returns></returns>
        public List<EditUserViewModel> getAllUsers()
        {
            var users = new List<EditUserViewModel>();

            foreach (var user in context.Users)
            {
                var r = new EditUserViewModel(user);
                users.Add(r);
            }
            return users;
        }
    }
}
