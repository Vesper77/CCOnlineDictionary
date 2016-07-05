using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineDiary.Models.CRUDViewModels
{
    public class EditUserViewModel : UserViewModel
    {
        private DiaryUser user = null;
        private ApplicationDbContext context = new ApplicationDbContext();

        [StringLength(100, ErrorMessage = "Миниум 6 символов", MinimumLength = 6)]
        public string newPassword { get; set; }

        public int[] LessonIds { get; set; }

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
            //this.PhoneNumber = user.PhoneNumber;
        }
    }
}