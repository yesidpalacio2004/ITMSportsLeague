using SportsLeague.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsLeague.Domain.Interfaces.Repositories
{
    public interface IMatchLineupRespository: IGenericRepository<MatchLineup>
    {
        Task<IEnumerable<MatchLineup>> GetByMatchAsync(int matchId);
        Task<IEnumerable<MatchLineup>> GetByMatchAndTeamAsync(int matchId, int teamId);
        Task<bool> ExistPlayerLineupInMatchsAsync(int matchId, int playerId);
        Task<int> countStartersOfTeamFromMatch(int matchId, int teamId);
        Task<MatchLineup?> GetByIdWithAndTeamAsync(int lineupid);
    }
}