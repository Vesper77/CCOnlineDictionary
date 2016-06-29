using OnlineDictionary.Models.People;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineDictionary.ViewModel {
    public class LoginFormViewModel  {
        public BaseUser User;

        [Required]
        public string Username { get; set; }
        [Required]
        public string Passowrd { get; set; }

        public bool LogIn() {
            BaseUser[] users = new BaseUser[] {
                new Teacher() { Username = "admin", Password = "admin" }
            };
            return true;
        }
    }
}
