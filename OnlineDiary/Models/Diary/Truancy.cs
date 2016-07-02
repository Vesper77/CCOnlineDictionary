using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineDiary.Models.Diary
{
    public class Truancy
    {
        [Key]
        public int Id { get; set; }
        public string ChildrenId { get; set; }
        public int LessonId { get; set; }
        public DateTime TruancyDate { get; set; }
    }
}
