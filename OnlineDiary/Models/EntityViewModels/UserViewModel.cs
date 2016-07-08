using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using OnlineDiary.DAL;
using OnlineDiary.Models;
using OnlineDiary.Models.CRUDViewModels;
using OnlineDiary.Models.Diary;
using OnlineDiary.Models.People;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
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
        }


        public string Id { get; set; }
        [Required(ErrorMessage = "Введите логин")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Введите email")]
        [EmailAddress(ErrorMessage = "Ввдеите нормальный емейл")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Введите имя")]
        [RegularExpression(@"^[а-яА-ЯЁё]+$", ErrorMessage = "Используйте буквы")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Введите фамилию")]
        [RegularExpression(@"^[а-яА-ЯЁё]+$", ErrorMessage = "Используйте буквы")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Введите отчество")]
        [RegularExpression(@"^[а-яА-ЯЁё]+$", ErrorMessage = "Используйте буквы")]
        public string ParentName { get; set; }
        // public string PhoneNumber { get; set; }
        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; } = "all";

        public string ParentId { get; set; }
        public int ClassId { get; set; }

        /// <summary>
        /// Список всех ролей пользователя
        /// </summary>
        public SelectListItem[] Roles = new[] {
                //new SelectListItem() { Text = "Все", Value = "all"},
                new SelectListItem() { Text = "Администратор", Value = "admin"},
                new SelectListItem() { Text = "Ученик",Value = "children"},
                new SelectListItem() { Text = "Родитель",Value = "parent"},
                new SelectListItem() { Text = "Учитель",Value = "teacher"}
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
                    //  PhoneNumber = this.PhoneNumber
                };
            }
            return user;
        }

        /// <summary>
        /// Возвращает список всех элементов из таблицы Lesson
        /// </summary>
        /// <returns></returns>
        public Lesson[] GetAllLessons(string id = "")
        {
            var allLessons = context.Lessons.Where(l => l.TeacherId == null || l.TeacherId == id).ToArray();
            if (id != "")
            {
                return allLessons;
            }
            if (allLessons != null)
                return allLessons;
            return new Lesson[0];
        }

        /// <summary>
        /// Возвращает список незанятых предметов
        /// </summary>
        /// <returns></returns>
        public Lesson[] GetNotBusyLessons()
        {
            var allLessons = context.Lessons.Where(l => l.TeacherId == null && l.Title != null).ToArray();
            if (allLessons != null)
            {
                return allLessons;
            }
            return new Lesson[0];
        }

        /// <summary>
        /// Возвращает имя школьного класса с помощью id ученика
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetClassNameByChildrenId(string id)
        {
            var arrChild = context.ChildrenData.Where(i => i.ChildrenId == id).ToArray();
            if (arrChild != null)
            {
                var schClassId = arrChild.First().SchoolClassId;
                var nameClass = context.SchoolClasses.Where(i => i.Id == schClassId).ToArray();
                if (nameClass != null)
                {
                    return nameClass.First().Title;
                }
            }
            return "";
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
        public string GetRoleNameById(string userId, bool isLocalization = false)
        {
            var user = context.Users.FirstOrDefault(i => i.Id == userId);
            if (user != null)
            {
                var role = user.Roles.FirstOrDefault();
                if (role != null)
                {
                    var name = context.Roles.First(i => i.Id == role.RoleId).Name;
                    if (isLocalization)
                    {
                        return GetRoleTitle(name);
                    }
                    return name;
                }
            }
            return "none";

        }

        /// <summary>
        /// Создает новый школьный класс
        /// </summary>
        /// <param name="className">название школьного класса</param>
        public void CreateClas(string className)
        {
            SchoolClass schClass = new SchoolClass();
            schClass.Title = className;
            context.SchoolClasses.Add(schClass);
            context.SaveChanges();
        }

        public Lesson[] GetTeacherLessons(string id)
        {
            var allLessons = context.Lessons.Where(l => l.TeacherId == id).ToArray();
            if (allLessons != null)
            {
                return allLessons;
            }
            return new Lesson[0];
        }
        private string GetRoleTitle(string role)
        {
            switch (role)
            {
                case "all": return "Все";
                case "children": return "Ученик";
                case "parent": return "Родитель";
                case "teacher": return "Учитель";
                case "admin": return "Администратор";
                default: return "Человек";
            }
        }

    }
}