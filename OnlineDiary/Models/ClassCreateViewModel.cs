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
        [Required(ErrorMessage =" Напишите имя")]
        public string Title { get; set; }
    }
}
