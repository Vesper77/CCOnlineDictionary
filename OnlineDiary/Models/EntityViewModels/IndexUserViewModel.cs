using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineDiary.Models.CRUDViewModels
{
    public class IndexUserViewModel : UserViewModel
    {

        private DiaryUser user = null;
        private ApplicationDbContext context = new ApplicationDbContext();
        private int itemsPerPage = 10;

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

        /// <summary>
        /// Возвращает список пользователей с определенной ролью
        /// </summary>
        /// <param name="role">Роль пользователя</param>
        /// <returns></returns>
        public List<UserViewModel> GetAllUsersByRole(string role)
        {
            var list = new List<UserViewModel>();
            var users = context.Users.ToArray();
            if (role == "all")
            {
                for (int i = 0; i < context.Users.Count(); i++)
                {
                    list.Add(new UserViewModel(users[i]));
                }
            }
            else
            {
                for (int i = 0; i < context.Users.Count(); i++)
                {
                    var nameRole = GetRoleNameById(users[i].Id);
                    if (nameRole == role)
                    {
                        list.Add(new UserViewModel(users[i]));
                    }
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

            foreach (var user in context.Users.OrderBy(x => x.FirstName).Skip(page * itemsPerPage).Take(itemsPerPage))
            {
                var r = new UserViewModel(user);
                users.Add(r);
            }
            users.Sort((x, y) => x.FirstName.CompareTo(y.FirstName));
            return users;
        }

        /// <summary>
        /// Возвращает число страниц
        /// </summary>
        /// <returns></returns>
        public int getCountAllPages()
        {
            return context.Users.Count() / itemsPerPage;
        }

        /// <summary>
        /// Возвращает название класса по id ученика
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetClassNameById(string userId)
        {
            var userClassId = context.ChildrenData.Where(i => i.ChildrenId == userId).ToArray();
            var adsad = userClassId[0].SchoolClassId;
            var name = context.SchoolClasses.Where(i => i.Id == adsad).ToArray()[0].Title;
            return name;
        }
    }
}