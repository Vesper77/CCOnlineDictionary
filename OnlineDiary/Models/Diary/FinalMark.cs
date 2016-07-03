using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineDiary.Models.Diary
{
    public class FinalMark
    {
        [Key]
        public int Id { get; set; }
        public string ChildrenId { get; set; }
        public int LessonId { get; set; }
        public int QuadmesterNumber { get; set; } // 0 - Годовая
        public int Year { get; set; }
        public int MarkValue { get; set; }
    }
}
