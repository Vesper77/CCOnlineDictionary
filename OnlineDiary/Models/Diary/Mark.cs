using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineDiary.Models.Diary
{
    public class Mark
    {
        public int Id { get; set; }
        [ForeignKey("Children")]
        public string ChildrenId { get; set; }
        [ForeignKey("Lesson")]
        public int LessonId { get; set; }
        public DateTime Day { get; set; }
        public int MarkValue { get; set; }

        public DiaryUser Children { get; set; }
        public Lesson Lesson { get; set; }
    }
}
