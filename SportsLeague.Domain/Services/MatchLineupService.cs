using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsLeague.Domain.Services
{
    

    public class MatchLineupService: IMatchLineupService
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IMatchLineupRespository _matchLineupRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly ILogger<MatchService> _logger;

        public MatchLineupService(
            IMatchRepository matchRepository,
            IMatchLineupRespository matchLineupRepository,
            IPlayerRepository playerRepository,
            ILogger<MatchService> logger)
        {
            _matchRepository = matchRepository;
            _matchLineupRepository = matchLineupRepository;
            _playerRepository = playerRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<MatchLineup>> GetByMatchAsync(int matchId)
        {
            var match = await _matchRepository.GetByIdAsync(matchId);
            if(match == null)
            {
                throw new KeyNotFoundException($"No se encontró el partido con ID {matchId}");
            }
            var matchLineup = await _matchLineupRepository.GetByMatchAsync(matchId);
            _logger.LogInformation("Retrieving lineup of match with ID: {MatchId}", matchId);
            return matchLineup;
        }

        public async Task<IEnumerable<MatchLineup>> GetByMatchAndTeamAsync(int matchId, int teamId)
        {
            var match = await _matchRepository.GetByIdAsync(matchId);
            if (match == null)
            {
                throw new KeyNotFoundException($"No se encontró el partido con ID {matchId}");
            }
            var team= await _playerRepository.GetByTeamAsync(teamId);
            if (team == null)
            {
                throw new KeyNotFoundException($"No se encontró el equipo con ID {teamId}");
            }
            var existTeamInMatch = await _matchRepository.ExistTeamInMatchAsync(matchId, teamId);
            if (!existTeamInMatch)
            {
                throw new KeyNotFoundException($"El equipo con id {teamId} no pertenece al partido con ID {matchId}");
            }
            _logger.LogInformation("Retrieving lineup of team with ID: {TeamId} from mactch whin id {MatchId}", matchId, teamId);
            return await _matchLineupRepository.GetByMatchAndTeamAsync(matchId, teamId);
        }

        public async Task<MatchLineup> CreateAsync(MatchLineup matchLineup)
        {
            var playerExist = await _playerRepository.GetByIdAsync(matchLineup.PlayerId);
            if (playerExist == null)
            {
                throw new KeyNotFoundException($"No se encontró el jugador con ID {matchLineup.PlayerId}");
            }
            var matchExist = await _matchRepository.GetByIdAsync(matchLineup.MatchId);
            if (matchExist == null)
            {
                throw new KeyNotFoundException($"No se encontró el partido con ID {matchLineup.MatchId}");
            }
            if(playerExist.TeamId != matchExist.HomeTeamId && playerExist.TeamId != matchExist.AwayTeamId)
            {
                throw new KeyNotFoundException($"El jugador no pertenece a ninguno de los equipos del partido");
            }
            var ExistPlayerInMatch = await _matchLineupRepository.ExistPlayerLineupInMatchsAsync(matchLineup.MatchId, matchLineup.PlayerId);
            if (ExistPlayerInMatch)
            {
                throw new InvalidOperationException($"El jugador ya está registrado en la alineaciónde este partido");
            }

            if(matchLineup.IsStarter)
            {
                int startersCount = await _matchLineupRepository.countStartersOfTeamFromMatch(matchLineup.MatchId, playerExist.TeamId);
                if (startersCount >= 11)
                {
                    throw new InvalidOperationException($"El equipo ya tiene 11 titulares registrados eneste partido");
                }
                
            }
            if(matchExist.Status != MatchStatus.Scheduled)
            {
                throw new InvalidOperationException($"Solo se pueden registrar alineaciones en partidos Scheduled");
            }

            return await _matchLineupRepository.CreateAsync(matchLineup);
        }

        public async Task DeleteAsync(int matchId, int matchLineupId) 
        { 
            var matchExist = await _matchRepository.GetByIdAsync(matchId);
            if (matchExist == null)
            {
                throw new KeyNotFoundException($"No se encontró el partido con ID {matchId}");
            }
            var matchLineupExist = await _matchLineupRepository.GetByIdAsync(matchLineupId);
            if (matchLineupExist == null)
            {
                throw new KeyNotFoundException($"No se encontró la alineación con ID {matchLineupId}");
            }
            if(matchLineupExist.MatchId != matchId)
            {
                throw new InvalidOperationException($"La alineación no pertenece al partido");
            }
            if (matchExist.Status != MatchStatus.Scheduled)
            {
                throw new InvalidOperationException($"Solo se pueden eliminar alineaciones en partidos Scheduled");
            }
            await _matchLineupRepository.DeleteAsync(matchLineupId);
        }

        public async Task<MatchLineup> GetById(int matchlineupId) 
        {
            var matchLineupExist = await _matchLineupRepository.GetByIdWithAndTeamAsync(matchlineupId);
            if (matchLineupExist == null)
            {
                throw new KeyNotFoundException($"No se encontró la alineación con ID {matchlineupId}");
            }
            return matchLineupExist;
        }
    }
}
