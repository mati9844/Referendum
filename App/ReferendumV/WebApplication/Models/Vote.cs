using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models
{
    public class Vote
    {
        public string Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public List<Question> Questions { get; set; }

    }
}
