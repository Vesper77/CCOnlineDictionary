using OnlineDictionary.Models.People;
using OnlineDictionary.Models.Schelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineDictionary.Models {
    public class SchoolClass {
        public int ID { get; set; }
        public string Title { get; set; }
        public Teacher SchoolTeacher { get; set; }

        public DayLesson[] getLessons() {
            int count = 36;
            DayLesson[] Lessons = new DayLesson[count];
            for (int i = 0, j = 0, b = 1; i < count; i++, j++) {
                Lessons[i] = new DayLesson();
                Lessons[i].DayNumber = b;
                Lessons[i].LessonNumber = j;
                Lessons[i].lesson = new Lesson();
                Lessons[i].lesson.LessonTitle = "For DayLesson#" + i;
                if (j == DayLesson.MAX_LESSONS_EVERY_DAY - 1) {
                    j = -1;
                    b++;
                }
            }
            return Lessons;
        }

        
    }
}
