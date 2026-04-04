using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Repositories
{
    public interface ITournamentSponsorRepository : IGenericRepository<TournamentSponsor>
    {
        Task<IEnumerable<TournamentSponsor>> GetByTournamentAsync(int tournamentId);
        Task<IEnumerable<TournamentSponsor>> GetBySponsorAsync(int sponsorId);
        Task<bool> ExistsAsync(int tournamentId, int sponsorId);
        Task<TournamentSponsor?> GetByTournamentAndSponsorAsync(int tournamentId, int sponsorId);
    }
}
