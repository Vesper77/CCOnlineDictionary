using OnlineDiary.Models;
using OnlineDiary.Models.Diary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
namespace OnlineDiary.Controllers
{
    public class WorkController : Controller
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        [HttpPost]
        public JsonResult GetHomeWork(int ScheduleLessonId, DateTime StartWeek)
        {
            var schLesson = context.ScheduleLessons.FirstOrDefault(l => l.Id == ScheduleLessonId);
            if (schLesson != null)
            {
                DateTime dayForHomeWork = StartWeek.AddDays(schLesson.DayNumber - 1);
                var homeWork = context.HomeWorks.FirstOrDefault(h => h.Day == dayForHomeWork && h.ScheludeLessonId == ScheduleLessonId);
                if (homeWork != null)
                {
                    var id = User.Identity.GetUserId();
                    var isDone = context.CompltetedHomeWorks.FirstOrDefault(h => h.HomeWorkId == homeWork.Id && h.childrenId == id);

                    return Json(new { text = homeWork.Description, result = true, homeWorkId = homeWork.Id, isDone = isDone });
                }
                else
                {
                    return Json(new { result = true, text = "Нету домашний работы" });
                }
            }
            return Json(new { result = false });
        }
        [HttpPost]
        [Authorize(Roles ="teacher")]
        public JsonResult SetHomeWork(int ScheduleLessonId, DateTime StartWeek, string Text) {
            var schLesson = context.ScheduleLessons.FirstOrDefault(l => l.Id == ScheduleLessonId);
            if (schLesson != null)
            {
                DateTime dayForHomeWork = StartWeek.AddDays(schLesson.DayNumber - 1);
                var homeWork = context.HomeWorks.FirstOrDefault(h => h.Day == dayForHomeWork && h.ScheludeLessonId == ScheduleLessonId);
                if (homeWork != null)
                {
                    homeWork.Description = Text;
                }
                else
                {
                    homeWork = new Models.Diary.Homework();
                    homeWork.Description = Text;
                    homeWork.Day = dayForHomeWork;
                    homeWork.ScheludeLessonId = ScheduleLessonId;
                    context.HomeWorks.Add(homeWork);
                }
                context.SaveChanges();
                return Json(new { result = true });
            }
            return Json(new { result = false });
        }

        [HttpPost]
        [Authorize(Roles = "children")]
        public JsonResult setCompletedHomeWork(int homeWorkId) {
            if (homeWorkId > 0) {
                var homeWork = new CompletedHomeWork();
                homeWork.HomeWorkId = homeWorkId;
                homeWork.childrenId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                return Json(new { result = true });
            } else {
                return Json(new { result = false });
            }
        }
    }
}