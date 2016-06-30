using OnlineDiary.Models.Diary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineDiary.Models.People
{
    public class ChildrenData
    {
        [Key]
        public int Id { get; set; }

        public string ParentId { get; set; }
        public string ChildrenId { get; set; }
        public int SchoolClassId { get; set; }

        [ForeignKey("ParentId")]
        public virtual DiaryUser Parent { get; set; }
        [ForeignKey("ChildrenId")]
        public virtual DiaryUser Children { get; set; }
        
        //public virtual SchoolClass ChildrenSchoolClass { get; set; }

    }
}
