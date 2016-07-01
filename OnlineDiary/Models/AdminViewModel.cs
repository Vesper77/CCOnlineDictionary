using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineDiary.Models
{
    public class EditUserViewModel {
        private DiaryUser user = null;

        public EditUserViewModel()
        {
            this.user = new DiaryUser();
            
        }
        public EditUserViewModel(DiaryUser user)
        {
            this.user = user;

            this.Id = user.Id;
            this.Email = user.Email;
            this.UserName = user.UserName;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.PhoneNumber = user.PhoneNumber;
        }

        public string Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string UserName { get; set; }

        [Display(Name ="New password")]
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }

        public DiaryUser GetUser() {
            if (user == null) {
                user = new DiaryUser()
                {
                    Email = this.Email,
                    FirstName = this.FirstName,
                    LastName = this.LastName,
                    PhoneNumber = this.PhoneNumber
                };                
            }
            return user;
        }
    }
}
