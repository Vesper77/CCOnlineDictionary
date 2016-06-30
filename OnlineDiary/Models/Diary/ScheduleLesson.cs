using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineDiary.Models.Diary
{
    public class ScheduleLesson
    {
        public const int MAX_LESSONS_PER_DAY = 6;

        public int Id { get; set; }
        public int DayNumber { get; set; }

        public int LessonId { get; set; }
        public int SchoolClassId { get; set; }
        public int Order { get; set; }
        //[ForeignKey("LessonId")]
        //public virtual Lesson Lesson { get; set; }
        //[ForeignKey("SchoolClassId")]
        //public virtual SchoolClass SchoolClass { get; set; }
    }
}
