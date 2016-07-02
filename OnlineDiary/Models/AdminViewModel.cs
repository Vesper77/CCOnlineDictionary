using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;

namespace OnlineDiary.Models
{
    public class EditUserViewModel {
        private DiaryUser user = null;
        private ApplicationDbContext context = new ApplicationDbContext();

        public EditUserViewModel()
        {
            //    this.user = new DiaryUser();

        }

        public EditUserViewModel(DiaryUser user)
        {
            this.user = user;

            this.Id = user.Id;
            this.Email = user.Email;
            this.UserName = user.UserName;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.ParentName = user.ParentName;
            this.PhoneNumber = user.PhoneNumber;
        }

        public string Id { get; set; }
        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }

        [Required]
        public string Email { get; set; }
        [Required]
        public string UserName { get; set; }
        public string ParentRoleId { get; set; }
        public bool IsLesson { get; set; }
        public Dictionary<int, string> Lessons { get; set; }
        public List<EditUserViewModel> Usesrs { get; set; }

        [Display(Name = "New password")]
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ParentName { get; set; }
        public string PhoneNumber { get; set; }
        public int[] LessonIds { get; set; }
        public bool SelectedLesson { get; set; }

        
        public DiaryUser GetUser() {
            if (user == null) {
                user = new DiaryUser()
                {
                    Email = this.Email,
                    UserName = this.UserName,
                    FirstName = this.FirstName,
                    LastName = this.LastName,
                    ParentName = this.ParentName,
                    PhoneNumber = this.PhoneNumber
                };
            }
            return user;
        }

        public Dictionary<string, string> getAllParent()
        {
            var parentRoleName = "parent";
            var parentRole = context.Roles.SingleOrDefault(r => r.Name == parentRoleName);

            if (parentRole != null)
            {
                var usersIdRole = context.Users.Where(u => u.Roles.Any(r => r.RoleId == parentRole.Id)).ToArray();

                var users = new Dictionary<string, string>();
                foreach (var u in usersIdRole)
                {
                    users.Add(u.Id, u.FirstName + " " + u.LastName + " " + u.ParentName);
                }
                return users;
            }
            return new Dictionary<string, string>();
        }

        public Dictionary<int, string> getAllLessons()
        {
            var allLesson = context.Lessons.Where(l => l.TeacherId == null).ToArray();
            if (allLesson != null)
            {
                var lessons = new Dictionary<int, string>();
                foreach (var l in allLesson)
                {
                    lessons.Add(l.Id, l.Title);
                }
                return lessons;
            }
            return new Dictionary<int, string>();
        }
    }

   
}
