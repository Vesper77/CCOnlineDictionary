using OnlineDiary.Models.Diary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineDiary.Models {
    public class ClassCreateViewModel {
        [Display(Name ="Название")]
        [Required(ErrorMessage = "Имя не может быть пустым")]
        public string Title { get; set; }
    }
    public class ClassEditViewModel {
        [Display(Name = "Название")]
        [Required(ErrorMessage = "Имя не может быть пустым")]
        public string Title { get; set; }

        [Required]
        public int Id { get; set; }
    }
    public class ClassListViewModel {
        public const int PER_PAGE = 10;

        public SchoolClass[] Classes { get; set; }
        public int Page { get; set; }
        public int PageCount { get; set; }
    }
}
