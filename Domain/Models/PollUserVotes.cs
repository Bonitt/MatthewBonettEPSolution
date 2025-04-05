using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class PollUserVotes
    {
        public int Id { get; set; } 
        public int PollId { get; set; }
        public string UserId { get; set; }
    }

}
