using OnlineDiary.Models.Diary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace OnlineDiary.Models
{
    public class LessonViewModel
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        public Dictionary<string, string> Teachers { get; set; }
        public Dictionary<string, string> GetAllTeachers()
        {
            var role = context.Roles.SingleOrDefault(r => r.Name == "teacher");
            if (role != null)
            {
                var usersIdRole = context.Users.Where(u => u.Roles.Any(r => r.RoleId == role.Id)).ToArray();
                Teachers = new Dictionary<string, string>();
                foreach (var u in usersIdRole)
                {
                    Teachers.Add(u.Id, u.FirstName);
                }
                return Teachers;
            }
            Teachers = new Dictionary<string, string>();
            return Teachers;
        }

        public string teacherId { get; set; }
        [Required(ErrorMessage = "Введите имя")]
        public string name { get; set; }
    }
    public class ListLessonsViewModel
    {
        public const int ITEMS_PER_PAGE =10;

        public Lesson[] Lessons;
        public int page { get; set; } = 0;
        public int PageCount { get; set; }
    }

    public class EditLessonViewModel
    {
        [Required]
        [Display(Name = "Название")]
        public string Title { get; set; }
        public int Id { get; set; }
        public string TeacherId { get; set; }

        public Dictionary<string, string> Teachers { get; set; }
    }
}