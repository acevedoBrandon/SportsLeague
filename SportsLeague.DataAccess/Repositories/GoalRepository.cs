using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace SportsLeague.DataAccess.Repositories
{
    public class GoalRepository : GenericRepository<Goal>, IGoalRepository
    {
        public GoalRepository(LeagueDbContext context) : base(context) { }

        public async Task<IEnumerable<Goal>> GetByMatchAsync(int matchId)
        {
            return await _dbSet
                .Where(g => g.MatchId == matchId)
                .OrderBy(g => g.Minute) //Order by ordena de forma ascendente
                                        //.OrderByDescending(g => g.Minute) //Ordena de forma descendente
                .ToListAsync();
        }

        public async Task<IEnumerable<Goal>> GetByMatchWithDetailsAsync(int matchId)
        {
            return await _dbSet
                .Where(g => g.MatchId == matchId)
                .Include(g => g.Player)
                .ThenInclude(p => p.Team)
                //.ThenInclude(t => t.TournamentTeams)
                //.ThenInclude(tt => tt.Tournament)
                .OrderBy(g => g.Minute)
                .ToListAsync();
        }
    }
}