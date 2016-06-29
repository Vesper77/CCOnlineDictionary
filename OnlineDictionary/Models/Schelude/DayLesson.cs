using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineDictionary.Models.Schelude {
    public class DayLesson {
        public const int STUDY_DAY_IN_WEEK = 6;
        public const int MAX_LESSONS_EVERY_DAY = 6;

        public int DayNumber { get; set; }
        public int LessonNumber { get; set; }
        public Lesson lesson { get; set; }
        public SchoolClass schoolClass { get; set; }

    }
}
