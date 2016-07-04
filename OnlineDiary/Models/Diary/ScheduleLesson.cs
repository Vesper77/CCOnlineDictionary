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

        [ForeignKey("Lesson")]
        public int LessonId { get; set; }
        [ForeignKey("SchoolClass")]
        public int SchoolClassId { get; set; }
        public int Order { get; set; }

        public virtual Lesson Lesson { get; set; }
        public virtual SchoolClass SchoolClass { get; set; }
    }
}
