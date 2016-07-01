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
            else if (await UserManager.IsInRoleAsync(user.Id, "children")) {
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
                var viewModel = new TeacherMarksViewModel();
                viewModel.Teacher = user;
                return View("TeacherMarks", viewModel);
            }
            else if(await UserManager.IsInRoleAsync(user.Id, "parent"))
            {
                return View();
            }
            else if(await UserManager.IsInRoleAsync(user.Id, "children"))
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "teacher")]
        public async Task<ActionResult> TeacherMarks(TeacherSelectMarksDataViewModel form = null) {
            //TOOD End this Method
            var user = await UserManager.FindByNameAsync(User.Identity.Name);
            var viewModel = new TeacherMarksViewModel();
            viewModel.Teacher = user;
            viewModel.form = form == null ? new TeacherSelectMarksDataViewModel() : form;
            return View(viewModel);
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