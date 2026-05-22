using SportsLeague.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsLeague.Domain.Interfaces.Services
{
    public interface IMatchLineupService
    {
        Task<IEnumerable<MatchLineup>> GetByMatchAsync(int matchId);
        Task<IEnumerable<MatchLineup>> GetByMatchAndTeamAsync(int matchId, int teamId);
        Task<MatchLineup> CreateAsync(MatchLineup matchlineup);
        Task DeleteAsync(int matchId,int matchLineupId);
        Task<MatchLineup> GetById(int matchlineupId);
    }
}
