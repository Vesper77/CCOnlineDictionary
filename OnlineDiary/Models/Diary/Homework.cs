using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineDiary.Models.Diary
{
    public class Homework
    {
        public int Id { get; set; }
        public int ScheludeLessonId { get; set; }
        public DateTime Day { get; set; }
        public string Description { get; set; }
    }
}
