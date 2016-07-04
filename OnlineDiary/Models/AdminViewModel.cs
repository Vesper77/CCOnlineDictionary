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
    public class EditUserViewModel
    {
        private DiaryUser user = null;
        private ApplicationDbContext context = new ApplicationDbContext();
        /// <summary>
        /// Номер страниц
        /// </summary>
        public int page = 0;
        private int itemsPerPage = 10;

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
        public string ParentId { get; set; }
        public int ClassId { get; set; }
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

        /// <summary>
        /// Возвращает список всех элементов из таблице SchoolClass
        /// </summary>
        /// <returns>
        /// </returns>
        public Dictionary<int, string> GetAllClass()
        {
            var allClass = context.SchoolClasses.ToArray();
            if (allClass != null)
            {
                var classes = new Dictionary<int, string>();
                foreach (var l in allClass)
                {
                    classes.Add(l.Id, l.Title);
                }
                return classes;
            }
            return new Dictionary<int, string>();
        }

        /// <summary>
        /// Возвращает пользователя
        /// </summary>
        /// <returns></returns>
        public DiaryUser GetUser()
        {
            if (user == null)
            {
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

        /// <summary>
        /// Возвращает список всех пользователей из таблицы User с ролью Parent
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetAllParent()
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

        /// <summary>
        /// Возвращает список всех элементов из таблицы Lesson
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, string> GetAllLessons()
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

        /// <summary>
        /// Возвращает все пользователей из таблицы User
        /// </summary>
        /// <returns></returns>
        public List<EditUserViewModel> getAllUsers()
        {
            var users = new List<EditUserViewModel>();
            
            foreach (var user in context.Users.OrderBy(x => x.FirstName).Skip(page * itemsPerPage).Take(itemsPerPage))
            {
                var r = new EditUserViewModel(user);
                users.Add(r);
            }
            users.Sort((x, y) => x.FirstName.CompareTo(y.FirstName));
            return users;
        }

        /// <summary>
        /// Находит и возвращает название роли с помощью id пользователя
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        /// <returns></returns>
        public string GetRoleNameById(string userId)
        {
            //var userRoleId = context.Users.Where(i => i.Id == userId).ToArray()[0].Roles.ToArray()[0].RoleId;
            //var name = context.Roles.Where(i => i.Id == userRoleId).ToArray()[0].Name;
            //return name;
            return "";
        }

        /// <summary>
        /// Сортирует и возвращает список пользователей с определенной ролью
        /// </summary>
        /// <param name="role">Роль пользователя</param>
        /// <returns></returns>
        public List<EditUserViewModel> SortUserListByRole(string role)
        {
            var list = new List<EditUserViewModel>();
            var users = context.Users.ToArray();
            for (int i = 0; i < context.Users.Count(); i++)
            {
                var nameRole = GetRoleNameById(users[i].Id);
                if (nameRole == role)
                {
                    list.Add(new EditUserViewModel(users[i]));
                }
            }
            list.Sort((x, y) => x.FirstName.CompareTo(y.FirstName));
            return list;
        }
        public int getCountAllPages() {
            return context.Users.Count() / itemsPerPage;
        }
    }
}
