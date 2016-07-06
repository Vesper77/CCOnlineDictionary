using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineDiary.Models.Diary
{
    public class Quadmester
    {
        [Key]
        public int Id { get; set; }
        public int Number {get; set;}
        public DateTime StartDate {get; set;}
        public DateTime EndDate {get; set;}
    }
}