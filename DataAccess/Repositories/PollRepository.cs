using DataAccess.DataContext;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Repositories
{
    public class PollRepository
    {
        private readonly PollDbContext _context;

        public PollRepository(PollDbContext context)
        {
            _context = context;
        }

        public void CreatePoll(Poll myPoll) {
            _context.Polls.Add(myPoll);
            _context.SaveChanges();
        }
        public IReadOnlyList<Poll> GetPolls()
        {
            return _context.Polls
                .OrderByDescending(p => p.DateCreated)
                .ToList();
        }
    }
}