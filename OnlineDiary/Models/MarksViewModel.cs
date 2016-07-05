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
        private int currentYear;
        public int FinalMarkYear { get; set; }
        public MarksViewModel()
        {
        }
        public MarksViewModel(int year)
        {
            this.FinalMarkYear = year;
        }
        public Tuple<DateTime, DateTime>[] Periods = new Tuple<DateTime, DateTime>[4];
        protected ApplicationDbContext context = new ApplicationDbContext();
        /// <summary>
        /// Текущий пользователь
        /// </summary>
        public DiaryUser User { get; set; }
        /// <summary>
        /// Текущий учебный год
        /// </summary>
        public int CurrentYear
        {
            get
            {
                currentYear = DateTime.Now.Month > 9 ? DateTime.Now.Year : DateTime.Now.Year - 1;
                return currentYear;
            }
            set
            {
                currentYear = value;
            }
        }
        /// <summary>
        /// Текущая четверть
        /// </summary>
        public int quadmesterNumber { get; set; }
        /// <summary>
        /// Определяет даты начала и конца четвертей
        /// </summary>
        /// <returns>даты начала и конца четвертей</returns>
        public Tuple<DateTime, DateTime>[] GetPeriods()
        {
            Tuple<DateTime, DateTime>[] periods = new Tuple<DateTime, DateTime>[4];
            int year = CurrentYear;
            var quads = context.Quadmesters.ToArray(); // считаю, что они гарантированно есть в бд, иначе не понятно, что возвращать
            var startdate = new DateTime(year, quads[0].StartDate.Month, quads[0].StartDate.Day);
            var enddate = new DateTime(quads[0].StartDate.Month > quads[0].EndDate.Month ? 
                                       ++year : year, quads[0].EndDate.Month, quads[0].EndDate.Day);
            periods[0] = new Tuple<DateTime, DateTime>(startdate, enddate);
            for(int i = 1; i < 4; i++)
            {
                startdate = new DateTime(quads[i].StartDate.Month < quads[i- 1].StartDate.Month ? ++year : year, 
                                                                quads[i].StartDate.Month, quads[i].StartDate.Day);
                enddate = new DateTime(quads[i].EndDate.Month < quads[i].StartDate.Month? ++year : year,
                                                           quads[i].EndDate.Month, quads[i].EndDate.Day);
                periods[i] = new Tuple<DateTime, DateTime>(startdate, enddate);
            }
            return periods;
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
        public int GetFinalMark(string ChildrenId, int LessonId, int quadmesterNumber)
        {
            var period = GetPeriods();
            int counterMarks = 0;
            int sumMarks = 0;
            for (DateTime beginDate = period[quadmesterNumber - 1].Item1; beginDate <= period[quadmesterNumber - 1].Item2;
                                                                                          beginDate = beginDate.AddDays(1))
            {
                var mark = context.Marks.Where(x => x.ChildrenId == ChildrenId && x.Day == beginDate && x.LessonId == LessonId).
                                               FirstOrDefault();
                if (mark != null)
                {
                    sumMarks += mark.MarkValue;
                    counterMarks++;
                }
            }
            if (counterMarks == 0)
            {
                return 0;
            }
            return (int)Math.Round((double)sumMarks / counterMarks);
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
            var schoolClass = context.SchoolClasses.Where(x => x.Id == classId).FirstOrDefault();
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
        public ChildrenMarksViewModel() : base() { }
        public ChildrenMarksViewModel(int year) : base(year) { }
    }
    public class ParentMarksViewModel : MarksViewModel
    {
        public ParentMarksViewModel() : base() { }
        public ParentMarksViewModel(int year) : base(year) { }
        /// <summary>
        /// Ребенок, для которого необходимо просмотреть оценки
        /// </summary>
        public DiaryUser CurrentChildren { get; set; }
        /// <summary>
        /// Определяет детей данного родителя
        /// </summary>
        /// <returns>Список детей</returns>
        public List<DiaryUser> Childrens { get; set; }
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
        public TeacherMarksViewModel() : base() { }
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
            var lesson = context.Lessons.Where(x => x.Id == lessonId).FirstOrDefault();
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
            if (lessons.Count > 0 && form.LessonId == 0)
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
            var childrensId = context.ChildrenData.Where(x => x.SchoolClassId == classId).ToList().Select(x => x.ChildrenId);
            if (childrensId != null)
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
            var schedule = context.ScheduleLessons.Where(x => x.SchoolClassId == classId && x.LessonId == LessonId).ToList();
            return schedule == null || schedule.Count == 0 ? new List<DayOfWeek>() : schedule.
                                       Select(x => (DayOfWeek)x.DayNumber).Distinct().ToList();
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
        /// <summary>
        /// Классы, где ведет учитель
        /// </summary>
        public Dictionary<int, string> Classes { get; set; }
        /// <summary>
        /// Предмеы, которые ведет учитель
        /// </summary>
        public Dictionary<int, string> Lessons { get; set; }
    }
}