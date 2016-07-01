using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineDiary.Models
{
    public abstract class MarksViewModel
    {
        protected ApplicationDbContext context = new ApplicationDbContext();
    }
    public class TeacherMarksViewModel : MarksViewModel
    {
        //TODO : override this function with joins
        public DiaryUser Teacher { get; set; }
        public TeacherSelectMarksDataViewModel form { get; set; }

        public Dictionary<int, string> getSchoolClasses() {
            var lessons = context.Lessons.Where(l => l.TeacherId == Teacher.Id).ToList();
            var classesIds = new List<int>();
            lessons.ForEach(
                l => classesIds.AddRange(context.ScheduleLessons.Where(x => x.LessonId == l.Id).Select(x => x.SchoolClassId).Distinct().ToArray())
            );
            Dictionary<int, string> SchoolClasses = new Dictionary<int, string>();
            classesIds.ForEach(
                c =>
                {
                    if (!SchoolClasses.Keys.Any(k => k == c))
                        SchoolClasses.Add(c, context.SchoolClasses.FirstOrDefault(x => x.Id == c).Title);
                }
            );
            if (SchoolClasses.Count > 0) {
                form.ClassId = SchoolClasses.Keys.First();
            }
            return SchoolClasses;
        }
        public Dictionary<int, string> getLessons() {
            Dictionary<int, string> lessons = new Dictionary<int, string>();
            if (form.ClassId != default(int)) {
                context.ScheduleLessons.Where(l => l.SchoolClassId == form.ClassId).ToList().ForEach(l => lessons.Add(l.Id, context.Lessons.FirstOrDefault(x => x.Id == l.LessonId).Title));
            }
            return lessons;
        }

    }
    public class TeacherSelectMarksDataViewModel {
        public int ClassId { get; set; }
        public int LessonId { get; set; }

        public Dictionary<int, string> SchoolClasses = new Dictionary<int, string>();
        public Dictionary<int, string> Lessons = new Dictionary<int, string>();

    }
}
