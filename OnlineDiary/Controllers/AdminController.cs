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
            return View(dUser);
            //return HttpNotFound();
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
                    //user.PhoneNumber = dUser.PhoneNumber;

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

                    if (dUser.newPassword != null && dUser.newPassword.Length > 0)
                    {
                        var result = await UserManager.AddPasswordAsync(dUser.Id, dUser.newPassword);
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
                var children = context.ChildrenData.Where(i => i.ChildrenId == id).ToArray()[0];
                context.ChildrenData.Remove(children);
                var childrenTruancys = context.Truancys.Where(i => i.ChildrenId == id).ToArray();
                foreach (var t in childrenTruancys)
                {
                    context.Truancys.Remove(t);
                }
                var childrenMarks = context.Marks.Where(i => i.ChildrenId == id).ToArray();
                foreach (var m in childrenMarks)
                {
                    context.Marks.Remove(m);
                }
                var childrenFinalMarks = context.FinalMarks.Where(i => i.ChildrenId == id).ToArray();
                foreach (var fm in childrenFinalMarks)
                {
                    context.FinalMarks.Remove(fm);
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

        public ActionResult QuadMester()
        {
            var qMester = new List<QuadMesterViewModel>();
            qMester.Add(new QuadMesterViewModel());
            qMester.Add(new QuadMesterViewModel());
            qMester.Add(new QuadMesterViewModel());
            qMester.Add(new QuadMesterViewModel());
            return View(qMester);
        }
        [HttpPost]
        public ActionResult QuadMester(List<QuadMesterViewModel> viewQMest)
        {
            if (viewQMest != null)
            {
                for (int i = 0; i < viewQMest.Count; i++)
                {
                    if (viewQMest[i].StartDate.ToShortDateString() != "01.01.0001" && viewQMest[i].EndDate.ToShortDateString() != "01.01.0001")
                    {
                        var qMester = new Quadmester();
                        qMester.StartDate = viewQMest[i].StartDate;
                        qMester.EndDate = viewQMest[i].EndDate;
                        qMester.Number = i + 1;
                        context.Quadmesters.Add(qMester);
                    }
                }
                context.SaveChanges();
            }
            return View(viewQMest);
        }
        [HttpPost]
        public async Task<ActionResult> CreateLesson(string teacherId, string name)
        {
            if (!String.IsNullOrWhiteSpace(name))
            {
                var lesson = new Lesson() { TeacherId = teacherId, Title = name };
                var identLesson = context.Lessons.Where(x => x.Title.ToLower() == name.ToLower()).FirstOrDefault();
                if (identLesson == null)
                {
                    context.Lessons.Add(lesson);
                    context.SaveChanges();
                } else
                {
                    ModelState.AddModelError("", "Уже существует");
                }
            } else
            {
                ModelState.AddModelError("name", "Пустое имя");
            }
            return await CreateLesson();
        }

        public ActionResult ListLessons(int page = 0)
        {
            var LessonsView = new ListLessonsViewModel();
            LessonsView.page = page;
            var lessons = context.Lessons.OrderBy(l => l.Title).Skip(page * ListLessonsViewModel.ITEMS_PER_PAGE).Take(ListLessonsViewModel.ITEMS_PER_PAGE).ToArray();
            LessonsView.Lessons = lessons;
            LessonsView.PageCount = (int)Math.Floor((float)context.Lessons.Count() / (float)ListLessonsViewModel.ITEMS_PER_PAGE);
            return View(LessonsView);
        }
        public ActionResult DeleteLesson(int Id)
        {
            var lesson = context.Lessons.FirstOrDefault(l => l.Id == Id);
            if (lesson != null)
            {
                var schLesson = context.ScheduleLessons.FirstOrDefault(s => s.LessonId == lesson.Id);
                if (schLesson != null)
                {
                    var homeWorks = context.HomeWorks.Where(h => h.ScheludeLessonId == schLesson.Id).ToArray();
                    var completedHomeWorks = context.CompltetedHomeWorks.Where(c => homeWorks.Any(h => h.Id == c.HomeWorkId)).ToArray();
                    context.HomeWorks.RemoveRange(homeWorks);
                    context.CompltetedHomeWorks.RemoveRange(completedHomeWorks);
                    context.ScheduleLessons.Remove(schLesson);
                }
                var marks = context.Marks.Where(m => m.Lesson.Id == lesson.Id).ToArray();
                var truancys = context.Truancys.Where(t => t.LessonId == lesson.Id);
                context.Marks.RemoveRange(marks);
                context.Truancys.RemoveRange(truancys);
                context.Lessons.Remove(lesson);
            }
            return RedirectToAction("ListLessons");
        }
        public ActionResult EditLesson(int Id)
        {
            var lesson = context.Lessons.FirstOrDefault(l => l.Id == Id);
            if (lesson != null)
            {
                var viewModel = new EditLessonViewModel();
                viewModel.Id = lesson.Id;
                viewModel.Title = lesson.Title;
                viewModel.TeacherId = lesson.TeacherId;
                var teacherRoleId = context.Roles.FirstOrDefault(r => r.Name == "teacher");
                if (teacherRoleId != null)
                {

                    viewModel.Teachers = context.Users.Where(u => u.Roles.Any(r => r.RoleId == teacherRoleId.Id)).ToDictionary(x => x.Id, y => y.FirstName + " " + y.LastName + " " + y.ParentName);
                    return View(viewModel);
                }
            }
            return HttpNotFound();
        }
        [HttpPost]
        public ActionResult EditLesson(EditLessonViewModel viewModel)
        {
            if (viewModel != null)
            {
                if (ModelState.IsValid)
                {
                    var lesson = context.Lessons.FirstOrDefault(l => l.Id == viewModel.Id);
                    if (lesson != null)
                    {
                        lesson.TeacherId = viewModel.TeacherId;
                        lesson.Title = viewModel.Title;
                        context.SaveChanges();
                        return RedirectToAction("EditLesson", new { Id = lesson.Id });
                    } else
                    {
                        return HttpNotFound();
                    }
                } else
                {
                    return View(viewModel);
                }
            }
            return RedirectToAction("LessonList");
        }
        public ActionResult CreateClass()
        {
            ClassCreateViewModel sch = new ClassCreateViewModel();
            return View(sch);
        }
        [HttpPost]
        public ActionResult CreateClass(ClassCreateViewModel sch)
        {
            SchoolClass sc = new SchoolClass();
            sc.Title = sch.Title;
            var schl = context.SchoolClasses.FirstOrDefault(c => c.Title == sch.Title);
            if (schl == null)
            {
                context.SchoolClasses.Add(sc);
                context.SaveChanges();
            } else
            {
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
                if (sch != null)
                {
                    sch.Title = viewModel.Title;
                    context.SaveChanges();
                }
                return RedirectToAction("EditClass", new { Id = viewModel.Id });
            } else
            {
                return View(viewModel);
            }
        }
        public ActionResult DeleteClass(int Id)
        {
            var schClass = context.SchoolClasses.FirstOrDefault(s => s.Id == Id);
            if (schClass != null)
            {
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
        public ActionResult ListClasses(int page = 0)
        {
            var viewModel = new ClassListViewModel();
            var classes = context.SchoolClasses.OrderBy(x => x.Title).Skip(page * ClassListViewModel.PER_PAGE).Take(ClassListViewModel.PER_PAGE).ToArray();
            viewModel.Classes = classes;
            viewModel.Page = page;
            viewModel.PageCount = (int)Math.Floor((float)context.SchoolClasses.Count() / (float)ClassListViewModel.PER_PAGE);
            return View(viewModel);
        }

        protected override void Dispose(bool disposing)
        {
            context.Dispose();
            base.Dispose(disposing);
        }
    }
}
