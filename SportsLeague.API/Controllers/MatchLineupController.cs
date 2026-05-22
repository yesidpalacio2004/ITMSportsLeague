using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.API.Controllers
{
    [Route("api/match/{matchId}/lineup")]
    [ApiController]
    public class MatchLineupController : ControllerBase
    {
        private readonly IMatchLineupService _matchLineupService;
        private readonly IMapper _mapper;


        public MatchLineupController(
            IMatchLineupService matchLineupService, IMapper mapper)
        {
            _matchLineupService = matchLineupService;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatchLineupResponseDTO>>> GetLineup(int matchId)
        {
            try
            {
                var lineup = await _matchLineupService.GetByMatchAsync(matchId);
                return Ok(_mapper.Map<IEnumerable<MatchLineupResponseDTO>>(lineup));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("team/{teamId}")]
        public async Task<ActionResult<IEnumerable<MatchLineupResponseDTO>>> GetLineupByTeam(int matchId, int teamId)
        {
            try 
            {
                var lineup = await _matchLineupService.GetByMatchAndTeamAsync(matchId, teamId);
                return Ok(_mapper.Map<IEnumerable<MatchLineupResponseDTO>>(lineup));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("{lineupId}")]
        public async Task<ActionResult<MatchLineupResponseDTO>> GetById(int lineupId)
        {
            try
            {
                var matchLineup = await _matchLineupService.GetById(lineupId);
                return Ok(_mapper.Map<MatchLineupResponseDTO>(matchLineup));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int matchId, int lineupId)
        {
            try
            {
                await _matchLineupService.DeleteAsync(matchId, lineupId);
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

        [HttpPost]
        public async Task<ActionResult<MatchLineupResponseDTO>> Create(int matchId, MatchLineupRequestDTO dto)
        {
            try
            {
                MatchLineup matchLineup = new MatchLineup
                {
                    MatchId = matchId,
                    PlayerId = dto.PlayerId,
                    IsStarter = dto.IsStarter,
                    Position = dto.Position
                };
                var created = await _matchLineupService.CreateAsync(matchLineup);
                var responseDTO= _mapper.Map<MatchLineupResponseDTO>(created);
                return CreatedAtAction(
                    nameof(GetById),
                     new { lineupId = responseDTO.Id, matchId=matchId},
                     responseDTO);
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
    }
}
