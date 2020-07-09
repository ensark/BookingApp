using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.JsonPatch;
using Booking.Core.Domain.DTOs;
using Booking.Core.Services.Interfaces;
using Booking.Infrastructure.Database;
using Booking.Core.Domain.Entities;
using Booking.Common.Exceptions;
using Booking.Core.Domain.Enums;
using Booking.Core.Domain.Queries;
using Booking.Common.Shared;

namespace Booking.Core.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly BookingDBContext _context;
        private readonly ILogger<AppointmentService> _logger;

        public AppointmentService(BookingDBContext context, ILogger<AppointmentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IEnumerable<AppointmentDto> CreateAppointment(long userId, IEnumerable<AddAppointmentDto> addAppointmentDto)
        {
            try
            {
                var appointments = Map(addAppointmentDto, userId);

                _context.Appointments.AddRange(appointments);

                return Map(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create appointment service exception: ");
                throw;
            }
        }

        public async Task UpdateAppointmentTimeAsync(long id, long userId, UpdateAppointmentTimeDto updateAppointmentDto, CancellationToken cancellationToken)
        {
            try
            {
                var appointment = await _context.Appointments.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

                if (appointment is null)
                    throw new NotFoundException($"Appointment with id {id} doesn't exist.");
               
                appointment.ModifiedBy = userId.ToString();
                appointment.ModifiedDate = DateTime.UtcNow;
                appointment.AppointmentTime = updateAppointmentDto.AppointmentTime;

                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update appointment service exception: ");
                throw;
            }
        }

        public async Task DeleteAppointmentAsync(long id, CancellationToken cancellationToken)
        {
            try
            {
                var deleteAppointment = await _context.Appointments.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

                if (deleteAppointment is null)
                    throw new NotFoundException($"Appointment with id {id} doesn't exist.");

                _context.Appointments.Remove(deleteAppointment);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete appointment service exception:");
                throw;
            }
        }

        public async Task<IEnumerable<AppointmentDto>> GetAppointmentsByReservationIdAsync(long reservationId, CancellationToken cancellationToken)
        {
            try
            {
                return await _context.Appointments.Where(x => x.ReservationId == reservationId && x.AppointmentStatus != AppointmentStatus.Completed)
                                                  .Select(x => new AppointmentDto
                                                  {
                                                      AppointmentTime = x.AppointmentTime.ToString("dd MMM, yyyy HH:mm").ToUpper()
                                                  })
                                                  .ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get appointments service exception:");
                throw;
            }
        }

        public async Task<PagedResult<CustomerAppoinmtentsDto>> GetAppointmensByCustomerAsync(long userId, PagedQuery pagedQuery, CancellationToken cancellationToken)
        {
            try
            {
                var appointments = await _context.Appointments.Where(x => x.Reservation.CustomerId == userId)
                                                              .OrderBy(x => x.AppointmentTime)
                                                              .Select(x => new CustomerAppoinmtentsDto
                                                              {
                                                                  Date = $"{x.AppointmentTime.ToString("dddd")},{x.AppointmentTime.ToString("dd MMM").ToUpper()}",
                                                                  Time = x.AppointmentTime.ToString("HH:mm"),
                                                                  ProviderTitle = x.Reservation.Provider.Title,
                                                                  ProviderName = $"{x.Reservation.Provider.User.FirstName} {x.Reservation.Provider.User.LastName}",
                                                                  ProviderAddress = $"{x.Reservation.Provider.Location.Name}",
                                                                  Duration = x.Reservation.Provider.ScheduleSettings.DurationOfSessionInMinutes.ToString()
                                                              })
                                                              .ToListAsync(cancellationToken);
                
                var totalAppointments = appointments.Count;

                var pagedAppointments = appointments.Skip(pagedQuery.Skip)
                                                    .Take(pagedQuery.Take);

                var pagedResult = new PagedResult<CustomerAppoinmtentsDto>
                {
                    CurrentPage = pagedQuery.Page,
                    TotalPages = pagedQuery.CalculatePages(totalAppointments),
                    TotalItems = totalAppointments,
                    Items = pagedAppointments
                };

                return pagedResult;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get appointments by customer service exception:");
                throw;
            }
        }

        private IEnumerable<Appointment> Map(IEnumerable<AddAppointmentDto> addAppointmentsDto, long userId)
        {
            if (!addAppointmentsDto.Any())
                throw new Exception("addAppointmentsDto is null");

            return addAppointmentsDto.Select(x => new Appointment
            {
                CreatedBy = userId.ToString(),
                AppointmentTime = x.AppointmentTime,
                AppointmentStatus = x.AppointmentStatus,
                ReservationId = x.ReservationId,
                PricePerSession = x.PricePerSession,
                AppointmentExternalId = x.AppointmentExternalId
            }).ToList();
        }

        private IEnumerable<AppointmentDto> Map(IEnumerable<Appointment> appointments)
        {
            if (!appointments.Any())
                throw new Exception("Appointments is null");

            return appointments.Select(x => new AppointmentDto
            {
                AppointmentTime = x.AppointmentTime.ToString("dd MM yyyy HH:mm")
            }).ToList();
        }
    }
}
