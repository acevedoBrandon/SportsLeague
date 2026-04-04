using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services
{
    public class SponsorService : ISponsorService
    {
        private readonly ILogger _logger;
        private readonly ISponsorRepository _sponsorRepository;

        public SponsorService(
            ILogger<SponsorService> logger,
            ISponsorRepository sponsorRepository)
        {
            _logger = logger;
            _sponsorRepository = sponsorRepository;
        }

        public async Task<IEnumerable<Sponsor>> GetAllAsync()
        {
            _logger.LogInformation("Retrieving all sponsors");
            return await _sponsorRepository.GetAllAsync();
        }

        public async Task<Sponsor?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving sponsor by ID: {Id}", id);
            var sponsor = await _sponsorRepository.GetByIdAsync(id);

            if (sponsor == null)
            {
                _logger.LogWarning("Sponsor with ID {Id} not found", id);
                throw new KeyNotFoundException(
                    $"No se encontró el patrocinador con ID {id}");
            }
            return sponsor;
        }

        public async Task<Sponsor?> GetByNameAsync(string name)
        {
            _logger.LogInformation("Retrieving sponsor by name: {Name}", name);
            var sponsor = await _sponsorRepository.GetByNameAsync(name);

            if (sponsor == null)
            {
                _logger.LogWarning("Sponsor with name {Name} not found", name);
                throw new KeyNotFoundException(
                    $"No se encontró el patrocinador con nombre {name}");
            }
            return sponsor;
        }

        public async Task<Sponsor?> GetByEmailAsync(string contactEmail)
        {
            _logger.LogInformation("Retrieving sponsor by email: {Email}", contactEmail);
            var sponsor = await _sponsorRepository.GetByEmailAsync(contactEmail);

            if (sponsor == null)
            {
                _logger.LogWarning("Sponsor with email {Email} not found", contactEmail);
                throw new KeyNotFoundException(
                    $"No se encontró el patrocinador con email {contactEmail}");
            }
            return sponsor;
        }

        public async Task<Sponsor> CreateAsync(Sponsor sponsor)
        {
            _logger.LogInformation("Creating new sponsor with name: {Name}", sponsor.Name);

            if (!IsValidEmail(sponsor.ContactEmail))
            {
                _logger.LogWarning("Invalid email format: {Email}", sponsor.ContactEmail);
                throw new InvalidOperationException(
                    $"El email {sponsor.ContactEmail} no tiene un formato válido");
            }

            var existsName = await _sponsorRepository.ExistsByNameAsync(sponsor.Name);
            var existsEmail = await _sponsorRepository.ExistByEmailAsync(sponsor.ContactEmail);

            if (existsName)
            {
                _logger.LogWarning("Sponsor with name {Name} already exists", sponsor.Name);
                throw new InvalidOperationException(
                    $"Ya existe un patrocinador con el nombre {sponsor.Name}");
            }

            if (existsEmail)
            {
                _logger.LogWarning("Sponsor with email {Email} already exists", sponsor.ContactEmail);
                throw new InvalidOperationException(
                    $"Ya existe un patrocinador con el email {sponsor.ContactEmail}");
            }

            return await _sponsorRepository.CreateAsync(sponsor);
        }

        public async Task UpdateAsync(int id, Sponsor sponsor)
        {
            _logger.LogInformation("Updating sponsor with ID: {Id}", id);
            var existing = await _sponsorRepository.GetByIdAsync(id);

            if (existing == null) throw new KeyNotFoundException($"No se encontró el patrocinador con ID {id}");

            if (!IsValidEmail(sponsor.ContactEmail))
            {
                _logger.LogWarning("Invalid email format: {Email}", sponsor.ContactEmail);
                throw new InvalidOperationException(
                    $"El email {sponsor.ContactEmail} no tiene un formato válido");
            }

            var existsName = await _sponsorRepository.ExistsByNameAsync(sponsor.Name);
            var emailTaken = await _sponsorRepository.ExistByEmailAsync(sponsor.ContactEmail);

            if (existsName)
            {
                _logger.LogWarning("Sponsor with name {Name} already exists", sponsor.Name);
                throw new InvalidOperationException(
                    $"Ya existe un patrocinador con el nombre {sponsor.Name}");
            }

            if (emailTaken && sponsor.ContactEmail != existing.ContactEmail)
            {
                _logger.LogWarning("Sponsor with email {Email} already exists", sponsor.ContactEmail);
                throw new InvalidOperationException(
                    $"Ya existe un patrocinador con el email {sponsor.ContactEmail}");
            }

            existing.Name = sponsor.Name;
            existing.ContactEmail = sponsor.ContactEmail;
            existing.Phone = sponsor.Phone;
            existing.WebsiteUrl = sponsor.WebsiteUrl;
            existing.Category = sponsor.Category;

            await _sponsorRepository.UpdateAsync(existing);
        }

        public async Task UpdateCategoryAsync(int id, SponsorCategory newCategory)
        {
            var existing = await _sponsorRepository.GetByIdAsync(id);

            if (existing == null)
                throw new KeyNotFoundException($"No se encontró el patrocinador con ID {id}");

            if (existing.Category == newCategory)
                throw new InvalidOperationException("El patrocinador ya tiene esa categoría");

            existing.Category = newCategory;

            await _sponsorRepository.UpdateAsync(existing);
        }

        public async Task DeleteAsync(int id)
        {
            var exists = await _sponsorRepository.ExistsAsync(id);

            if (!exists)
            {
                _logger.LogWarning("Sponsor with ID {Id} not found for deletion", id);
                throw new KeyNotFoundException(
                    $"No se encontró el patrocinador con ID {id}");
            }

            _logger.LogInformation("Deleting sponsor with ID {Id}", id);
            await _sponsorRepository.DeleteAsync(id);
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}