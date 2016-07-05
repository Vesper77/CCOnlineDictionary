using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineDiary.Models.Diary
{
    public class Lesson
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string TeacherId { get; set; }

        public virtual DiaryUser Teacher { get; set; }
        //public virtual ScheduleLesson ScheduleLesson { get; set; }
    }
}
