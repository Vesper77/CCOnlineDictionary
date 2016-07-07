using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineDiary.Models.Diary
{
    public class CompletedHomeWork
    {
        [Key]
        public int Id { get; set; }

        public string childrenId { get; set; }
        public int HomeWorkId { get; set; }

        public virtual Homework HomeWork { get; set; }
        public virtual DiaryUser Chilldren { get; set; }
    }
}