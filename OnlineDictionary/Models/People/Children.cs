using OnlineDictionary.Models.Schelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineDictionary.Models.People {
    public class Children  : BaseUser {
        public Children() {
            int countLessons = 3;
            Lessons = new Lesson[3];
            for (int i = 0; i < countLessons; i++) {
                Lessons[i] = new Lesson();
                Lessons[i].LessonTitle = "Lesson #" + i;
                Lessons[i].ID = i;
            }
            Random r = new Random();
            int count = 15;
            Marks = new Mark[15];
            for (int i = 0; i < count; i++) {
                Marks[i] = new Mark();
                Marks[i].Day = new DateTime(2015, 9, i + 1);
                Marks[i].MarkValue = r.Next(4) + 1;                
                Marks[i].SchoolLesson = Lessons[r.Next(3)];
            }
        }
        public Parent ChildrenParent { get; set; }
        public SchoolClass ChildrenClass { get; set; }

        public Lesson[] Lessons = null;
        public Mark[] Marks = null;
    }
}
