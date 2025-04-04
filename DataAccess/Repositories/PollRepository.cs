using DataAccess.DataContext;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class PollRepository
    {

        private PollDbContext _context;

        public PollRepository(PollDbContext context)
        {
            _context = context;
        }

        public void CreatePoll(string title, string option1, string option2, string option3)
        {
            var poll = new Poll
            {
                Title = title,
                Option1Text = option1,
                Option2Text = option2,
                Option3Text = option3,
                Option1VotesCount = 0,
                Option2VotesCount = 0,
                Option3VotesCount = 0,
                DateCreated = DateTime.UtcNow
            };

            _context.Polls.Add(poll);
            _context.SaveChanges();
        }


    }
}
