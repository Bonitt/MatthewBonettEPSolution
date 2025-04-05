using DataAccess.Repositories;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Presentation.ActionFilter;
using System;
using DataAccess.DataContext; // Make sure to import your DbContext

namespace Presentation.Controllers
{
    public class PollController : Controller
    {
        private readonly IPollRepository _pollRepository;
        private readonly PollDbContext _context; // Inject PollDbContext

        // Constructor to inject both PollDbContext and IPollRepository
        public PollController(IPollRepository pollRepository, PollDbContext context)
        {
            _pollRepository = pollRepository;
            _context = context; // Initialize the context
        }

        [HttpPost]
        [ServiceFilter(typeof(VotingActionFilter))]
        public IActionResult Vote(int id, int option)
        {
            var userId = User.Identity.Name;

            _pollRepository.Vote(id, option, userId);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Poll
            {
                Option1VotesCount = 0,
                Option2VotesCount = 0,
                Option3VotesCount = 0,
                DateCreated = DateTime.UtcNow
            });
        }

        [HttpPost]
        public IActionResult Create(
            [FromServices] IPollRepository pollRepository,
            Poll myPoll)
        {
            myPoll.Option1VotesCount = 0;
            myPoll.Option2VotesCount = 0;
            myPoll.Option3VotesCount = 0;
            myPoll.DateCreated = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                pollRepository.CreatePoll(myPoll);
                return RedirectToAction("Index");
            }

            return View(myPoll);
        }

        public IActionResult Index()
        {
            var polls = _pollRepository.GetPolls();
            return View(polls);
        }

        public IActionResult Details(int id)
        {
            var poll = _pollRepository.GetPolls(p => p.Id == id).FirstOrDefault();
            if (poll == null)
            {
                return NotFound();
            }

            var userId = User.Identity.Name;

            var hasVoted = _context.PollUserVoted.Any(pv => pv.PollId == id && pv.UserId == userId);

            ViewBag.HasVoted = hasVoted;

            return View(poll);
        }
    }
}
