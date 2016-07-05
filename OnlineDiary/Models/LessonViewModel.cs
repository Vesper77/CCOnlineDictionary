using System;
using System.Collections.Generic;
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
    }
}
