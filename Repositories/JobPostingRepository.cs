using DevSpot.Models;
using DevSpot.Data;
using Microsoft.EntityFrameworkCore;

namespace DevSpot.Repositories

{
    public class JobPostingRepository : IRepository<JobPosting>
    {
        private readonly ApplicationDbContext _context;

        public JobPostingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(JobPosting entity)
        {
            await _context.JobPostings.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var jobposting = await _context.JobPostings.FindAsync(id);

            if(jobposting != null)
            {
                _context.JobPostings.Remove(jobposting);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentNullException(nameof(jobposting));
            }
            
        }

        public async Task<IEnumerable<JobPosting>> GetAllAsync()
        {
            return await _context.JobPostings.ToListAsync();
        }

        public async Task<JobPosting> GetByIdAsync(int id)
        {
            var jobposting  = await _context.JobPostings.FindAsync(id);

            if(jobposting == null)
            {
                throw new KeyNotFoundException();
            }

            return jobposting;
        }

        public async Task UpdateAsync(JobPosting entity)
        {
       
            _context.JobPostings.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
