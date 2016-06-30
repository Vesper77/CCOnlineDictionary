using OnlineDiary.Models.Diary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineDiary.Models.People
{
    public class ChildrenData
    {
        [ForeignKey("Parent")]
        public string ParentId { get; set; }
        [ForeignKey("Children")]
        public string ChildrenId { get; set; }
        [ForeignKey("SchoolClass")]
        public int SchoolClassId { get; set; }

        public DiaryUser Parent { get; set; }
        public DiaryUser Children { get; set; }
        public SchoolClass SchoolClass { get; set; }

    }
}
