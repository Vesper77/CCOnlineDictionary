using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineDiary.Models.CRUDViewModels
{
    public class IndexUserViewModel : UserViewModel
    {

        public const int ITEMS_PER_PAGE = 10;

        private DiaryUser user = null;
        private ApplicationDbContext context = new ApplicationDbContext();

        public IndexUserViewModel()
        {
            //    this.user = new DiaryUser();
        }

        public IndexUserViewModel(DiaryUser user)
        {
            this.user = user;

            this.Id = user.Id;
            this.Email = user.Email;
            this.UserName = user.UserName;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.ParentName = user.ParentName;
           // this.PhoneNumber = user.PhoneNumber;
        }

        public int CountPages { get; set; }

        /// <summary>
        /// Возвращает список пользователей с определенной ролью
        /// </summary>
        /// <param name="role">Роль пользователя</param>
        /// <returns></returns>
        public List<UserViewModel> GetAllUsersByRole(string role)
        {
            var list = new List<UserViewModel>();
            if (role == "all"  || String.IsNullOrEmpty(role))
            {
                var users = context.Users.OrderBy(u => u.FirstName).Skip(page * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE).ToArray();
                CountPages = (int)Math.Round((float)users.Count() / (float)ITEMS_PER_PAGE);
                for (int i = 0; i < users.Length; i++)
                {
                    list.Add(new UserViewModel(users[i]));
                }
            }
            else
            {
                var roleId = context.Roles.FirstOrDefault(r => r.Name == role);
                var users = context.Users.Where(u => u.Roles.Any(r => r.RoleId == roleId.Id)).ToArray();
                CountPages = (int)Math.Ceiling((float)users.Count() / (float)ITEMS_PER_PAGE);
                for (int i = 0; i < users.Length; i++)
                {
                    list.Add(new UserViewModel(users[i]));
                }
            }
            if (list != null && list.Count > 0) {
                list.Sort((x, y) => x.FirstName.CompareTo(y.FirstName));
            }
            
            return list;
        }


        /// <summary>
        /// Возвращает все пользователей из таблицы User
        /// </summary>
        /// <returns></returns>
        public List<UserViewModel> getAllUsers()
        {
            var users = new List<UserViewModel>();

            foreach (var user in context.Users.OrderBy(x => x.FirstName).Skip(page * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE))
            {
                var r = new UserViewModel(user);
                users.Add(r);
            }
            users.Sort((x, y) => x.FirstName.CompareTo(y.FirstName));
            return users;
        }
        /// <summary>
        /// Возвращает название класса по id ученика
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetClassNameById(string userId)
        {
            var classId = context.ChildrenData.Where(i => i.ChildrenId == userId).FirstOrDefault();
            if (classId != null) {
                var name = context.SchoolClasses.Where(i => i.Id == classId.SchoolClassId).FirstOrDefault();
                if (name != null)
                {
                    return name.Title;
                }
            }
            return "";            
        }
    }
}