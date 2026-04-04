using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services
{
    public class TournamentSponsorService : ITournamentSponsorService
    {
        private readonly ITournamentSponsorRepository _tournamentSponsorRepository;
        private readonly ITournamentRepository _tournamentRepository;
        private readonly ISponsorRepository _sponsorRepository;
        private readonly ILogger<TournamentSponsorService> _logger;

        public TournamentSponsorService(
        ITournamentSponsorRepository tournamentSponsorRepository,
        ITournamentRepository tournamentRepository,
        ISponsorRepository sponsorRepository,
        ILogger<TournamentSponsorService> logger)
        {
            _tournamentSponsorRepository = tournamentSponsorRepository;
            _tournamentRepository = tournamentRepository;
            _sponsorRepository = sponsorRepository;
            _logger = logger;
        }

        public async Task AssignSponsorAsync(int tournamentId, int sponsorId, decimal contractAmount)
        {
            var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
            if (tournament == null)
            {
                _logger.LogWarning("Torneo no encontrado: {TournamentId}", tournamentId);
                throw new KeyNotFoundException($"Torneo con ID {tournamentId} no existe");
            }
                
            var sponsor = await _sponsorRepository.GetByIdAsync(sponsorId);
            if (sponsor == null)
            {
                _logger.LogWarning("Sponsor no encontrado: {SponsorId}", sponsorId);
                throw new KeyNotFoundException($"Sponsor con ID {sponsorId} no existe");
            }

            var exists = await _tournamentSponsorRepository.ExistsAsync(tournamentId, sponsorId);
            if (exists)
            {
                _logger.LogWarning("Relación duplicada: Sponsor {SponsorId} ya está en torneo {TournamentId}", sponsorId, tournamentId);
                throw new InvalidOperationException("El sponsor ya está vinculado a este torneo");
            }

            if (contractAmount <= 0)
            {
                _logger.LogWarning("Monto inválido: {Amount}", contractAmount);
                throw new InvalidOperationException("El monto del contrato debe ser mayor a 0");
            }

            var tournamentSponsor = new TournamentSponsor
            {
                TournamentId = tournamentId,
                SponsorId = sponsorId,
                ContractAmount = contractAmount,
                JoinedAt = DateTime.UtcNow
            };

            await _tournamentSponsorRepository.CreateAsync(tournamentSponsor);
        }

        public async Task<IEnumerable<TournamentSponsor>> GetSponsorsByTournamentAsync(int tournamentId)
        {
            _logger.LogInformation("Obteniendo sponsors del torneo {TournamentId}", tournamentId);

            var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
            if (tournament == null)
            {
                _logger.LogWarning("Torneo no encontrado: {TournamentId}", tournamentId);
                throw new KeyNotFoundException($"Torneo con ID {tournamentId} no existe");
            }

            return await _tournamentSponsorRepository.GetByTournamentAsync(tournamentId);
        }

        public async Task<IEnumerable<TournamentSponsor>> GetTournamentsBySponsorAsync(int sponsorId)
        {
            _logger.LogInformation("Obteniendo torneos del sponsor {SponsorId}", sponsorId);

            var sponsor = await _sponsorRepository.GetByIdAsync(sponsorId);
            if (sponsor == null)
            {
                _logger.LogWarning("Sponsor no encontrado: {SponsorId}", sponsorId);
                throw new KeyNotFoundException($"Sponsor con ID {sponsorId} no existe");
            }

            return await _tournamentSponsorRepository.GetBySponsorAsync(sponsorId);
        }

        public async Task RemoveSponsorAsync(int tournamentId, int sponsorId)
        {
            _logger.LogInformation("Intentando eliminar sponsor {SponsorId} del torneo {TournamentId}", sponsorId, tournamentId);

            var entity = await _tournamentSponsorRepository
                .GetByTournamentAndSponsorAsync(tournamentId, sponsorId);

            if (entity == null)
            {
                _logger.LogWarning("Relación no encontrada: Sponsor {SponsorId} en torneo {TournamentId}", sponsorId, tournamentId);
                throw new KeyNotFoundException("La relación no existe");
            }

            await _tournamentSponsorRepository.DeleteAsync(entity.Id);

            _logger.LogInformation("Sponsor {SponsorId} eliminado del torneo {TournamentId}", sponsorId, tournamentId);
        }
    }
}
