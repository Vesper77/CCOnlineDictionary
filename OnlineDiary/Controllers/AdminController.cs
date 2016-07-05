﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using OnlineDiary.DAL;
using OnlineDiary.Models;
using OnlineDiary.Models.CRUDViewModels;
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
            var viewModel = new IndexUserViewModel();
            viewModel.page = page;
            viewModel.Role = "all";
            if (viewModel == null)
            {
                return HttpNotFound();
            }
            return View(viewModel);
        }
        public ActionResult Create()
        {
            var viewModel = new CreateUserViewModel();
            
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
            var userRoleId = context.Users.Where(i => i.Id == viewModel.Id).ToArray()[0].Roles.ToArray()[0].RoleId;
            var roleName = context.Roles.Where(i => i.Id == userRoleId).ToArray()[0].Name;
            viewModel.Role = roleName;
            if (viewModel == null)
            {
                return HttpNotFound();
            }
            return View(viewModel);
        }
        public ActionResult Details(string id)
        {
            var user = context.Users.Find(id);
            var dUser = new DetailUserViewModel(user);
            if (dUser == null)
            {
                return HttpNotFound();
            }
            return View(dUser);
        }
        public ActionResult Delete(string id)
        {
            var user = context.Users.Find(id);
            var dUser = new DeleteUserViewModel(user);
            if (dUser == null)
            {
                return HttpNotFound();
            }
            return View(dUser);
        }
        [HttpPost]
        public ActionResult Index(IndexUserViewModel dUser)
        {
            return View(dUser);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateUserViewModel dUser)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.CreateAsync(dUser.GetUser(), dUser.Password);

                if (result.Succeeded) {
                    var user = dUser.GetUser();
                    if (dUser.Role == "admin") {
                        var roles = UserManager.GetRoles(user.Id);
                        foreach (var r in roles) {
                            UserManager.RemoveFromRole(user.Id, r);
                        }
                        UserManager.AddToRole(user.Id, dUser.Role);
                    }

                    if (dUser.Role == "parent") {
                        var roles = UserManager.GetRoles(user.Id);
                        foreach (var r in roles) {
                            UserManager.RemoveFromRole(user.Id, r);
                        }
                        UserManager.AddToRole(user.Id, dUser.Role);
                    }

                    if (dUser.Role == "children") {
                        var roles = UserManager.GetRoles(user.Id);
                        foreach (var r in roles) {
                            UserManager.RemoveFromRole(user.Id, r);
                        }
                        UserManager.AddToRole(user.Id, dUser.Role);
                        ChildrenData data = new ChildrenData();
                        data.ChildrenId = user.Id;
                        data.ParentId = dUser.ParentId;
                        data.SchoolClassId = dUser.ClassId;
                        context.ChildrenData.Add(data);

                    }

                    if (dUser.Role == "teacher") {
                        var roles = UserManager.GetRoles(user.Id);
                        foreach (var r in roles) {
                            UserManager.RemoveFromRole(user.Id, r);
                        }
                        UserManager.AddToRole(user.Id, dUser.Role);
                    }

                    context.SaveChanges();
                    return RedirectToAction("Details", new { id = dUser.GetUser().Id });
                } else {
                    foreach (var er in result.Errors) {
                        ModelState.AddModelError("",er);
                    }
                    return View(dUser);
                }
            }
            return View(dUser);
            //return HttpNotFound();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditUserViewModel dUser)
        {

            if (ModelState.IsValid) {
                var user = await UserManager.FindByIdAsync(dUser.Id);
                if (user != null) {
                    user.FirstName = dUser.FirstName;
                    user.LastName = dUser.LastName;
                    user.ParentName = dUser.ParentName;
                    user.Email = dUser.Email;

                    if (dUser.Role == "admin") {
                        var roles = UserManager.GetRoles(user.Id);
                        foreach (var r in roles) {
                            UserManager.RemoveFromRole(user.Id, r);
                        }
                        UserManager.AddToRole(user.Id, dUser.Role);
                    }

                    if (dUser.Role == "parent") {
                        var roles = UserManager.GetRoles(user.Id);
                        foreach (var r in roles) {
                            UserManager.RemoveFromRole(user.Id, r);
                        }
                        UserManager.AddToRole(user.Id, dUser.Role);
                    }

                    if (dUser.Role == "children") {
                        context.ChildrenData.Where(c => c.ChildrenId == dUser.Id).ToArray()[0].ParentId = dUser.ParentId;
                        context.ChildrenData.Where(c => c.ChildrenId == dUser.Id).ToArray()[0].SchoolClassId = dUser.ClassId;
                        UserManager.AddToRole(user.Id, dUser.Role);
                        context.SaveChanges();
                    }

                    if (dUser.Role == "teacher") {
                        var roles = UserManager.GetRoles(user.Id);
                        foreach (var r in roles) {
                            UserManager.RemoveFromRole(user.Id, r);
                        }
                        UserManager.AddToRole(user.Id, dUser.Role);
                        context.Lessons.Where(l => l.TeacherId == dUser.Id).ToList().ForEach(l => l.TeacherId = null);
                        //for (int i = 0; i < context.Lessons.ToArray().Count(); i++)
                        //{
                        //    if (context.Lessons.ToArray()[i].TeacherId == dUser.Id)
                        //    {
                        //        context.Lessons.ToArray()[i].TeacherId = null;
                        //    }
                        //}

                        for (int i = 0; i < dUser.LessonIds.Count(); i++) {
                            var lessonId = dUser.LessonIds.ToArray()[i];
                            context.Lessons.Where(model => model.Id == lessonId).ToArray()[0].TeacherId = dUser.Id;
                        }

                    }
                    await UserManager.UpdateAsync(user);

                    if (!string.IsNullOrWhiteSpace(dUser.newPassword)) {
                        UserStore<DiaryUser> store = new UserStore<DiaryUser>();
                        PasswordHasher hasher = new PasswordHasher();
                        await store.SetPasswordHashAsync(user, hasher.HashPassword(dUser.newPassword));
                        //var result = await UserManager.se(dUser.Id, dUser.newPassword);
                        //if (!result.Succeeded)

                        //ModelState.AddModelError("","Неверный пароль");
                        //}
                    }

                    context.SaveChanges();
                }
            } 
            return View(dUser);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var dUser = context.Users.Find(id);
            var userRole = context.Users.Where(i => i.Id == dUser.Id).ToArray()[0].Roles.ToArray()[0];
            var roleName = context.Roles.Where(i => i.Id == userRole.RoleId).ToArray()[0].Name;
            UserManager.RemoveFromRoleAsync(id, roleName);
            if (roleName == "parent")
            {
                var parentData = context.ChildrenData.Where(i => i.ParentId == id).ToArray();
                foreach (var p in parentData)
                {
                    p.ParentId = null;
                }
            }
            if (roleName == "children")
            {
                var children = context.ChildrenData.Where(i => i.ChildrenId == id).FirstOrDefault();
                if (children != null) {
                    context.ChildrenData.Remove(children);
                    var childrenTruancys = context.Truancys.Where(i => i.ChildrenId == id).ToArray();
                    foreach (var t in childrenTruancys) {
                        context.Truancys.Remove(t);
                    }
                    var childrenMarks = context.Marks.Where(i => i.ChildrenId == id).ToArray();
                    foreach (var m in childrenMarks) {
                        context.Marks.Remove(m);
                    }
                    var childrenFinalMarks = context.FinalMarks.Where(i => i.ChildrenId == id).ToArray();
                    foreach (var fm in childrenFinalMarks) {
                        context.FinalMarks.Remove(fm);
                    }
                }   
            }
            if (roleName == "teacher")
            {
                var lessons = context.Lessons.Where(i => i.TeacherId == id).ToArray();
                foreach (var lesson in lessons)
                {
                    lesson.TeacherId = null;
                }

            }
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
            if (!String.IsNullOrWhiteSpace(name)) {
                var lesson = new Lesson() { TeacherId = teacherId, Title = name };
                var identLesson = context.Lessons.Where(x => x.TeacherId == teacherId && x.Title.
                                                       ToLower() == name.ToLower()).FirstOrDefault();
                if (identLesson == null) {
                    context.Lessons.Add(lesson);
                    context.SaveChanges();
                }
            } else {
                ModelState.AddModelError("name", "Пустое имя");
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
            return await EditSchedule(classId, day);
        }
        protected override void Dispose(bool disposing)
        {
            context.Dispose();
            base.Dispose(disposing);
        }
    }
}
