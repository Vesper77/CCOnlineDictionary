using Microsoft.AspNet.Identity.Owin;
using OnlineDiary.Models;
using OnlineDiary.Models.Diary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
namespace OnlineDiary.Controllers
{
    public class DiaryController : Controller
    {
        private ApplicationUserManager _userManager = null;
        private ApplicationDbContext context = new ApplicationDbContext();
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
        [Authorize]
        public async Task<ActionResult> Schedule(int numberWeek = 0)
        {
            var user = await UserManager.FindByNameAsync(User.Identity.Name);

            if (await UserManager.IsInRoleAsync(user.Id, "teacher"))
            {
                var viewModel = new TeacherScheduleViewModel();
                viewModel.Teacher = user;
                viewModel.NumberWeek = numberWeek;
                return View("TeacherSchedule", viewModel);
            }
            else if (await UserManager.IsInRoleAsync(user.Id, "parent"))
            {
                var viewModel = new ParentUserScheduleViewModel();
                viewModel.Parent = user;
                viewModel.NumberWeek = numberWeek;
                return View("ParentSchedule", viewModel);
            }
            else if (await UserManager.IsInRoleAsync(user.Id, "children"))
            {
                var viewModel = new UserScheduleViewModel();
                viewModel.User = user;
                viewModel.NumberWeek = numberWeek;
                return View("ChildrenSchedule", viewModel);
            }
            return RedirectToAction("Index", "Home");
        }
        [Authorize]
        public async Task<ActionResult> Marks()
        {
            var user = await UserManager.FindByNameAsync(User.Identity.Name);
            if (await UserManager.IsInRoleAsync(user.Id, "teacher"))
            {
                return RedirectToAction("TeacherMarks");
            }
            else if (await UserManager.IsInRoleAsync(user.Id, "parent"))
            {
                return RedirectToAction("ParentMarks");
            }
            else if (await UserManager.IsInRoleAsync(user.Id, "children"))
            {
                return RedirectToAction("ChildrenMarks");
            }
            return RedirectToAction("Index", "Home");
        }
        public async Task<ActionResult> FinalMarks() {
            var user = await UserManager.FindByNameAsync(User.Identity.Name);
            if (await UserManager.IsInRoleAsync(user.Id, "teacher"))
            {
                return RedirectToAction("TeacherFinalMarks");
            }
            else if (await UserManager.IsInRoleAsync(user.Id, "parent"))
            {
                return RedirectToAction("ParentFinalMarks");
            }
            else if (await UserManager.IsInRoleAsync(user.Id, "children"))
            {
                return RedirectToAction("ChildrenFinalMarks");
            }
            return RedirectToAction("Index", "Home");
        }
        [Authorize(Roles = "children")]
        [HttpGet]
        public async Task<ActionResult> ChildrenMarks(int quadmester = 1, int year = 2015)
        {
            var user = await UserManager.FindByNameAsync(User.Identity.Name);
            ChildrenMarksViewModel model = new ChildrenMarksViewModel(year, quadmester);
            model.User = user;
            return View("ChildrenMarks", model);
        }
        [Authorize(Roles = "parent")]
        [HttpGet]
        public async Task<ActionResult> ParentMarks(string childrenId, int quadmester = 1, int year = 2015)
        {
            var user = await UserManager.FindByNameAsync(User.Identity.Name);
            ParentMarksViewModel model = new ParentMarksViewModel(year, quadmester);
            model.User = user;
            model.Childrens = model.GetChildrens();
            if (model.Childrens.Count > 0)
            {
                if (childrenId == null)
                {
                    childrenId = model.Childrens[0].Id;
                }
                model.CurrentChildren = context.Users.Where(x => x.Id == childrenId).First();
                ViewBag.ChildId = childrenId;
                return View("ParentMarks", model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [Authorize(Roles = "parent")]
        [HttpPost]
        public async Task<ActionResult> ParentMarksPost(string childrenId)
        {
            return await ParentMarks(childrenId);
        }

        [Authorize(Roles = "teacher")]
        [HttpGet]
        public async Task<ActionResult> TeacherMarks(int LessonId = 0, int ClassId = 0, int quadmester = 1, int year = 2015)
        {
            var user = await UserManager.FindByNameAsync(User.Identity.Name);
            TeacherMarksViewModel model = new TeacherMarksViewModel(year, quadmester);
            if (LessonId != 0)
            {
                model.form.LessonId = LessonId;
            }
            if (ClassId != 0)
            {
                model.form.ClassId = ClassId;
            }
            model.User = user;
            model.form.Classes = model.getSchoolClasses();
            model.form.Lessons = model.getLessons();
            return View("TeacherMarks", model);
        }
        [Authorize(Roles = "teacher")]
        [HttpPost]
        public async Task<ActionResult> TeacherMarksPost(int lessonId, int classId)
        {
            return await TeacherMarks(lessonId, classId);
        }
        [Authorize(Roles = "children")]
        [HttpGet]
        public async Task<ActionResult> ChildrenFinalMarks(int year = 2015)
        {
            var user = await UserManager.FindByNameAsync(User.Identity.Name);
            ChildrenMarksViewModel model = new ChildrenMarksViewModel(year);
            model.User = user;
            return View("ChildrenFinalMarks", model);
        }
        [Authorize(Roles = "parent")]
        [HttpGet]
        public async Task<ActionResult> ParentFinalMarks(string childrenId, int year = 2015)
        {
            var user = await UserManager.FindByNameAsync(User.Identity.Name);
            ParentMarksViewModel model = new ParentMarksViewModel(year);
            model.User = user;
            model.Childrens = model.GetChildrens();
            if (model.Childrens.Count > 0)
            {
                if (childrenId == null)
                {
                    childrenId = model.Childrens[0].Id;
                }
                model.CurrentChildren = context.Users.Where(x => x.Id == childrenId).First();
                ViewBag.ChildId = childrenId;
                return View("ParentFinalMarks", model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [Authorize(Roles = "parent")]
        [HttpPost]
        public async Task<ActionResult> ParentFinalMarksPost(string childrenId)
        {
            return await ParentFinalMarks(childrenId);
        }
        [Authorize(Roles = "teacher")]
        [HttpGet]
        public async Task<ActionResult> TeacherFinalMarks(int classId = 0, int lessonId = 0, int year = 2015)
        {
            var user = await UserManager.FindByNameAsync(User.Identity.Name);
            TeacherMarksViewModel model = new TeacherMarksViewModel(year);
            if (lessonId != 0)
            {
                model.form.LessonId = lessonId;
            }
            if (classId != 0)
            {
                model.form.ClassId = classId;
            }
            model.User = user;
            model.form.Classes = model.getSchoolClasses();
            model.form.Lessons = model.getLessons();
            return View("TeacherFinalMarks", model);
        }
        [Authorize(Roles = "teacher")]
        [HttpPost]
        public async Task<ActionResult> TeacherFinalMarksPost(int classId, int lessonId)
        {
            return await TeacherFinalMarks(classId, lessonId);
        }
        [HttpPost]
        [Authorize(Roles ="teacher")]
        public JsonResult SetMark(DateTime day, int markvalue, string childrenId, int lessonId)
        {
            if (markvalue > 5 && markvalue < 0)
            {
                return Json(new { result = false });
            }
            var mark = context.Marks.FirstOrDefault(m => m.ChildrenId == childrenId && m.LessonId == lessonId && m.Day == day);
            if (mark == null)
            {
                mark = new Models.Diary.Mark();
                mark.ChildrenId = childrenId;
                mark.MarkValue = markvalue;
                mark.Day = day;
                mark.LessonId = lessonId;
                context.Marks.Add(mark);
            }
            else
            {
                mark.MarkValue = markvalue;
            }
            var tryancy = context.Truancys.FirstOrDefault(t => t.ChildrenId == childrenId && t.LessonId == lessonId && t.TruancyDate == day);
            if (tryancy != null) {
                context.Truancys.Remove(tryancy);
            }                
            context.SaveChanges();
            return Json(new { result = true, markValue = mark.MarkValue });
        }
        [HttpPost]
        [Authorize(Roles = "teacher")]
        public JsonResult SetTruancy(DateTime day, string childrenId, int lessonId)
        {
            if (day.Hour == 0 && day.Minute == 0 && day.Second == 0) {
                context.Truancys.Add(new Models.Diary.Truancy() { TruancyDate = day, ChildrenId = childrenId, LessonId = lessonId });
                var mark = context.Marks.FirstOrDefault(m => m.ChildrenId == childrenId && m.Day == day && m.LessonId == lessonId);
                if (mark != null)
                {
                    context.Marks.Remove(mark);
                }
                context.SaveChanges();
                return Json(new { result = true, markValue = "Н" });
            }
            return Json(new { result = false });
        }
        [HttpPost]
        [Authorize(Roles = "teacher")]
        public JsonResult setFinalMark(string childrenId, int lessonId, int fourth, int markvalue) {
            if (markvalue < 1 || markvalue > 5) {
                return Json(new { result = false});
            }
            FinalMark mark = context.FinalMarks.FirstOrDefault(m => m.LessonId == lessonId && m.QuadmesterNumber == fourth && m.ChildrenId == childrenId);
            if (mark != null)
            {
                mark.MarkValue = markvalue;
            }
            else
            {
                mark = new FinalMark();
                mark.ChildrenId = childrenId;
                mark.LessonId = lessonId;
                mark.QuadmesterNumber = fourth;
                mark.MarkValue = markvalue;
                mark.Year = DateTime.Now.Year - 1;
                context.FinalMarks.Add(mark);
            }
            context.SaveChanges();
            return Json(new { result = true, markValue = markvalue });
        }
    }
}
