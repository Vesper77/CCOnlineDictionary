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
        /// <summary>
        /// Day when week start
        /// </summary>
        public DateTime StartWeek { get; set; }
        /// <summary>
        /// Dat when week end
        /// </summary>
        public DateTime EndWeek { get; set; }
        /// <summary>
        /// Number of week 
        /// </summary>
        public int NumberWeek
        {
            get { return numberWeek; }
            set
            {
                numberWeek = value;
                initWeek();
            }
        }
        /// <summary>
        /// Дни недели
        /// </summary>
        public string[] days = { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота" };
        /// <summary>
        /// Дни
        /// </summary>
        public DateTime[] numberDays = new DateTime[6];

        protected ApplicationDbContext context = new ApplicationDbContext();
        private int numberWeek = 0;

        /// <summary>
        /// Определяет имя пользователя по его Id
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Если пользователь найден, его имя, если оно задано иначе его email, если не найден - сообщение об этом</returns>
        public string getUserName(string id)
        {
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
        public Dictionary<string, ScheduleLessonViewModel[]> getDaysWithScheduleLessons(string userId)
        {
            var schedule = new Dictionary<string, ScheduleLessonViewModel[]>();

            var childrenData = context.ChildrenData.FirstOrDefault(d => d.ChildrenId == userId);
            var lessons = context.ScheduleLessons.Where(l => childrenData.SchoolClassId == l.SchoolClassId).Select(x => x).ToArray();

            for (int i = 0; i < days.Length; i++)
            {
                var schLessons = new ScheduleLessonViewModel[ScheduleLesson.MAX_LESSONS_PER_DAY];
                var ls = lessons.Where(l => l.DayNumber == i + 1).OrderBy(l => l.Order).Select(x => x).ToArray();
                for (int o = 0; o < ls.Length; o++)
                {
                    if (ls[o] != null) {
                        schLessons[ls[o].Order - 1] = new ScheduleLessonViewModel(ls[o]);
                    }
                }
                schedule.Add(days[i], schLessons);
            }
            return schedule;
        }
        /// <summary>
        /// Return lesosn title by id(Not use now, maybe)
        /// </summary>
        /// <param name="LessonId"></param>
        /// <returns></returns>
        public string getLessonTitle(int LessonId)
        {
            var lesson = context.Lessons.FirstOrDefault(x => x.Id == LessonId);
            return lesson == null ? "Урок не найден" : lesson.Title;
        }

        private void initWeek()
        {
            int diff = DayOfWeek.Monday - DateTime.Now.DayOfWeek - 1;
            if (diff == 0)
            {
                diff = -6;
            }
            var currentStartWeek = DateTime.Now.AddDays(diff);

            currentStartWeek = currentStartWeek.AddHours(-currentStartWeek.Hour);
            currentStartWeek = currentStartWeek.AddMinutes(-currentStartWeek.Minute);
            currentStartWeek = currentStartWeek.AddSeconds(-currentStartWeek.Second);

            StartWeek = currentStartWeek.AddDays(numberWeek * 7);
            EndWeek = StartWeek.AddDays(6);

            for (int i = 0; i < numberDays.Length; i++)
            {
                numberDays[i] = StartWeek.AddDays(i);
            }
        }
    }
    public class UserScheduleViewModel : ScheduleViewModel
    {
        public DiaryUser User;

        public Dictionary<string, ScheduleLessonViewModel[]> getDaysWithScheduleLessons()
        {
            return getDaysWithScheduleLessons(User.Id);
        }
        public string getChildUserName()
        {
            return getUserName(User);
        }
    }
    public class ParentUserScheduleViewModel : ScheduleViewModel
    {
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
        private void findChildrens()
        {
            var childrensData = context.ChildrenData.Where(x => x.ParentId == Parent.Id).ToList();
            List<DiaryUser> childrens = new List<DiaryUser>();
            childrensData.ForEach(
                x => {
                    var child = context.Users.FirstOrDefault(u => u.Id == x.ChildrenId);
                    if (child != null)
                    {
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
    public class TeacherScheduleViewModel : ScheduleViewModel
    {
        public DiaryUser Teacher;
        public Dictionary<string, ScheduleLessonViewModel[]> getDaysWithScheduleLessons()
        {
            var schedule = new Dictionary<string, ScheduleLessonViewModel[]>();
            var lessons = context.Lessons.Where(l => l.TeacherId == Teacher.Id).ToList();
            var schLessons = new List<ScheduleLessonViewModel>();
            lessons.ForEach(
                l => {
                context.ScheduleLessons.Where(x => x.LessonId == l.Id).ToList().ForEach(sc => schLessons.Add(new ScheduleLessonViewModel(sc)));
                }
            );
            for (int i = 0; i < days.Length; i++)
            {
                var dayLessons = new ScheduleLessonViewModel[ScheduleLesson.MAX_LESSONS_PER_DAY];
                schLessons.ForEach(
                    l => {
                        if (l.DayNumber == i + 1)
                        {
                            dayLessons[l.Order - 1] = l;
                        }
                    }
                );
                schedule.Add(days[i], dayLessons);
            }
            return schedule;
        }
        public string getUserName()
        {
            return getUserName(Teacher);
        }
    }
    public class EditScheduleViewModel
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        public int Day { get; set; }
        public int classiD { get; set; }
        public Dictionary<int, string> days = new Dictionary<int, string>()
        {
            {1, "Понедельник" },
            {2, "Вторник" },
            {3, "Среда" },
            {4, "Четверг" },
            {5, "Пятница" },
            {6, "Суббота" }
        };
        public Dictionary<int, string> Lessons { get; set; } = new Dictionary<int, string>();
        /// <summary>
        /// Возвращает все предметы
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, string> GetAllLessons()
        {
            Dictionary<int, string> lessons = new Dictionary<int, string>();
            lessons.Add(0, "Нет урока");
            foreach(var iter in context.Lessons.ToList())
            {
                lessons.Add(iter.Id, iter.Title);
            }
            return lessons;
        }
        /// <summary>
        /// Возвращает название предмета, который стоит у определенного класса в поределенный день под определенным номером
        /// </summary>
        /// <param name="order"></param>
        /// <param name="classId"></param>
        /// <returns></returns>
        public KeyValuePair<int, string> GetCurrentLesson(int order, int classId)
        {
            var schedulelesson = context.ScheduleLessons.
                Where(x => x.DayNumber == Day && x.SchoolClassId == classId && x.Order == order).
                FirstOrDefault();
            if (schedulelesson == null)
            {
                return new KeyValuePair<int, string>(0, "Нет урока");
            }
            var currentlesson = context.Lessons.Where(x => x.Id == schedulelesson.LessonId).FirstOrDefault();
            if(currentlesson == null)
            {
                return new KeyValuePair<int, string>(0,"Нет урока");
            }
            return new KeyValuePair<int, string>(currentlesson.Id, currentlesson.Title); 
        }
        public Dictionary<int, string> AllClasses { get; set; } = new Dictionary<int, string>();
        public Dictionary<int, string> GetAllClasses()
        {
            Dictionary<int, string> allClasses = new Dictionary<int, string>();
            foreach(var iter in context.SchoolClasses.ToList())
            {
                allClasses.Add(iter.Id, iter.Title);
            }
            return allClasses;
        }
        public void EditSchedule(int[] lesson)
        {
            for (int i = 0; i < 6; i++)
            {
                var removeobj = context.ScheduleLessons.FirstOrDefault(x => x.SchoolClassId == classiD &&
                                                                        x.DayNumber == Day &&
                                                                        x.Order == i + 1);
                if (removeobj != null)
                {
                    var homework = context.HomeWorks.Where(x => x.ScheludeLessonId == removeobj.Id).ToList();
                    if(homework != null && homework.Count > 0)
                    {
                        foreach(var iter in homework)
                        {
                            context.HomeWorks.Remove(iter);
                            context.SaveChanges();
                        }
                    }
                    if (lesson[i] != 0) {
                        removeobj.LessonId = lesson[i];
                    }
                    else 
                    {
                        context.ScheduleLessons.Remove(removeobj);
                    }
                    context.SaveChanges();
                } 
                else 
                if (lesson[i] != 0)
                {
                    var schedulelesson = new ScheduleLesson()
                    {
                        SchoolClassId = classiD,
                        LessonId = lesson[i],
                        Order = i + 1,
                        DayNumber = Day
                    };
                    context.ScheduleLessons.Add(schedulelesson);
                    context.SaveChanges();
                }
            }
        }
    }
    public class ScheduleLessonViewModel {
        private ScheduleLesson scheduleLesson { get; set; }
        private ApplicationDbContext context = new ApplicationDbContext();

        public ScheduleLessonViewModel(ScheduleLesson ScheduleLessons) {
            if (ScheduleLessons == null)
                throw new Exception("Need Schedule lesson");
            this.scheduleLesson = ScheduleLessons;
            
        }
        public bool isHaveHomeWork(DateTime day) {
            day = day.AddHours(-day.Hour);
            day = day.AddMinutes(-day.Minute);
            day = day.AddSeconds(-day.Second);
            var homeWork = context.HomeWorks.FirstOrDefault(l => l.ScheludeLessonId == scheduleLesson.Id );
            return homeWork != null;
        }
        public string Title {
            get {
                return scheduleLesson.Lesson.Title;
            }
        }
        public int Id {
            get {
                return scheduleLesson.Id;
            }
        }
        public int DayNumber {
            get {
                return scheduleLesson.DayNumber;
            }
        }
        public int Order {
            get {
                return scheduleLesson.Order;
            }
        }
    }
}
