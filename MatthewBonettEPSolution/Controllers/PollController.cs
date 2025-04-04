using DataAccess.Repositories;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Presentation.Controllers
{
    public class PollController : Controller
    {
        private readonly PollRepository _pollRepository;

        public PollController(PollRepository pollRepository)
        {
            _pollRepository = pollRepository;
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
            [FromServices] PollRepository pollRepository, 
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
    }
}