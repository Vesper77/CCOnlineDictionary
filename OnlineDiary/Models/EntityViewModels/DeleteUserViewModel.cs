using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineDiary.Models.CRUDViewModels
{
    public class DeleteUserViewModel : UserViewModel
    {
        private DiaryUser user = null;
        private ApplicationDbContext context = new ApplicationDbContext();
        private int itemsPerPage = 10;

        public DeleteUserViewModel()
        {
            //    this.user = new DiaryUser();
        }

        public DeleteUserViewModel(DiaryUser user)
        {
            this.user = user;

            this.Id = user.Id;
            this.Email = user.Email;
            this.UserName = user.UserName;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.ParentName = user.ParentName;
            //this.PhoneNumber = user.PhoneNumber;
        }
    }
}