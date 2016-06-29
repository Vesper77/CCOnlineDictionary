using OnlineDictionary.Models;
using OnlineDictionary.Models.People;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineDictionary.ViewModel {
    public class MarksViewModel {
        private DateTime currentMonth = new DateTime();

        public MarksViewModel() {
            CurrentMonth = new DateTime(2015,9,1);
        }

        public int? getMarkValue(int Day, int LessonID) {
            foreach (var m in children.Marks) {
                if (m.Day.Month == currentMonth.Month && m.Day.Day == Day && m.SchoolLesson.ID == LessonID) {
                    return m.MarkValue;
                }
            }
            return null;
        }

        public DateTime CurrentMonth {
            get {
                return currentMonth;
            }
            set {
                if (value != currentMonth) {
                    currentMonth = value;
                    CountDays = DateTime.DaysInMonth(CurrentMonth.Year, CurrentMonth.Month);
                }
            }
        }
        public int CountDays = 0;
        public Children children = null;    
           
    }
}
