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
        protected ApplicationDbContext context = new ApplicationDbContext();
        public DiaryUser User { get; set; }
        public int CurrentYear { get; set; }
        public int quadmesterNumber { get; set; }
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
        public string getClassName(DiaryUser user)
        {
            var children = context.ChildrenData.Where(x => x.ChildrenId == user.Id).First();
            if (children == null)
            {
                return "";
            }
            var schoolClass = context.SchoolClasses.Where(x => x.Id == children.SchoolClassId).First();
            return schoolClass == null ? "" : schoolClass.Title;
        }
        public string getClassName(int classId)
        {
            var schoolClass = context.SchoolClasses.Where(x => x.Id == classId).First();
            return schoolClass == null ? "" : schoolClass.Title;
        }
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
    }
    public class ParentMarksViewModel : MarksViewModel
    {
        public ParentMarksViewModel(int year, int quadmester) : base(year, quadmester) { }
        public DiaryUser CurrentChildren { get; set; }
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
        public TeacherSelectMarksDataViewModel form = new TeacherSelectMarksDataViewModel();
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
        public string getLessonName(int lessonId)
        {
            var lesson = context.Lessons.Where(x => x.Id == lessonId).First();
            return lesson == null ? "" : lesson.Title;
        }
        public Dictionary<int, string> getLessons()
        {
            Dictionary<int, string> lessons = new Dictionary<int, string>();
            if (form.ClassId != default(int))
            {
                context.ScheduleLessons.Where(l => l.SchoolClassId == form.ClassId).ToList().
                    ForEach(l => lessons.Add(l.LessonId, context.Lessons.FirstOrDefault(x => x.Id == l.LessonId).Title));
            }
            if(lessons.Count > 0 && form.LessonId == 0)
            {
                form.LessonId = lessons.Keys.First();
            }
            return lessons;
        }
        public List<DiaryUser> GetChildrensInClass(int classId)
        {
            var result = new List<DiaryUser>();
            var childrensId = context.ChildrenData.Where(x => x.SchoolClassId == classId).Select(x => x.ChildrenId);
            if(childrensId != null)
            {
                result = (from i in childrensId
                          from j in context.Users
                          where i == j.Id
                          select j).ToList();

            }
            return result;
        }
    }
    public class TeacherSelectMarksDataViewModel
    {
        public int ClassId { get; set; }
        public int LessonId { get; set; }
        public Dictionary<int, string> SchoolClasses = new Dictionary<int, string>();
        public Dictionary<int, string> Lessons = new Dictionary<int, string>();
    }
}
