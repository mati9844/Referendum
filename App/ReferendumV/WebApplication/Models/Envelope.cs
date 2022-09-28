using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Areas.Identity.Data;

namespace WebApplication.Models
{
    public enum Status
    {
        A, B, C, D, F, E
    }
    public class Envelope
    {
        public string Id { get; set; }
        [ForeignKey("WebApplicationUser")]
        public string WebApplicationUserId { get; set; }
        public virtual WebApplicationUser WebApplicationUser { get; set; }
        [ForeignKey("Vote")]
        public string VoteId { get; set; }
        public virtual Vote Vote { get; set; }
        [Timestamp]
        public DateTime Timestamp = DateTime.Now;
        public ICollection<Session> Sessions { get; set; }
        public Status Status { get; set; }


    }
}
