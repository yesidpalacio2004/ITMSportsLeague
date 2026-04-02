using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsLeague.Domain.Services
{
    public class SponsorService : ISponsorService
    {
        private readonly ISponsorRepository _sponsorRepository;
        private readonly ILogger<SponsorService> _logger;

        public SponsorService(ISponsorRepository sponsorRepository, ILogger<SponsorService> logger)
        {
            _sponsorRepository = sponsorRepository;
            
            _logger = logger;
        }

        public async Task<IEnumerable<Sponsor>> GetAllAsync()
        {
            _logger.LogInformation("Retrieving all sponsors");
            return await _sponsorRepository.GetAllAsync();
        }

        public async Task<Sponsor?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving sponsor with ID: {SponsorId}", id);
            var sponsor = await _sponsorRepository.GetByIdAsync(id);
            if (sponsor == null)
                _logger.LogWarning("Sponsor with ID {SponsorId} not found", id);
            return sponsor;
        }


        public async Task<Sponsor> CreateAsync(Sponsor sponsor)
        {
            // Validación de negocio: nombre único
            var existingSponsor = await _sponsorRepository.ExistsByNameAsync(sponsor.Name);
            if (existingSponsor)
            {
                _logger.LogWarning("Sponsor with name '{SponsorName}' already exists", sponsor.Name);
                throw new InvalidOperationException(
                $"Ya existe un patrocinador con el nombre '{sponsor.Name}'");
            }
            _logger.LogInformation("Creating sponsor: {SponsorName}", sponsor.Name);
            return await _sponsorRepository.CreateAsync(sponsor);
        }

        public async Task UpdateAsync(int id, Sponsor sponsor)
        {
            var existingSponsor = await _sponsorRepository.GetByIdAsync(id);
            if (existingSponsor == null)
            {
                _logger.LogWarning("Sponsor with ID {SponsorId} not found for update", id);
                throw new KeyNotFoundException(
                $"No se encontró el patrocinador con ID {id}");
            }
            // Validación de negocio: nombre único
            var ExistWithName = await _sponsorRepository.ExistsByNameAsync(sponsor.Name);
            if (ExistWithName)
            {
                _logger.LogWarning("Sponsor with name '{SponsorName}' already exists", sponsor.Name);
                throw new InvalidOperationException(
                $"Ya existe un patrocinador con el nombre '{sponsor.Name}'");
            }
            existingSponsor.Name = sponsor.Name;
            existingSponsor.ContactEmail = sponsor.ContactEmail;
            existingSponsor.Phone = sponsor.Phone;
            existingSponsor.WebsiteUrl = sponsor.WebsiteUrl;
            existingSponsor.Category = sponsor.Category;


            _logger.LogInformation("Updating sponsor with ID: {SponsorId}", id);
            await _sponsorRepository.UpdateAsync(existingSponsor);
        }

        public async Task DeleteAsync(int id)
        {
            var existingSponsor = await _sponsorRepository.GetByIdAsync(id);
            if (existingSponsor == null)
            {
                _logger.LogWarning("Sponsor with ID {SponsorId} not found for deletion", id);
                throw new KeyNotFoundException(
                $"No se encontró el patrocinador con ID {id}");
            }
            _logger.LogInformation("Deleting sponsor with ID: {SponsorId}", id);
            await _sponsorRepository.DeleteAsync(id);
        }

    }
}
