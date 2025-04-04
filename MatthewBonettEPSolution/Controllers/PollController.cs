using DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

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
            return View();
        }

        [HttpPost]
        public IActionResult Create(
            [FromServices] PollRepository pollRepository,
            string title,
            string option1,
            string option2,
            string option3)
        {
            pollRepository.CreatePoll(title, option1, option2, option3);
            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            var polls = _pollRepository.GetPolls()
                .OrderByDescending(p => p.DateCreated) 
                .ToList();

            return View(polls);
        }

        public IActionResult Vote(int pollId, int optionNumber)
        {
            return RedirectToAction("Index");
        }
    }
}