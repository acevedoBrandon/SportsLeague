using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsLeague.Domain.Interfaces.Repositories 
{
    public interface ISponsorRepository : IGenericRepository<Sponsor>
    {
        Task<bool> ExistsByNameAsync(string name);
        Task<bool> ExistByEmailAsync(string contactEmail);
        Task<Sponsor?> GetByNameAsync(string name);
        Task<Sponsor?> GetByEmailAsync(string contactEmail);
        Task<IEnumerable<Sponsor>> GetByCategoryAsync(SponsorCategory category);
    }
}
