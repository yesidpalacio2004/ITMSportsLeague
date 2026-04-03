using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Repositories
{
    public interface ITournamentSponsorRepository : IGenericRepository<TournamentSponsor>
    {
        Task<IEnumerable<Sponsor>> GetbyTournamentAsync(int tournamentId);
        Task<bool> ExistByTournamentAndSponsorAsync(int tournamentId,int sponsor);
        Task DeleteRegisteredSponsortoTournamentAsync(int tournamentId, int sponsorId);
        Task<IEnumerable<Tournament>> GetTournamentsBySponsorIdAsync(int sponsorId);
    }
}
