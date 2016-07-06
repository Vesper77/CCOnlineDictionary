using Microsoft.AspNet.Identity;
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
            var user = context.Users.FirstOrDefault(i => i.Id == id);
            if (user != null) {
                var viewModel = new EditUserViewModel(user);
                var role = UserManager.GetRoles(user.Id).FirstOrDefault();
                if (role != null) {
                    viewModel.Role = role;
                }      
                var data = context.ChildrenData.FirstOrDefault(d => d.ChildrenId == id);
                if (data != null) {
                    viewModel.ClassId = data.SchoolClassId;
                    data.ParentId = data.ParentId;
                }
                return View(viewModel);

            } else {
                return HttpNotFound();
            }

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
                    user.UserName = dUser.UserName;

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
                        var childrenData = context.ChildrenData.FirstOrDefault(c => c.ChildrenId == dUser.Id);
                        if (childrenData != null) {
                            childrenData.ParentId = dUser.ParentId;
                            childrenData.SchoolClassId = dUser.ClassId;
                        } else {
                            childrenData = new ChildrenData();
                            childrenData.ParentId = dUser.ParentId;
                            childrenData.SchoolClassId = dUser.ClassId;
                        }
                        context.SaveChanges();
                        UserManager.AddToRole(user.Id, dUser.Role);
                    }

                    if (dUser.Role == "teacher") {
                        var roles = UserManager.GetRoles(user.Id);
                        foreach (var r in roles) {
                            UserManager.RemoveFromRole(user.Id, r);
                        }
                        UserManager.AddToRole(user.Id, dUser.Role);
                        context.Lessons.Where(l => l.TeacherId == dUser.Id).ToList().ForEach(l => l.TeacherId = null);

                        if (dUser.LessonIds != null) {
                            for (int i = 0; i < dUser.LessonIds.Count(); i++) {
                                var lessonId = dUser.LessonIds.ToArray()[i];
                                var lesson = context.Lessons.FirstOrDefault(model => model.Id == lessonId);
                                if (lesson != null) {
                                    lesson.TeacherId = dUser.Id;
                                }
                            }
                        }
                    }
                    await UserManager.UpdateAsync(user);

                    if (!string.IsNullOrWhiteSpace(dUser.newPassword)) {
                        UserStore<DiaryUser> store = new UserStore<DiaryUser>();
                        PasswordHasher hasher = new PasswordHasher();
                        await store.SetPasswordHashAsync(user, hasher.HashPassword(dUser.newPassword));
                    }

                    context.SaveChanges();
                    
                }
            }
            dUser.newPassword = "";
            return View(dUser);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var dUser = context.Users.Find(id);
            if (dUser != null) {
                var role = UserManager.GetRoles(id).FirstOrDefault();
                if (role != null) {
                    UserManager.RemoveFromRoleAsync(id, role);
                    if (role == "parent") {
                        var parentData = context.ChildrenData.Where(i => i.ParentId == id).ToArray();
                        foreach (var p in parentData) {
                            p.ParentId = null;
                        }
                    }
                    if (role == "children") {
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
                    if (role == "teacher") {
                        var lessons = context.Lessons.Where(i => i.TeacherId == id).ToArray();
                        foreach (var lesson in lessons) {
                            lesson.TeacherId = null;
                        }

                    }
                }
                context.Users.Remove(dUser);
                try {
                    context.SaveChanges();
                } catch (Exception e) { }
                
                return RedirectToAction("Index");
            } else {
                return HttpNotFound();
            }
            
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
                var identLesson = context.Lessons.Where(x => x.Title.ToLower() == name.ToLower()).FirstOrDefault();
                if (identLesson == null) {
                    context.Lessons.Add(lesson);
                    context.SaveChanges();
                } else {
                    ModelState.AddModelError("", "Уже существует");
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
            return RedirectToAction("EditSchedule", new { classId = classId, day = day });
        }

        public ActionResult ListLessons(int page = 0) {
            var LessonsView = new ListLessonsViewModel();
            LessonsView.page = page;
            var lessons = context.Lessons.OrderBy(l => l.Title).Skip(page * ListLessonsViewModel.ITEMS_PER_PAGE).Take(ListLessonsViewModel.ITEMS_PER_PAGE).ToArray();
            LessonsView.Lessons = lessons;
            return View(LessonsView);
        }
        public ActionResult DeleteLesson(int Id) {
            var lesson = context.Lessons.FirstOrDefault(l => l.Id == Id);
            if (lesson != null) {

            }
            return RedirectToAction("ListLessons");
        }
        public ActionResult EditLesson(int Id) {
            var lesson = context.Lessons.FirstOrDefault(l => l.Id == Id);
            if (lesson != null) {
                var viewModel = new EditLessonViewModel();
                viewModel.Id = lesson.Id;
                viewModel.Title = lesson.Title;
                return View(viewModel);
            }
            return HttpNotFound();
        }
        //public ActionResult EditLesson(EditLessonViewModel lesson) { }

        public ActionResult CreateClass() {
            ClassCreateViewModel sch = new ClassCreateViewModel();
            return View(sch);
        }
        [HttpPost]
        public ActionResult CreateClass(ClassCreateViewModel sch) {
            SchoolClass sc = new SchoolClass();
            sc.Title = sch.Title;
            var schl = context.SchoolClasses.FirstOrDefault(c => c.Title == sch.Title);
            if (schl == null) {
                context.SchoolClasses.Add(sc);
                context.SaveChanges();
            } else {
                ModelState.AddModelError("", "Такой класс уже существует");
            }    
            return View(sch);
        }
        public ActionResult EditClass(int Id) 
        {
            var schoolClass = context.SchoolClasses.FirstOrDefault(s => s.Id == Id);
            if (schoolClass != null) 
            {
                var viewModel = new ClassEditViewModel();
                viewModel.Id = schoolClass.Id;
                viewModel.Title = schoolClass.Title;
                return View(viewModel);
            }
            return HttpNotFound();            
            
        }
        [HttpPost]
        public ActionResult EditClass(ClassEditViewModel viewModel) 
        {
            if (ModelState.IsValid)
            {
                var sch = context.SchoolClasses.FirstOrDefault(s => s.Id == viewModel.Id);
                if (sch != null) {
                    sch.Title = viewModel.Title;
                    context.SaveChanges();
                }
                return RedirectToAction("EditClass", new { Id = viewModel.Id });
            } 
            else
            {
                return View(viewModel);
            }
        }
        public ActionResult DeleteClass(int Id) {
            var schClass = context.SchoolClasses.FirstOrDefault(s => s.Id == Id);
            if (schClass != null) {
                context.SchoolClasses.Remove(schClass);
                context.ChildrenData.Where(c => c.SchoolClassId == schClass.Id).ToList().ForEach(
                    c => { c.SchoolClassId = 0; }
                );
                context.ScheduleLessons.Where(l => l.SchoolClassId == schClass.Id).ToList().ForEach(
                    l => { l.SchoolClassId = 0; }    
                );
                context.SaveChanges();
            }
            return RedirectToAction("ListClasses");
            
        }
        public ActionResult ListClasses(int page = 0) {
            var viewModel = new ClassListViewModel();
            var classes = context.SchoolClasses.OrderBy(x => x.Title).Skip(page * ClassListViewModel.PER_PAGE).Take(ClassListViewModel.PER_PAGE).ToArray();
            viewModel.Classes = classes;
            viewModel.Page = page;
            return View(viewModel);
        }

        protected override void Dispose(bool disposing)
        {
            context.Dispose();
            base.Dispose(disposing);
        }
    }
}
