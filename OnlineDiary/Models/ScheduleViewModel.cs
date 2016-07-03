using Microsoft.AspNet.Identity;
using OnlineDiary.Models.Diary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineDiary.Models
{
    public abstract class ScheduleViewModel
    {
        protected ApplicationDbContext context = new ApplicationDbContext();
        /// <summary>
        /// Дни недели
        /// </summary>
        public string[] days = { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота" };
        /// <summary>
        /// Определяет имя пользователя по его Id
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Если пользователь найден, его имя, если оно задано иначе его email, если не найден - сообщение об этом</returns>
        public string getUserName(string id) {
            var user = context.Users.FirstOrDefault(x => x.Id == id);
            if (user != null)
            {
                if (user.FirstName != null && user.LastName != null)
                {
                    return user.FirstName + " " + user.LastName;
                }
                else
                {
                    return user.Email;
                }

            }
            else
            {
                return "User not found";
            }
        }
        /// <summary>
        /// Определяет имя пользователя
        /// </summary>
        /// <param name="User">Пользователь</param>
        /// <returns>Имя пользователя, если оно задано, иначе его email</returns>
        public string getUserName(DiaryUser User)
        {
            if (User.FirstName != null && User.LastName != null)
            {
                return User.FirstName + " " + User.LastName;
            }
            else
            {
                return User.Email;
            }
        }
        /// <summary>
        /// Определяет расписание ученика или учителя
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Dictionary<string, ScheduleLesson[]> getDaysWithScheduleLessons(string userId)
        {
            var schedule = new Dictionary<string, ScheduleLesson[]>();

            var childrenData = context.ChildrenData.FirstOrDefault(d => d.ChildrenId == userId);
            var lessons = context.ScheduleLessons.Where(l => childrenData.SchoolClassId == l.SchoolClassId).Select(x => x).ToArray();

            for (int i = 0; i < days.Length; i++)
            {
                var schLessons = new ScheduleLesson[ScheduleLesson.MAX_LESSONS_PER_DAY];
                var ls = lessons.Where(l => l.DayNumber == i + 1).OrderBy(l => l.Order).Select(x => x).ToArray();
                for (int o = 0; o < ls.Length; o++)
                {
                    schLessons[ls[o].Order - 1] = ls[o];
                }
                schedule.Add(days[i], schLessons);
            }
            return schedule;
        }
        public string getLessonTitle(int LessonId)
        {
            var lesson = context.Lessons.FirstOrDefault(x => x.Id == LessonId);
            return lesson == null ? "Урок не найден" : lesson.Title;
        }
    }
    public class UserScheduleViewModel : ScheduleViewModel
    {        
        public DiaryUser User;

        public Dictionary<string, ScheduleLesson[]> getDaysWithScheduleLessons()
        {
            return getDaysWithScheduleLessons(User.Id);
        }
        public string getChildUserName() {
            return getUserName(User);
        }
    }
    public class ParentUserScheduleViewModel : ScheduleViewModel {
        private DiaryUser parent = null;
        public DiaryUser Parent
        {
            get { return parent; }
            set
            {
                parent = value;
                findChildrens();
            }
        }
        private void findChildrens() {
            var childrensData = context.ChildrenData.Where(x => x.ParentId == Parent.Id).ToList();
            List<DiaryUser> childrens = new List<DiaryUser>();
            childrensData.ForEach(
                x => {
                    var child = context.Users.FirstOrDefault(u => u.Id == x.ChildrenId);
                    if (child != null) {
                        childrens.Add(child);
                    }
                }
            );
            Childrens = childrens.ToArray();
        }
        public DiaryUser[] Childrens { get; set; }

        public string getParentUserName()
        {
            return getUserName(parent);
        }

    }
    public class TeacherScheduleViewModel : ScheduleViewModel {
        public DiaryUser Teacher;
        public Dictionary<string, ScheduleLesson[]> getDaysWuthScheduleLessons() {
            var schedule = new Dictionary<string, ScheduleLesson[]>();
            var lessons = context.Lessons.Where(l => l.TeacherId == Teacher.Id).ToList();
            var schLessons = new List<ScheduleLesson>();
            lessons.ForEach(
                l => {
                    schLessons.AddRange(context.ScheduleLessons.Where(x => x.LessonId == l.Id).ToArray());
                }
            );
            for (int i = 0; i < days.Length; i++) {
                var dayLessons = new ScheduleLesson[ScheduleLesson.MAX_LESSONS_PER_DAY];
                schLessons.ForEach( 
                    l => {
                        if (l.DayNumber == i + 1) {
                            dayLessons[l.Order - 1] = l;
                        }
                    }    
                );
                schedule.Add(days[i], dayLessons);
            }
            return schedule;
        }
        public string getUserName() {
            return getUserName(Teacher);
        }
    }
}
