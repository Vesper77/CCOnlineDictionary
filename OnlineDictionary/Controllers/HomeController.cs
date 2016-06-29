using OnlineDictionary.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;


namespace OnlineDictionary.Controllers {
    public class HomeController : Controller {
        public ActionResult Index() {
            LoginFormViewModel ViewModel = new LoginFormViewModel();
            return View(ViewModel);
        }
        [HttpPost]
        public ActionResult Login(LoginFormViewModel ViewModel) {
            if (ViewModel != null) {
                if (ModelState.IsValid && ViewModel.LogIn()) {
                    this.RedirectToAction("","");
                }
                return View("Index", ViewModel);
            }
            return this.RedirectToAction("index");        
        }
    }
}