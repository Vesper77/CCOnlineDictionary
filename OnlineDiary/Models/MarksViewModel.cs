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
        public HelpTableMarksViewModel marksModel = new HelpTableMarksViewModel();
        public MarksViewModel() { }
        public MarksViewModel(int quadmester)
        {
            this.quadmesterNumber = quadmester;
        }
        /// <summary>
        /// Даты начала и конца четверти учебного года
        /// </summary>
        public Tuple<DateTime, DateTime> Period;
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
        public Tuple<DateTime, DateTime>[] GetPeriod()
        {
            Tuple<DateTime, DateTime>[] periods = new Tuple<DateTime, DateTime>[4];
            int year = CurrentYear;
            var quads = context.Quadmesters.ToList();
            if (quads == null || quads.Count == 0)
            {
                return periods;
            }
            quads.Sort((x, y) => x.Number < y.Number ? -1 : x.Number > y.Number ? 1 : 0);
            var startdate = new DateTime(year, quads[0].StartDate.Month, quads[0].StartDate.Day);
            var enddate = new DateTime(quads[0].StartDate.Month > quads[0].EndDate.Month ?
                                       ++year : year, quads[0].EndDate.Month, quads[0].EndDate.Day);
            periods[quads[0].Number - 1] = new Tuple<DateTime, DateTime>(startdate, enddate);
            for (int i = quads[0].Number; i < Math.Min(4, quads.Count); i++)
            {
                for (int j = 2; j <= 4; j++)
                {
                    if (quads[i].Number == j)
                    {
                        startdate = new DateTime(quads[i].StartDate.Month < quads[i - 1].StartDate.Month ? ++year : year,
                                                                        quads[i].StartDate.Month, quads[i].StartDate.Day);
                        enddate = new DateTime(quads[i].EndDate.Month < quads[i].StartDate.Month ? ++year : year,
                                                                   quads[i].EndDate.Month, quads[i].EndDate.Day);
                        periods[j - 1] = new Tuple<DateTime, DateTime>(startdate, enddate);
                    }
                }
            }
            return periods;
        }
        public string GetUserName(DiaryUser user)
        {
            if (User == null)
            {
                return "Пользователь не найден";
            }
            var curUser = context.Users.Where(x => x.Id == user.Id).FirstOrDefault();
            return curUser == null ? "Пользователь не найден" : curUser.FirstName + " " + curUser.LastName[0] + '.' + curUser.ParentName[0] + '.';
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
            var period = GetPeriod()[quadmesterNumber - 1];
            int counterMarks = 0;
            int sumMarks = 0;
            if (period != null && period.Item1 != null && period.Item2 != null)
            {
                for (DateTime beginDate = period.Item1; beginDate <= period.Item2; beginDate = beginDate.AddDays(1))
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
                return (int)Math.Round((double)sumMarks / counterMarks, MidpointRounding.AwayFromZero);
            }
            return 0;
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
                return "Класс не найден";
            }
            var schoolClass = context.SchoolClasses.Where(x => x.Id == children.SchoolClassId).FirstOrDefault();
            return schoolClass == null ? "Класс не найден" : schoolClass.Title;
        }
        /// <summary>
        /// Определяет название класса по его Id
        /// </summary>
        /// <param name="classId">Id класса</param>
        /// <returns>Название класса, если найден класс с таким id, иначе пустую строку</returns>
        public string getClassName(int classId)
        {
            var schoolClass = context.SchoolClasses.Where(x => x.Id == classId).FirstOrDefault();
            return schoolClass == null ? "Класс не найден" : schoolClass.Title;
        }
        /// <summary>
        /// Определяет предметы, которые изучает ученик
        /// </summary>
        /// <param name="user">Ученик</param>
        /// <returns>Список предметов</returns>
        public List<Lesson> getLessons(DiaryUser user)
        {
            //var lessons = context.Marks.Where(x => x.ChildrenId == user.Id).Select(x => x.LessonId).Distinct();
            //var res = from i in lessons
            //          from j in context.Lessons
            //          where i == j.Id
            //          select j;
            if (user == null)
            {
                return new List<Lesson>();
            }
            var childrenData = context.ChildrenData.FirstOrDefault(x => x.ChildrenId == user.Id);
            if (childrenData != null)
            {
                var schlessons = context.ScheduleLessons.Where(l => l.SchoolClassId == childrenData.SchoolClassId).ToList();
                var lessons = new Dictionary<int, Lesson>();

                schlessons.ForEach(sc => {
                    if (!lessons.ContainsKey(sc.LessonId)) lessons.Add(sc.LessonId, sc.Lesson);
                });
                return lessons.Values.ToList();
            }
            return new List<Lesson>();
        }
    }
    public class ChildrenMarksViewModel : MarksViewModel
    {
        public ChildrenMarksViewModel() : base() { }
        public ChildrenMarksViewModel(int quadmester) : base(quadmester) { }
    }
    public class ParentMarksViewModel : MarksViewModel
    {
        public ParentMarksViewModel() : base() { }
        public ParentMarksViewModel(int quadmester) : base(quadmester) { }
        /// <summary>
        /// Ребенок, для которого необходимо просмотреть оценки
        /// </summary>
        public DiaryUser CurrentChildren { get; set; }
        /// <summary>
        /// Определяет детей данного родителя
        /// </summary>
        /// <returns>Список детей</returns>
        public Dictionary<string, string> Childrens { get; set; }
        public Dictionary<string, string> GetChildrens()
        {
            var childrenIDs = context.ChildrenData.Where(x => x.ParentId == User.Id).Select(x => x.ChildrenId);
            if (childrenIDs == null)
            {
                return new Dictionary<string, string>();
            }
            var allChildrens = (from i in childrenIDs
                                from j in context.Users
                                where j.Id == i
                                select j).ToList();
            if (allChildrens == null || allChildrens.Count == 0)
            {
                return new Dictionary<string, string>();
            }
            var childrens = new Dictionary<string, string>();
            foreach (var item in allChildrens)
            {
                childrens.Add(item.Id, item.FirstName + '.' + item.LastName[0] + '.' + item.ParentName[0] + '.');
            }
            return childrens;
        }
    }
    public class TeacherMarksViewModel : MarksViewModel
    {
        public TeacherMarksViewModel() : base() { }
        public TeacherMarksViewModel(int quadmester) : base(quadmester) { }
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
            return lesson == null ? "Предмет не найден" : lesson.Title;
        }
        /// <summary>
        /// Определяет предметы, которые ведет учитель у определенного класса
        /// </summary>
        /// <returns>Коллекцию Dictionary вида Id предмета - название предмета</returns>
        public Dictionary<int, string> getLessons()
        {
            Dictionary<int, string> lessons = new Dictionary<int, string>();
            if (User != null && form.ClassId != default(int))
            {
                var lessonIds = context.ScheduleLessons.Where(l => l.SchoolClassId == form.ClassId).Select(y => y.LessonId).Distinct().ToList();
                foreach (var item in lessonIds)
                {
                    var lesson = context.Lessons.Where(x => x.Id == item && x.TeacherId == User.Id).FirstOrDefault();
                    if (lesson != null)
                    {
                        lessons.Add(lesson.Id, lesson.Title);
                    }
                }
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
    }
    public class HelpTableMarksViewModel
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        /// <summary>
        /// минимум столбцов в таблице (при разбиении)
        /// </summary>
        public const int MinColumns = 13;
        /// <summary>
        /// максимум стобцов в таблице
        /// </summary>
        public const int MaxColumns = 25;
        /// <summary>
        /// Число столбцов в таблице
        /// </summary>
        public int ColumnsInTable { get; set; }
        /// <summary>
        /// Число столбцов в таблице
        /// </summary>
        public int ColumnsNumber { get; set; }
        /// <summary>
        /// Количество частей таблицы
        /// </summary>
        public int TablesCounter { get; set; }
        /// <summary>
        /// Возвращает список дней, в которые у определенного класса есть определенный урок
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="LessonId"></param>
        /// <returns></returns>
        public List<DayOfWeek> GetDays(int classId, int LessonId)
        {
            var schedule = context.ScheduleLessons.Where(x => x.SchoolClassId == classId && x.LessonId == LessonId).ToList();
            return schedule == null || schedule.Count == 0 ? new List<DayOfWeek>() : schedule.
                                       Select(x => (DayOfWeek)x.DayNumber).Distinct().ToList();
        }
        private int GetDayInPeriod(DateTime beginDate, DateTime endDate)
        {
            return endDate > beginDate ? (endDate - beginDate).Days : 0;
        }
        /// <summary>
        /// Вычисляет число столбцов в таблице
        /// </summary>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        public void GetColumnsNumber(DateTime beginDate, DateTime endDate)
        {
            int res = 0;
            for (DateTime date = beginDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Sunday)
                {
                    res++;
                }
            }
            ColumnsNumber = res > 0 ? res + 2 : 0;
        }
        /// <summary>
        /// Вычисляет число строк в таблице
        /// </summary>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="classId"></param>
        /// <param name="lessonId"></param>
        public void GetColumnsNumber(DateTime beginDate, DateTime endDate, int classId = 0, int lessonId = 0)
        {
            int res = 0;
            List<DayOfWeek> days = GetDays(classId, lessonId);
            for (DateTime date = beginDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Sunday && days.Contains(date.DayOfWeek))
                {
                    res++;
                }
            }
            ColumnsNumber = res > 0 ? res + 2 : 0;
        }
        /// <summary>
        /// Возвращает количество частей, на которые надо разбить талицу
        /// </summary>
        /// <param name="columns"></param>
        public void GetTableCount(int columns)
        {
            if (columns <= MaxColumns)
            {
                TablesCounter = 0;
                return;
            }
            int i = 2;
            while (!(columns / i >= MinColumns && columns / i <= MaxColumns))
            {
                i++;
            }
            TablesCounter = i;
            return;
        }
        /// <summary>
        /// Вычисляет число столбцов в части таблицы
        /// </summary>
        /// <returns></returns>
        public int GetColumnsInTable()
        {
            if (TablesCounter == 0)
            {
                return ColumnsNumber;
            }
            return ColumnsNumber / TablesCounter;
        }
        public List<DateTime> EndDates = new List<DateTime>();
        /// <summary>
        /// Возвращает список конечных дат в частях таблицы
        /// </summary>
        /// <returns></returns>
        public void GetEndDates(DateTime startDate, DateTime endDate, string Role = "", int classId = 0, int lessonId = 0)
        {
            int days = 0;
            if (Role == "children" || Role == "parent")
            {
                for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    if (date.DayOfWeek != DayOfWeek.Sunday)
                    {
                        days++;
                        if (days % ColumnsInTable == 0)
                        {
                            EndDates.Add(date);
                        }
                    }
                    if (EndDates.Count == TablesCounter - 1)
                    {
                        break;
                    }
                }
            }
            else if (Role == "teacher")
            {
                List<DayOfWeek> teacherdays = GetDays(classId, lessonId);
                for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    if (date.DayOfWeek != DayOfWeek.Sunday && teacherdays.Contains(date.DayOfWeek))
                    {
                        days++;
                        if (days % ColumnsInTable == 0)
                        {
                            EndDates.Add(date);
                        }
                    }
                    if (EndDates.Count == TablesCounter - 1)
                    {
                        break;
                    }
                }
            }
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