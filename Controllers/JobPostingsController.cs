using DevSpot.Repositories;
using Microsoft.AspNetCore.Mvc;
using DevSpot.Models;
using Microsoft.AspNetCore.Identity;
using NuGet.Protocol.Core.Types;
using DevSpot.ViewModel;

namespace DevSpot.Controllers
{
    public class JobPostingsController : Controller
    {
        private readonly IRepository<JobPosting> _repository;
        private readonly UserManager<IdentityUser> _userManager;

        public JobPostingsController(IRepository<JobPosting> repository, UserManager<IdentityUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<JobPosting> jobpostings = await _repository.GetAllAsync();
            return View(jobpostings);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobPosting jobposting)
        {
            var currentUserId = _userManager.GetUserId(User);
            Console.WriteLine($"User ID: {currentUserId}");

            if (currentUserId == null) 
            {
                return Unauthorized("User must be logged in to create a job posting.");
            }

            var currentUser = await _userManager.FindByIdAsync(currentUserId);

            if (currentUser == null)
            {
                return Unauthorized("User not found!");
            }
            jobposting.User = currentUser;

            // Assign the UserId (this happens after binding, which is why ModelState is invalid initially)
            jobposting.UserId = currentUserId;

            // Clear the validation error for UserId and revalidate
            ModelState.ClearValidationState(nameof(jobposting.UserId));
            ModelState.ClearValidationState(nameof(jobposting.User));
            TryValidateModel(jobposting);

            if (ModelState.IsValid)
            {
                Console.WriteLine($"ModelState IsValid: {ModelState.IsValid}");
                await _repository.AddAsync(jobposting);
                return RedirectToAction(nameof(Index));
            }

            //Log all th errors for the ModelState
            foreach(var key in ModelState.Keys)
            {
                var errors = ModelState[key].Errors;
                foreach (var error in errors)
                {
                    Console.WriteLine($"Error for {key}: {error.ErrorMessage}");
                }
            }
            
            // If ModelState is invalid, redisplay the form with error messages
            return View(jobposting);
        }
    }
}
