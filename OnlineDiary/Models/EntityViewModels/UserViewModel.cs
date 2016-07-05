using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineDiary.Models.CRUDViewModels
{
    public class UserViewModel
    {
        private DiaryUser user = null;
        private ApplicationDbContext context = new ApplicationDbContext();
        /// <summary>
        /// Номер страниц
        /// </summary>
        public int page = 0;

        public UserViewModel()
        {
            //    this.user = new DiaryUser();

        }

        public UserViewModel(DiaryUser user)
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


        //public bool IsLesson { get; set; }
        //public Dictionary<int, string> Lessons { get; set; }
        //public List<UserViewModel> Usesrs { get; set; }
        //public bool SelectedLesson { get; set; }


        public string Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        [Display(Name = "New password")]
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ParentName { get; set; }
        public string PhoneNumber { get; set; }
        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }
       
        public string ParentId { get; set; }
        public int ClassId { get; set; }

        /// <summary>
        /// Список всех ролей пользователя
        /// </summary>
        public SelectListItem[] Roles = new[] {
                new SelectListItem() { Text = "Admin", Value = "admin"},
                new SelectListItem() { Text = "Children",Value = "children"},
                new SelectListItem() { Text = "Parent",Value = "parent"},
                new SelectListItem() { Text = "Teacher",Value = "teacher"}
            };

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
        /// Находит и возвращает название роли с помощью id пользователя
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        /// <returns></returns>
        public string GetRoleNameById(string userId)
        {
            var userRoleId = context.Users.Where(i => i.Id == userId).ToArray()[0].Roles.ToArray()[0].RoleId;
            var name = context.Roles.Where(i => i.Id == userRoleId).ToArray()[0].Name;
            return name;
        }




    }
}