using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineDictionary.Models.People {
    public class Teacher : BaseUser {
        private static Teacher[] teachers = new Teacher[2];
        public static Teacher[] getTeachers() {
            teachers[0] = new Teacher();
            teachers[0].FirstName = "H1";
            teachers[1] = new Teacher();
            teachers[1].FirstName = "Bye";
            return teachers;
        }
    }
}
