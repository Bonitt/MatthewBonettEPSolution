using DataAccess.DataContext;
using DataAccess.Repositories;
using Domain.Models;

public class PollRepository : IPollRepository
{
    private readonly PollDbContext _context;

    public PollRepository(PollDbContext context)
    {
        _context = context;
    }

    public void CreatePoll(Poll myPoll)
    {
        _context.Polls.Add(myPoll);
        _context.SaveChanges();
    }

    public IReadOnlyList<Poll> GetPolls(Func<Poll, bool> filter = null)
    {
        var query = _context.Polls.AsQueryable();

        if (filter != null)
        {
            query = query.Where(filter).AsQueryable();
        }

        return query.OrderByDescending(p => p.DateCreated).ToList();
    }

    public void Vote(int pollId, int optionNumber, string userId)
    {
        var poll = _context.Polls.Find(pollId);
        if (poll == null) return;

        switch (optionNumber)
        {
            case 1:
                poll.Option1VotesCount++;
                break;
            case 2:
                poll.Option2VotesCount++;
                break;
            case 3:
                poll.Option3VotesCount++;
                break;
            default:
                throw new ArgumentException("Invalid option number");
        }

        var userVote = new PollUserVotes
        {
            PollId = pollId,
            UserId = userId
        };

        _context.PollUserVoted.Add(userVote);

        _context.SaveChanges();
    }
}
