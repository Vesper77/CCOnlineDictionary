using OnlineDictionary.Models.People;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineDictionary.Models.Schelude {
    public class Lesson {
        public int ID { get; set; }
        public Teacher Principal { get; set; }
        public string LessonTitle { get; set; }
    }
}
