using OnlineDictionary.Models;
using OnlineDictionary.Models.People;
using OnlineDictionary.Models.Schelude;
using OnlineDictionary.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineDictionary.Controllers {
    public class ScheludeController : Controller {
        public ActionResult List(int ClassId) {

            SchoolClass SelectedSchoolClass = new SchoolClass();
            SelectedSchoolClass.Title = "Class #" + ClassId;

            ScheludeViewModel ModelView = new ScheludeViewModel();
            ModelView.Lessons = SelectedSchoolClass.getLessons();
            ModelView.schoolClass = SelectedSchoolClass;

            return View(ModelView);
        }
        public ActionResult Marks(int UserId) {
            Children children = new Children();
            children.ID = UserId;
            children.FirstName = "FirstNameTest";
            children.LastName = "LastNameTest";
            MarksViewModel ModelView = new MarksViewModel();
            ModelView.children = children;

            return View(ModelView);
            
        }
    }
}