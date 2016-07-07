using OnlineDiary.Models.Diary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineDiary.Models
{
    public class QuadMesterViewModel
    {
        private ApplicationDbContext context = new ApplicationDbContext();

        public int Id { get; set; }
        public int Number { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public QuadMesterViewModel() { }

        public QuadMesterViewModel(QuadMester qMester)
        {
            this.Id = qMester.Id;
            this.Number = qMester.Number;
            this.StartDate = qMester.StartDate;
            this.EndDate = qMester.EndDate;
        }

    }
}