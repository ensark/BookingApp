using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetTopologySuite;
using GeoAPI.Geometries;
using Newtonsoft.Json;
using Booking.Core.Domain.DTOs;
using Booking.Core.Domain.Entities;
using Booking.Core.Services.Interfaces;
using Booking.Infrastructure.Database;
using Booking.Common.Exceptions;
using Booking.Core.Domain.Enums;
using Booking.Common.RecurrenceProcessor;
using Location = Booking.Core.Domain.Entities.Location;
using Booking.Common.Shared;
using Booking.Core.Domain.Queries;

namespace Booking.Core.Services
{
    public class ProviderService : IProviderService
    {
        private readonly BookingDBContext _context;
        private readonly ILogger<ProviderService> _logger;
        private readonly IGeometryFactory _geometryFactory;
        private readonly IConfiguration _configuration;
        public readonly string _workingHoursStart;
        public readonly string _workingHoursEnd;

        public ProviderService(BookingDBContext context, ILogger<ProviderService> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            _configuration = configuration.GetSection(ScheduleSettingsConfig.SCHEDULE_SETTINGS_SECTION);
            _workingHoursStart = _configuration.GetValue<string>(ScheduleSettingsConfig.WORKING_HOURS_START);
            _workingHoursEnd = _configuration.GetValue<string>(ScheduleSettingsConfig.WORKING_HOURS_END);
        }

        public async Task<ProviderDto> CreateProviderAsync(long userId, AddProviderDto addProviderDto, CancellationToken cancellationToken)
        {
            try
            {
                var provider = Map(addProviderDto, userId);

                _logger.LogInformation($"Create provider: {provider}");
                _context.Providers.Add(provider);
                await _context.SaveChangesAsync(cancellationToken);

                return Map(provider.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create provider service exception: ");
                throw;
            }
        }

        public async Task UpdateProviderAsync(long id, long userId, AddProviderDto updateProviderDto, CancellationToken cancellationToken)
        {
            try
            {
                var provider = await _context.Providers.Include(x => x.Location)
                                                       .Include(x => x.ScheduleSettings)
                                                       .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

                if (provider is null)
                    throw new NotFoundException($"Provider with id {id} does not exist.");

                provider.ModifiedBy = userId.ToString();
                provider.ModifiedDate = DateTime.UtcNow;
                provider.Title = updateProviderDto.Title;
                provider.Description = updateProviderDto.Description;
                provider.ServiceType = updateProviderDto.ServiceType;
                provider.ProfessionType = updateProviderDto.ProfessionType;
                provider.PricePerSession = updateProviderDto.PricePerSession;
                provider.NumberOfParticipants = updateProviderDto.NumberOfParticipants;
                provider.FiveSessionsDiscount = updateProviderDto.FiveSessionsDiscount;
                provider.TenSessionsDiscount = provider.TenSessionsDiscount;
                provider.Location.Name = updateProviderDto.Location.Name;
                provider.Location.GeoLocation = _geometryFactory.CreatePoint(new Coordinate(updateProviderDto.Location.Longitude, updateProviderDto.Location.Latitude));
                provider.Location.ModifiedBy = userId.ToString();
                provider.Location.ModifiedDate = DateTime.UtcNow;
                provider.ScheduleSettings.ModifiedBy = userId.ToString();
                provider.ScheduleSettings.ModifiedDate = DateTime.UtcNow;
                provider.ScheduleSettings.ScheduledDaysOfWeek = ScheduledDaysOfTheWeek(updateProviderDto.ScheduleSettings.DaysOfWeek);
                provider.ScheduleSettings.StartDate = updateProviderDto.ScheduleSettings.StartDate;
                provider.ScheduleSettings.EndDate = updateProviderDto.ScheduleSettings.EndDate;
                provider.ScheduleSettings.WorkingHoursStart = updateProviderDto.ServiceType == ServiceType.Single ? updateProviderDto.ScheduleSettings.WorkingHoursStart : _workingHoursStart;
                provider.ScheduleSettings.WorkingHoursEnd = updateProviderDto.ServiceType == ServiceType.Single ? updateProviderDto.ScheduleSettings.WorkingHoursEnd : _workingHoursEnd;
                provider.ScheduleSettings.DurationOfSessionInMinutes = updateProviderDto.ScheduleSettings.DurationOfSessionInMinutes;
                provider.ScheduleSettings.GapBetweenSessionsInMinutes = updateProviderDto.ScheduleSettings.GapBetweenSessionsInMinutes;
                provider.ScheduleSettings.ScheduledTimeSlots = ScheduledTimeSlots(updateProviderDto.ScheduleSettings.SelectedTimeSlots);

                _logger.LogInformation($"Update provider: {provider}");

                _context.Providers.Update(provider);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Create provider service exception:");
                throw;
            }
        }

        public async Task DeleteProviderAsync(long id, CancellationToken cancellationToken)
        {
            try
            {
                var deleteProvider = await _context.Providers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

                if (deleteProvider is null)
                    throw new NotFoundException($"Provider with id {id} doesn't exist.");

                _context.Providers.Remove(deleteProvider);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete provider service exception:");
                throw;
            }
        }

        public async Task<IEnumerable<ProviderDto>> GetProvidersByServiceTypeAsync(long userId, ServiceType serviceType, CancellationToken cancellationToken)
        {
            try
            {
                return await _context.Providers.Where(x => x.UserId == userId && x.ServiceType == serviceType)
                                               .OrderByDescending(x => x.Id)
                                               .Select(x => new ProviderDto
                                               {
                                                   ProviderName = $"{x.User.FirstName} {x.User.LastName}",                                                  
                                                   Title = x.Title,
                                                   Description = x.Description,
                                                   ServiceType = x.ServiceType,
                                                   ProfessionType = x.ProfessionType,
                                                   PricePerSession = x.PricePerSession,
                                                   NumberOfParticipants = x.NumberOfParticipants,
                                                   FiveSessionsDiscount = x.FiveSessionsDiscount,
                                                   TenSessionsDiscount = x.TenSessionsDiscount,
                                                   LocationName = x.Location.Name,
                                                   Longitude = x.Location.GeoLocation.Coordinate.X,
                                                   Latitude = x.Location.GeoLocation.Coordinate.Y,
                                                   ScheduleSettings = new ScheduleSettingsDto
                                                   {
                                                       StartDate = x.ScheduleSettings.StartDate,
                                                       EndDate = x.ScheduleSettings.EndDate,
                                                       ScheduledDaysOfWeek = JsonConvert.DeserializeObject<List<string>>(x.ScheduleSettings.ScheduledDaysOfWeek),
                                                       WorkingHoursStart = x.ScheduleSettings.WorkingHoursStart,
                                                       WorkingHoursEnd = x.ScheduleSettings.WorkingHoursEnd,
                                                       DurationOfSessionInMinutes = x.ScheduleSettings.DurationOfSessionInMinutes,
                                                       GapBetweenSessionsInMinutes = x.ScheduleSettings.GapBetweenSessionsInMinutes,
                                                       ScheduledTimeSlots = JsonConvert.DeserializeObject<List<ScheduledTimeSlotsDto>>(x.ScheduleSettings.ScheduledTimeSlots),
                                                   }
                                               }).ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get providers service exception:");
                throw;
            }
        }

        public async Task<PagedResult<ProviderSearchListDto>> GetAllProvidersAsync(SearchQuery searchQuery, CancellationToken cancellationToken)
        {
            try
            {                           
                var userProviders = await _context.Users.Where(x => x.UserType == UserType.ServiceProvider)
                                                        .Select(x => new ProviderSearchListDto
                                                        {
                                                            Name = $"{x.FirstName} {x.LastName}",
                                                            City = $"{x.Address.City} {x.Address.Country}",
                                                            Title = x.UserType == UserType.ServiceProvider ? x.Providers.Select(a => a.Title).FirstOrDefault() : "",
                                                            ProfileImage = x.Attachments.Any() ? x.Attachments.Where(a => a.DocumentType == DocumentType.ProfileImage).Select(a => a.Data).FirstOrDefault() : null,
                                                            Rank = x.Reviews.Any() ? decimal.Round(x.Reviews.Sum(a => a.Grade) / x.Reviews.Count, 2, MidpointRounding.AwayFromZero) : 0
                                                        }).ToListAsync(cancellationToken);

                var query = userProviders.AsQueryable();

                if (!string.IsNullOrEmpty(searchQuery.QuickSearch))
                {
                    query = query.Where(x => x.Name.ToLower().Contains(searchQuery.QuickSearch.ToLower()));
                }

                var totaProviders = query.Count();

                query = query.Skip(searchQuery.Skip)
                             .Take(searchQuery.Take);

                var pagedResult = new PagedResult<ProviderSearchListDto>
                {
                    CurrentPage = searchQuery.Page,
                    TotalPages = searchQuery.CalculatePages(totaProviders),
                    TotalItems = totaProviders,
                    Items = query
                };

                return pagedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get all providers service exception:");
                throw;
            }
        }

        public async Task<ProviderDto> GetProviderByIdAsync(long id, CancellationToken cancellationToken)
        {
            try
            {
                var provider = await _context.Providers.Include(x => x.Location)
                                                       .Include(x => x.ScheduleSettings)
                                                       .Include(x => x.User)
                                                       .Where(x => x.Id == id)
                                                       .FirstOrDefaultAsync(cancellationToken);

                if (provider is null)
                    throw new NotFoundException($"Provider with id {id} does not exist.");

                return Map(provider.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get provider service exception");
                throw;
            }
        }

        public IList<string> CalculateTimeSlots(long userId, CalculateTimeSlotsValuesDto calculateTimeSlotsValuesDto)
        {
            try
            {
                var startWorkingHours = DateTime.Parse(calculateTimeSlotsValuesDto.WorkingHoursStart).Hour;
                var endWorkingHours = DateTime.Parse(calculateTimeSlotsValuesDto.WorkingHoursEnd).Hour;

                if (startWorkingHours < 0 && startWorkingHours > 23)
                    throw new Exception("Start working hours must not be great then 0 and less then 24!");

                if (endWorkingHours < 0 && endWorkingHours > 23)
                    throw new Exception("End working hours must not be great then 0 and less then 24!");

                if (startWorkingHours >= endWorkingHours)
                    throw new Exception("Start working hours must be less than end working hours");

                if (calculateTimeSlotsValuesDto.DurationOfSessionInMinutes <= 0)
                    throw new Exception("Duration session must not be 0");

                var calculatedTimeSlots = new List<string>();

                var startDay = DateTime.Today.AddHours(startWorkingHours);
                var endDay = DateTime.Today.AddHours(endWorkingHours);

                var ts = endDay - startDay;
                var hoursBetween = Enumerable.Range(0, (int)ts.TotalHours)
                                             .Select(i => startDay.AddHours(i));

                var providerScheduledTimeSlots = _context.Providers.Include(x => x.ScheduleSettings).Where(x => x.UserId == userId).Select(x => x.ScheduleSettings.ScheduledTimeSlots).ToList();

                if (providerScheduledTimeSlots.Any())
                {
                    foreach (var scheduledTimeSlot in providerScheduledTimeSlots)
                    {
                        var scheduleTime = JsonConvert.DeserializeObject<List<ScheduledTimeSlotsDto>>(scheduledTimeSlot).Select(x => x.Time);

                        for (var appointment = startDay; appointment < endDay; appointment = appointment.AddMinutes(calculateTimeSlotsValuesDto.DurationOfSessionInMinutes + calculateTimeSlotsValuesDto.GapBetweenSessionsInMinutes))
                        {
                            if (!scheduleTime.Contains(appointment.ToString("HH:mm")))
                            {
                                calculatedTimeSlots.Add(appointment.ToString("HH:mm"));
                            }
                        }
                    }
                }
                else
                {
                    for (var appointment = startDay; appointment < endDay; appointment = appointment.AddMinutes(calculateTimeSlotsValuesDto.DurationOfSessionInMinutes + calculateTimeSlotsValuesDto.GapBetweenSessionsInMinutes))
                    {
                        calculatedTimeSlots.Add(appointment.ToString("HH:mm"));
                    }
                }

                return calculatedTimeSlots;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Calculate working hours service exception: ");
                throw;
            }
        }

        private string ScheduledTimeSlots(IList<string> selectedTimeSlotsDto)
        {
            try
            {
                if (!selectedTimeSlotsDto.Any())
                    throw new Exception("Calculate slots are null or empty");

                var scheduledTimeSlots = new List<ScheduledTimeSlotsDto>();

                for (var i = 0; i < selectedTimeSlotsDto.Count; i++)
                {
                    var timeSlot = new ScheduledTimeSlotsDto
                    {
                        Id = i,
                        Time = selectedTimeSlotsDto[i],
                        Status = TimeSlotStatus.Available
                    };

                    scheduledTimeSlots.Add(timeSlot);
                }

                return JsonConvert.SerializeObject(scheduledTimeSlots);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Scheduled time slots service exception: ");
                throw;
            }
        }

        private string ScheduledDaysOfTheWeek(DaysOfWeek daysOfWeek)
        {
            try
            {
                var selectedDays = daysOfWeek.GetType()
                                   .GetProperties()
                                   .Where(p => p.PropertyType == typeof(bool) && (bool)
                                   p.GetValue(daysOfWeek, null))
                                   .Select(p => p.Name);

                return JsonConvert.SerializeObject(selectedDays);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Scheduled days of week service exception: ");
                throw;
            }
        }

        private ProviderDto Map(long providerId)
        {           
            var provider = _context.Providers.Include(x => x.User).FirstOrDefault(x => x.Id == providerId);

            if (provider is null)
                throw new Exception("Provider is null");

            return new ProviderDto
            {
                ProviderName = $"{provider.User.FirstName} {provider.User.LastName}",
                Email = provider.User.Email,
                ProviderStrypeId = provider.ProviderStrypeId,
                Title = provider.Title,
                Description = provider.Description,
                ServiceType = provider.ServiceType,
                ProfessionType = provider.ProfessionType,
                PricePerSession = provider.PricePerSession,
                NumberOfParticipants = provider.NumberOfParticipants,
                FiveSessionsDiscount = provider.FiveSessionsDiscount,
                TenSessionsDiscount = provider.TenSessionsDiscount,
                LocationName = provider.Location.Name,
                Longitude = provider.Location.GeoLocation.Coordinate.X,
                Latitude = provider.Location.GeoLocation.Coordinate.Y,
                ScheduleSettings = new ScheduleSettingsDto
                {
                    StartDate = provider.ScheduleSettings.StartDate,
                    EndDate = provider.ScheduleSettings.EndDate,
                    ScheduledDaysOfWeek = JsonConvert.DeserializeObject<List<string>>(provider.ScheduleSettings.ScheduledDaysOfWeek),
                    WorkingHoursStart = provider.ScheduleSettings.WorkingHoursStart,
                    WorkingHoursEnd = provider.ScheduleSettings.WorkingHoursEnd,
                    DurationOfSessionInMinutes = provider.ScheduleSettings.DurationOfSessionInMinutes,
                    GapBetweenSessionsInMinutes = provider.ScheduleSettings.GapBetweenSessionsInMinutes,
                    ScheduledTimeSlots = JsonConvert.DeserializeObject<List<ScheduledTimeSlotsDto>>(provider.ScheduleSettings.ScheduledTimeSlots),
                },
            };
        }

        private Provider Map(AddProviderDto addProviderDto, long userId)
        {
            if (addProviderDto is null)
                throw new Exception("addProviderDto is null");

            return new Provider
            {
                CreatedBy = userId.ToString(),
                UserId = userId,
                Title = addProviderDto.Title,
                Description = addProviderDto.Description,
                ServiceType = addProviderDto.ServiceType,
                ProfessionType = addProviderDto.ProfessionType,
                NumberOfParticipants = addProviderDto.NumberOfParticipants,
                FiveSessionsDiscount = addProviderDto.FiveSessionsDiscount,
                TenSessionsDiscount = addProviderDto.TenSessionsDiscount,
                PricePerSession = addProviderDto.PricePerSession,
                Location = new Location
                {
                    CreatedBy = userId.ToString(),
                    Name = addProviderDto.Location.Name,
                    GeoLocation = _geometryFactory.CreatePoint(new Coordinate(addProviderDto.Location.Longitude, addProviderDto.Location.Latitude))
                },
                ScheduleSettings = new ScheduleSettings
                {
                    CreatedBy = userId.ToString(),
                    StartDate = addProviderDto.ScheduleSettings.StartDate,
                    EndDate = addProviderDto.ScheduleSettings.EndDate,
                    ScheduledDaysOfWeek = ScheduledDaysOfTheWeek(addProviderDto.ScheduleSettings.DaysOfWeek),
                    WorkingHoursStart = addProviderDto.ServiceType == ServiceType.Single ? addProviderDto.ScheduleSettings.WorkingHoursStart : _workingHoursStart,
                    WorkingHoursEnd = addProviderDto.ServiceType == ServiceType.Single ? addProviderDto.ScheduleSettings.WorkingHoursEnd : _workingHoursEnd,
                    DurationOfSessionInMinutes = addProviderDto.ScheduleSettings.DurationOfSessionInMinutes,
                    GapBetweenSessionsInMinutes = addProviderDto.ScheduleSettings.GapBetweenSessionsInMinutes,
                    ScheduledTimeSlots = ScheduledTimeSlots(addProviderDto.ScheduleSettings.SelectedTimeSlots),
                },
            };
        }
    }
}
