using DevSpot.Repositories;
using Microsoft.AspNetCore.Mvc;
using DevSpot.Models;
using Microsoft.AspNetCore.Identity;
using NuGet.Protocol.Core.Types;
using DevSpot.ViewModel;
using Microsoft.AspNetCore.Authorization;
using NuGet.Protocol.Plugins;
using DevSpot.Constants;

namespace DevSpot.Controllers
{
    [Authorize]
    public class JobPostingsController : Controller
    {
        private readonly IRepository<JobPosting> _repository;
        private readonly UserManager<IdentityUser> _userManager;

        public JobPostingsController(IRepository<JobPosting> repository, UserManager<IdentityUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole(Roles.EMPLOYER))
            {
                IEnumerable<JobPosting> allJobpostings = (await _repository.GetAllAsync()).OrderByDescending(j => j.Posted_date);

                var userId = _userManager.GetUserId(User);

                var filtered_jobposting = allJobpostings.Where(jp => jp.UserId == userId);

                return View(filtered_jobposting);

            }

            IEnumerable<JobPosting> jobpostings = (await _repository.GetAllAsync()).OrderByDescending(j => j.Posted_date);
            return View(jobpostings);
        }

        [Authorize(Roles ="Admin, Employer")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Employer")]
        [ValidateAntiForgeryToken]
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


        [HttpDelete]
        [Authorize(Roles = "Admin, Employer")]
        public async Task<IActionResult> Delete(int id)
        {
            var jobposting = await _repository.GetByIdAsync(id);

            if(jobposting == null)
            {
                return NotFound("Specific Jobposting not found");
            }

            var userId = _userManager.GetUserId(User);

            if(User.IsInRole(Roles.ADMIN) == false && jobposting.UserId != userId)
            {
                return Forbid("This action is forbidden!");
            }

            await _repository.DeleteAsync(id);

            return Ok();
        }
    }
}
