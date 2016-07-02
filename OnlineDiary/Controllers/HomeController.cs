using OnlineDiary.DAL;
using OnlineDiary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineDiary.Controllers
{
    public class HomeController : Controller
    {
        
        public ActionResult Index()
        {
            var viewModel = new LoginViewModel();
            return View(viewModel);
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}