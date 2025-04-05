using DataAccess.Repositories;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public IActionResult Details(int id)
        {
            var poll = _pollRepository.GetPolls(p => p.Id == id).FirstOrDefault();
            if (poll == null)
            {
                return NotFound();
            }
            return View(poll);
        }

        [HttpPost]
        public IActionResult Vote(int id, int option)
        {
            _pollRepository.Vote(id, option);
            return RedirectToAction("Index");
        }
    }
}