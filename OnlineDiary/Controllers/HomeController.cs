using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using OnlineDiary.DAL;
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
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
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

        [AllowAnonymous]
        public ActionResult Index(string returnUrl)
        {
            ApplicationUserManager manager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            ApplicationDbContext context = new ApplicationDbContext();
            context.Quadmesters.Add(new Quadmester
            {
                Number = 1,
                StartDate = new DateTime(2015, 9, 1),
                EndDate = new DateTime(2015, 10, 30),
            });
            context.SaveChanges();
            context.Quadmesters.Add(new Quadmester
            {
                Number = 2,
                StartDate = new DateTime(2015, 11, 9),
                EndDate = new DateTime(2015, 12, 30),
            });
            context.SaveChanges();
            context.Quadmesters.Add(new Quadmester
            {
                Number = 3,
                StartDate = new DateTime(2016, 1, 11),
                EndDate = new DateTime(2016, 3, 18),
            });
            context.SaveChanges();
            context.Quadmesters.Add(new Quadmester
            {
                Number = 4,
                StartDate = new DateTime(2016, 3, 28),
                EndDate = new DateTime(2016, 5, 27),
            });
            context.SaveChanges();
            var viewModel = new LoginViewModel();
            return View(viewModel);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            if (result == SignInStatus.Success) {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (await UserManager.IsInRoleAsync(user.Id, "admin"))
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    return RedirectToAction("Schedule", "Diary");
                }
            }
            if(result == SignInStatus.Failure)
            {
                return RedirectToAction("Home", "Index");
            }
            return View();
        }
    }
}
