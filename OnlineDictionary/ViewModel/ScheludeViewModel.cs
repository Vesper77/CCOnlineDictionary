using OnlineDictionary.Models;
using OnlineDictionary.Models.Schelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineDictionary.ViewModel {
    public class ScheludeViewModel {
        public int CountDays = DayLesson.STUDY_DAY_IN_WEEK;

        public SchoolClass schoolClass = null;
        public DayLesson[] Lessons = null;
        /// <summary>
        /// Return Lessons by day
        /// </summary>
        /// <param name="day"></param>
        /// <returns>Array of DayLessons</returns>
        public DayLesson[] GetLessonsByDay(int Day) {
            if (Lessons != null && Day <= CountDays && Lessons.Length > 0) {
                DayLesson[] DayLessons = new DayLesson[DayLesson.MAX_LESSONS_EVERY_DAY];
                foreach (var Lesson in Lessons) {
                    if (Lesson.DayNumber == Day) {
                        DayLessons[Lesson.LessonNumber] = Lesson;
                    }
                }
                return DayLessons;
            }
            return new DayLesson[0];
        }
    }
}
