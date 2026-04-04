using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using SportsLeague.Domain.Interfaces.Services;
using SportsLeague.Domain.Services;

namespace SportsLeague.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class SponsorController : ControllerBase
    {
        private readonly ISponsorService _sponsorService;
        private readonly ITournamentSponsorService _tournamentSponsorService;
        private readonly IMapper _mapper;
        private readonly ILogger<SponsorController> _logger;

        public SponsorController(
        ISponsorService sponsorService,
        ITournamentSponsorService tournamentSponsorService,
        IMapper mapper,
        ILogger<SponsorController> logger)
        {
            _sponsorService = sponsorService;
            _tournamentSponsorService = tournamentSponsorService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SponsorResponseDTO>>> GetAll()
        {
            var sponsors = await _sponsorService.GetAllAsync();
            var sponsorsDto = _mapper.Map<IEnumerable<SponsorResponseDTO>>(sponsors);
            return Ok(sponsorsDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SponsorResponseDTO>> GetById(int id)
        {
            var sponsor = await _sponsorService.GetByIdAsync(id);

            if (sponsor == null)
                return NotFound(new { message = $"Patrocinador con ID {id} no encontrado" });

            var sponsorDto = _mapper.Map<SponsorResponseDTO>(sponsor);
            return Ok(sponsorDto);
        }

        [HttpGet("by-name/{name}")]
        public async Task<ActionResult<SponsorResponseDTO>> GetByName(string name)
        {
            var sponsor = await _sponsorService.GetByNameAsync(name);

            if (sponsor == null)
                return NotFound(new { message = "Patrocinador no encontrado" });

            var dto = _mapper.Map<SponsorResponseDTO>(sponsor);

            return Ok(dto);
        }

        [HttpGet("by-email")]
        public async Task<ActionResult<SponsorResponseDTO>> GetByEmail([FromQuery] string email)
        {
            var sponsor = await _sponsorService.GetByEmailAsync(email);

            if (sponsor == null)
                return NotFound();

            return Ok(_mapper.Map<SponsorResponseDTO>(sponsor));
        }

        [HttpPost]
        public async Task<ActionResult<SponsorResponseDTO>> Create(SponsorRequestDTO dto)
        {
            var sponsor = _mapper.Map<Sponsor>(dto);
            var createdSponsor = await _sponsorService.CreateAsync(sponsor);
            var responseDto = _mapper.Map<SponsorResponseDTO>(createdSponsor);

            return CreatedAtAction(
                nameof(GetById),
                new { id = responseDto.Id },
                responseDto
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SponsorRequestDTO dto)
        {
            var sponsor = _mapper.Map<Sponsor>(dto);

            try
            {
                await _sponsorService.UpdateAsync(id, sponsor);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/category")]
        public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDTO dto)
        {
            try
            {
                await _sponsorService.UpdateCategoryAsync(id, dto.Category);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _sponsorService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/tournaments")]
        public async Task<ActionResult> LinkTournament(int id, TournamentSponsorRequestDTO dto)
        {
            try
            {
                await _tournamentSponsorService.AssignSponsorAsync(
                    dto.TournamentId,
                    id,
                    dto.ContractAmount
                );

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpGet("{id}/tournaments")]
        public async Task<ActionResult<IEnumerable<TournamentSponsorResponseDTO>>> GetTournaments(int id)
        {
            try
            {
                var data = await _tournamentSponsorService.GetTournamentsBySponsorAsync(id);
                var result = _mapper.Map<IEnumerable<TournamentSponsorResponseDTO>>(data);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}/tournaments/{tournamentId}")]
        public async Task<ActionResult> UnlinkTournament(int id, int tournamentId)
        {
            try
            {
                await _tournamentSponsorService.RemoveSponsorAsync(tournamentId, id);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
