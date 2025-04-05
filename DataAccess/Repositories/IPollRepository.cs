using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface IPollRepository
    {
        void CreatePoll(Poll poll);
        IReadOnlyList<Poll> GetPolls(Func<Poll, bool> filter = null);
        void Vote(int pollId, int optionNumber, string userId);
    }
}
