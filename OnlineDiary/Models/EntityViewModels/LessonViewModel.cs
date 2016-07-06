using OnlineDiary.Models.Diary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineDiary.Models
{
    public class ListLessonsViewModel
    {
        public const int ITEMS_PER_PAGE = 10;

        public Lesson[] Lessons;
        public int page { get; set; } = 0;
    }

    public class EditLessonViewModel
    {
        [Required]
        [Display(Name ="Название")]
        public string Title { get; set; }
        public int Id { get; set; }
    }
}
