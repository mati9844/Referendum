using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models.Validation;

namespace WebApplication.Models
{
    public class Referendum
    {
        public int Id { get; set; }
        [ForeignKey("Question")]
        public int QuestionID { get; set; }
        public Question Question { get; set; }
        [Display(Name = "Początek głosowania"), DataType(DataType.Date)]
        [DateBiggerThanOrEqualToToday]
        public DateTime StartDate { get; set; }
        [Display(Name = "Koniec głosowania"), DataType(DataType.Date)]
        [DateBiggerThanOrEqualToToday]
        public DateTime EndDate { get; set; }

    }
}
