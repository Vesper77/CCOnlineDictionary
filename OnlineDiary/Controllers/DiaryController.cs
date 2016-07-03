using Microsoft.AspNet.Identity.Owin;
using OnlineDiary.Models;
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
        public async Task<ActionResult> Schelude()
        {
            var user = await UserManager.FindByNameAsync(User.Identity.Name);
            if (await UserManager.IsInRoleAsync(user.Id, "teacher"))
            {
                var viewModel = new TeacherScheduleViewModel();
                viewModel.Teacher = user;
                return View("TeacherSchedule", viewModel);
            }
            else if (await UserManager.IsInRoleAsync(user.Id, "parent"))
            {
                var viewModel = new ParentUserScheduleViewModel();
                viewModel.Parent = user;
                return View("ParentSchedule", viewModel);
            }
            else if (await UserManager.IsInRoleAsync(user.Id, "children"))
            {
                var viewModel = new UserScheduleViewModel();
                viewModel.User = user;
                return View("ChildrenSchedule", viewModel);
            }
            return RedirectToAction("Index", "Home");
        }
        [Authorize]
        public async Task<ActionResult> Marks(int lessonId = 2)
        {
            var user = await UserManager.FindByNameAsync(User.Identity.Name);
            if (await UserManager.IsInRoleAsync(user.Id, "teacher"))
            {
                return View();
            }
            else if (await UserManager.IsInRoleAsync(user.Id, "parent"))
            {
                return View();
            }
            else if (await UserManager.IsInRoleAsync(user.Id, "children"))
            {
                return View();
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
            var allChildrens = model.GetChildrens();
            if (allChildrens.Count > 0)
            {
                if (childrenId == null)
                {
                    childrenId = allChildrens[0].Id;
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
        //[Authorize(Roles = "teacher")]
        //public async Task<ActionResult> TeacherMarks(TeacherSelectMarksDataViewModel form = null)
        //{
        //    var user = await UserManager.FindByNameAsync(User.Identity.Name);
        //    var viewModel = new TeacherMarksViewModel(2015, 1);
        //    viewModel.User = user;
        //    viewModel.form = form == null ? new TeacherSelectMarksDataViewModel() : form;
        //    return View(viewModel);
        //}
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
            var allChildrens = model.GetChildrens();
            if (allChildrens.Count > 0)
            {
                if (childrenId == null)
                {
                    childrenId = allChildrens[0].Id;
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
            return View("TeacherFinalMarks", model);
        }
        [Authorize(Roles = "teacher")]
        [HttpPost]
        public async Task<ActionResult> TeacherFinalMarksPost(int classId, int lessonId)
        {
            return await TeacherFinalMarks(classId, lessonId);
        }
    }
}
/*
 * Parent 
 * Оценки для рёбёнка
 * Lessons\Days 1
 * Lessons1     3
 * --------
 * Children <-> Parent
 * --------
 * Teacher
 * Оценки за Физику для 5-Б
 * Children\Day 1
 * Петя         3
 * ------
 */
