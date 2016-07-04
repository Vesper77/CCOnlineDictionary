using OnlineDiary.Models.Diary;
using OnlineDiary.Models.People;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace OnlineDiary.Models
{
    public abstract class MarksViewModel
    {
        public MarksViewModel(int year, int quadmester)
        {
            this.CurrentYear = year;
            this.quadmesterNumber = quadmester;
        }
        public MarksViewModel(int year)
        {
            this.CurrentYear = year;
        }
        protected ApplicationDbContext context = new ApplicationDbContext();
        /// <summary>
        /// Текущий пользователь
        /// </summary>
        public DiaryUser User { get; set; }
        /// <summary>
        /// Текущий учебный год
        /// </summary>
        public int CurrentYear { get; set; }
        /// <summary>
        /// Текущая четверть
        /// </summary>
        public int quadmesterNumber { get; set; }
        /// <summary>
        /// Определяет дату начала и конца четверти
        /// </summary>
        /// <returns>даты начала и конца четверти</returns>
        public Tuple<DateTime, DateTime> GetPeriod()
        {
            var startDate = new DateTime();
            var endDate = new DateTime();
            switch (quadmesterNumber)
            {
                case 1:
                    startDate = new DateTime(CurrentYear, 9, 1);
                    endDate = new DateTime(CurrentYear, 10, 30);
                    break;
                case 2:
                    startDate = new DateTime(CurrentYear, 11, 9);
                    endDate = new DateTime(CurrentYear, 12, 30);
                    break;
                case 3:
                    startDate = new DateTime(CurrentYear + 1, 1, 11);
                    endDate = new DateTime(CurrentYear + 1, 3, 18);
                    break;
                case 4:
                    startDate = new DateTime(CurrentYear + 1, 3, 28);
                    endDate = new DateTime(CurrentYear + 1, 5, 27);
                    break;
                default:
                    break;
            }
            return new Tuple<DateTime, DateTime>(startDate, endDate);
        }
        /// <summary>
        /// Выдаёт оценку для ученика по определенному предмету в определенный день
        /// </summary>
        /// <param name="date">Дата</param>
        /// <param name="lessonID">Id предмета</param>
        /// <param name="user">Ученик</param>
        /// <returns>Значение оценки, если она выставлена, null иначе</returns>
        public int? getMarkValue(DateTime date, int lessonID, DiaryUser user)
        {
            foreach (var m in context.Marks.Where(x => x.ChildrenId == user.Id))
            {
                if (m.Day == date && m.LessonId == lessonID)
                {
                    return m.MarkValue;
                }
            }
            return null;
        }
        /// <summary>
        /// Определяет оценку за четверть по данному предмету для данного ученика
        /// </summary>
        /// <param name="ChildrenId">id ученика</param>
        /// <param name="LessonId">id предмета</param>
        /// <param name="quadmesterNumber">номер четверти</param>
        /// <returns></returns>
        public int? GetFinalMark(string ChildrenId, int LessonId, int quadmesterNumber)
        {
            var marks = context.FinalMarks.Where(x => x.ChildrenId == ChildrenId && x.LessonId == LessonId
                                                && x.QuadmesterNumber == quadmesterNumber && x.Year == this.CurrentYear).ToList();
            if (marks != null && marks.Count > 0)
            {
                return marks[0].MarkValue;
            }
            return null;
        }
        /// <summary>
        /// Определяет, прогулял ли ученик определенный предмет в определеный день
        /// </summary>
        /// <param name="childrenId">Id ученика</param>
        /// <param name="LessonId">Id предмета</param>
        /// <param name="date">Дата</param>
        /// <returns>true, если ученик совершил прогул, иначе false</returns>
        public bool IsTruancyed(string childrenId, int LessonId, DateTime date)
        {
            var truancy = context.Truancys.Where(x => x.ChildrenId == childrenId
                                     && x.LessonId == LessonId
                                     && x.TruancyDate == date).FirstOrDefault();
            return truancy == null ? false : true;
        }
        public string getClassName(DiaryUser user)
        {
            var children = context.ChildrenData.Where(x => x.ChildrenId == user.Id).FirstOrDefault();
            if (children == null)
            {
                return "";
            }
            var schoolClass = context.SchoolClasses.Where(x => x.Id == children.SchoolClassId).FirstOrDefault();
            return schoolClass == null ? "" : schoolClass.Title;
        }
        /// <summary>
        /// Определяет название класса по его Id
        /// </summary>
        /// <param name="classId">Id класса</param>
        /// <returns>Название класса, если найден класс с таким id, иначе пустую строку</returns>
        public string getClassName(int classId)
        {
            var schoolClass = context.SchoolClasses.Where(x => x.Id == classId).First();
            return schoolClass == null ? "" : schoolClass.Title;
        }
        /// <summary>
        /// Определяет предметы, которые изучает ученик
        /// </summary>
        /// <param name="user">Ученик</param>
        /// <returns>Список предметов</returns>
        public List<Lesson> getLessons(DiaryUser user)
        {
            var lessons = context.Marks.Where(x => x.ChildrenId == user.Id).Select(x => x.LessonId).Distinct();
            var res = from i in lessons
                      from j in context.Lessons
                      where i == j.Id
                      select j;
            return res.ToList();
        }
    }
    public class ChildrenMarksViewModel : MarksViewModel
    {
        public ChildrenMarksViewModel(int year, int quadmester) : base(year, quadmester) { }
        public ChildrenMarksViewModel(int year) : base(year){ }
    }
    public class ParentMarksViewModel : MarksViewModel
    {
        public ParentMarksViewModel(int year, int quadmester) : base(year, quadmester) { }
        public ParentMarksViewModel(int year) : base(year) { }
        /// <summary>
        /// Ребенок, для которого необходимо просмотреть оценки
        /// </summary>
        public DiaryUser CurrentChildren { get; set; }
        /// <summary>
        /// Определяет детей данного родителя
        /// </summary>
        /// <returns>Список детей</returns>
        public List<DiaryUser> GetChildrens()
        {
            var childrenIDs = context.ChildrenData.Where(x => x.ParentId == User.Id).Select(x => x.ChildrenId);
            if (childrenIDs == null)
            {
                return new List<DiaryUser>();
            }
            var allChildrens = (from i in childrenIDs
                                from j in context.Users
                                where j.Id == i
                                select j).ToList();
            return allChildrens == null ? new List<DiaryUser>() : allChildrens;
        }
    }
    public class TeacherMarksViewModel : MarksViewModel
    {
        public TeacherMarksViewModel(int year, int quadmester) : base(year, quadmester) { }
        public TeacherMarksViewModel(int year) : base(year) { }
        public TeacherDataViewModel form = new TeacherDataViewModel();
        /// <summary>
        /// Определяет классы, в которых преподает учитель. 
        /// </summary>
        /// <returns>Коллекцию Dictionary вида id класса - имя класса</returns>
        public Dictionary<int, string> getSchoolClasses()
        {
            var lessons = context.Lessons.Where(l => l.TeacherId == User.Id).ToList();
            var classesIds = new List<int>();
            lessons.ForEach(
                l => classesIds.AddRange(context.ScheduleLessons.Where(x => x.LessonId == l.Id).
                     Select(x => x.SchoolClassId).Distinct().ToArray())
            );
            Dictionary<int, string> SchoolClasses = new Dictionary<int, string>();
            classesIds.ForEach(
                c =>
                {
                    if (!SchoolClasses.Keys.Any(k => k == c))
                        SchoolClasses.Add(c, context.SchoolClasses.FirstOrDefault(x => x.Id == c).Title);
                }
            );
            if (SchoolClasses.Count > 0 && form.ClassId == 0)
            {
                form.ClassId = SchoolClasses.Keys.First();
            }
            return SchoolClasses;
        }
        /// <summary>
        /// Определяет название предмета по его id
        /// </summary>
        /// <param name="lessonId">Id предмета</param>
        /// <returns>Название предмета, если предмет с таким id найден, иначе пустую строку</returns>
        public string getLessonName(int lessonId)
        {
            var lesson = context.Lessons.Where(x => x.Id == lessonId).First();
            return lesson == null ? "" : lesson.Title;
        }
        /// <summary>
        /// Определяет предметы, которые ведет учитель у определенного класса
        /// </summary>
        /// <returns>Коллекцию Dictionary вида Id предмета - название предмета</returns>
        public Dictionary<int, string> getLessons()
        {
            Dictionary<int, string> lessons = new Dictionary<int, string>();
            if (form.ClassId != default(int))
            {
                context.ScheduleLessons.Where(l => l.SchoolClassId == form.ClassId).Select(y => y.LessonId).Distinct().ToList().
                    ForEach(l => lessons.Add(l, context.Lessons.FirstOrDefault(x => x.Id == l).Title));
            }
            if(lessons.Count > 0 && form.LessonId == 0)
            {
                form.LessonId = lessons.Keys.First();
            }
            return lessons;
        }
        /// <summary>
        /// Определяет учеников данного класса
        /// </summary>
        /// <param name="classId">Id класса</param>
        /// <returns>Список учеников</returns>
        public List<DiaryUser> GetChildrensInClass(int classId)
        {
            var result = new List<DiaryUser>();
            var childrensId = context.ChildrenData.Where(x => x.SchoolClassId == classId).Select(x => x.ChildrenId);
            if(childrensId != null)
            {
                result = (from i in childrensId
                          from j in context.Users
                          where i == j.Id
                          select j).OrderBy(x => x.FirstName).ToList();

            }
            return result;
        }
        /// <summary>
        /// Определяет дни, в которые у определенного класса есть данный урок
        /// </summary>
        /// <returns></returns>
        public List<DayOfWeek> GetDays(int classId, int LessonId)
        {
            var schedule = context.ScheduleLessons.Where(x => x.SchoolClassId == classId && x.LessonId == LessonId);
            return schedule == null ? new List<DayOfWeek>() : schedule.Select(x => (DayOfWeek)x.DayNumber).Distinct().ToList();
        }
    }
    public class TeacherDataViewModel
    {
        /// <summary>
        /// Id выбранного класса
        /// </summary>
        public int ClassId { get; set; }
        /// <summary>
        /// Id выбранного предмета
        /// </summary>
        public int LessonId { get; set; }
    }
}
