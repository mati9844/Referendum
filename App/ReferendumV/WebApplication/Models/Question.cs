using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models
{
    public class Question
    {
        public int Id { get; set; }

        [Display(Name = "Pytanie"), DataType(DataType.Text)]
        public string Name { get; set; }
        [Display(Name = "Odpowiedź pozytywna"), DataType(DataType.Text)]
        public string PositiveAnswer { get; set; }
        [Display(Name = "Odpowiedź negatywna"), DataType(DataType.Text)]
        public string NegativeAnswer { get; set; }


    }
}
