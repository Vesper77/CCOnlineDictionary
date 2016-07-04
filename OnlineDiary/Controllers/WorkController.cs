using OnlineDiary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

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
                    return Json(new { text = homeWork.Description, result = true });
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
    }
}