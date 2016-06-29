using OnlineDictionary.Models.People;
using OnlineDictionary.Models.Schelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineDictionary.Models {
    public class Mark {
        public Children SchoolChildren { get; set; }
        public Lesson SchoolLesson { get; set; }

        public DateTime Day { get; set; }
        public int MarkValue { get; set; }
    }
}
