using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;


namespace SportsLeague.Domain.Interfaces.Services
{
    public interface ISponsorService
    {
        Task<IEnumerable<Sponsor>> GetAllAsync();
        Task<Sponsor?> GetByIdAsync(int id);
        Task<Sponsor?> GetByNameAsync(string name);
        Task<Sponsor?> GetByEmailAsync(string contactEmail);
        Task<Sponsor> CreateAsync(Sponsor sponsor);
        Task UpdateAsync(int id, Sponsor Sponsor);
        Task UpdateCategoryAsync(int id, SponsorCategory newCategory);
        Task DeleteAsync(int id);
    }
}
