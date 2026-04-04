using SportsLeague.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsLeague.Domain.Interfaces.Services
{
    public interface ITournamentSponsorService
    {
        Task<IEnumerable<TournamentSponsor>> GetSponsorsByTournamentAsync(int tournamentId);
        Task<IEnumerable<TournamentSponsor>> GetTournamentsBySponsorAsync(int sponsorId);
        Task AssignSponsorAsync(int tournamentId, int sponsorId, decimal contractAmount);
        Task RemoveSponsorAsync(int tournamentId, int sponsorId);
    }
}
