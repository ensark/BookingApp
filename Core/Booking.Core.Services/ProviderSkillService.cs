using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Booking.Common.Exceptions;
using Booking.Core.Domain.DTOs;
using Booking.Core.Domain.Entities;
using Booking.Core.Services.Interfaces;
using Booking.Infrastructure.Database;

namespace Booking.Core.Services
{
    public class ProviderSkillService : IProviderSkillService
    {
        private readonly BookingDBContext _context;
        private readonly ILogger<ProviderSkillService> _logger;

        public ProviderSkillService(BookingDBContext context, ILogger<ProviderSkillService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ProviderSkillDto> CreateProviderSkillAsync(long userId, AddProviderSkillDto addProviderSkillDto, CancellationToken cancellationToken)
        {
            try
            {
                var skill = new ProviderSkill
                {
                    CreatedBy = userId.ToString(),
                    SkillName = addProviderSkillDto.SkillName,
                    UserId = userId,
                };

                _context.ProviderSkills.Add(skill);
                await _context.SaveChangesAsync(cancellationToken);

                return new ProviderSkillDto { SkillName = addProviderSkillDto.SkillName };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create skill service exception: ");
                throw;
            }
        }

        public async Task UpdateProviderSkillAsync(long userId, long providerSkillId, AddProviderSkillDto addProviderSkillDto, CancellationToken cancellationToken)
        {
            try
            {
                var providerSkill = await _context.ProviderSkills.FirstOrDefaultAsync(x => x.Id == providerSkillId, cancellationToken);

                if (providerSkill is null)
                    throw new NotFoundException($"Provider skill with id {providerSkillId} doesn't exist.");

                providerSkill.ModifiedBy = userId.ToString();
                providerSkill.ModifiedDate = DateTime.UtcNow;
                providerSkill.SkillName = addProviderSkillDto.SkillName;
                providerSkill.UserId = userId;

                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update skill service exception: ");
                throw;
            }
        }

        public async Task DeleteProviderSkillAsync(long providerSkillId, CancellationToken cancellationToken)
        {
            try
            {
                var providerSkill = await _context.ProviderSkills.FirstOrDefaultAsync(x => x.Id == providerSkillId, cancellationToken);

                if (providerSkill is null)
                    throw new NotFoundException($"Provider skill with id {providerSkillId} doesn't exist.");

                _context.ProviderSkills.Remove(providerSkill);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete provider skill service exception:");
                throw;
            }
        }

        public async Task<IEnumerable<ProviderSkillDto>> GetProviderSkillsAsync(long userId, CancellationToken cancellationToken)
        {
            try
            {
                return await _context.ProviderSkills.Where(x => x.UserId == userId)
                                                    .Select(x => new ProviderSkillDto
                                                    {
                                                        SkillName = x.SkillName
                                                    })
                                                    .ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get provider skills service exception:");
                throw;
            }
        }
    }
}
