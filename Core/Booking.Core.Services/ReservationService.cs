using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Booking.Core.Services.Interfaces;
using Booking.Infrastructure.Database;
using Booking.Core.Domain.DTOs;
using Booking.Core.Domain.Entities;
using Booking.Core.Domain.Enums;
using Booking.Common.RecurrenceProcessor;
using Booking.Infrastructure.Common.Interfaces;

namespace Booking.Core.Services
{
    public class ReservationService : IReservationService
    {
        private const int RECUR_EVERY_X_WEEEKS = 1;
        private const int FIVE_SESSIONS = 5;
        private const int TEN_SESSIONS = 10;

        private readonly BookingDBContext _context;
        private readonly ILogger<ReservationService> _logger;
        private readonly IProviderService _providerService;
        private readonly IAppointmentService _appointmentService;
        private readonly IFirebaseMessageClient _firebaseClient;

        public ReservationService(BookingDBContext context, ILogger<ReservationService> logger, IProviderService providerService, IAppointmentService appointmentService, IFirebaseMessageClient firebaseClient)
        {
            _context = context;
            _logger = logger;
            _providerService = providerService;
            _appointmentService = appointmentService;
            _firebaseClient = firebaseClient;
        }

        public async Task<ReservationDto> CreateReservationAsync(long userId, AddReservationDto addReservationDto, CancellationToken cancellationToken)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var reservation = Map(userId, addReservationDto);

                    var providerInfo = await _providerService.GetProviderByIdAsync(reservation.ProviderId, cancellationToken);

                    _logger.LogInformation($"Create reservation: {reservation}");

                    _context.Reservations.Add(reservation);
                    await _context.SaveChangesAsync(cancellationToken);

                    var appointments = ScheduledAppointmentsTime(addReservationDto).Select(x => new AddAppointmentDto
                    {
                        AppointmentTime = x.AppointmentTime,
                        ReservationId = reservation.Id,
                        AppointmentStatus = AppointmentStatus.Created,
                        PricePerSession = providerInfo.PricePerSession,
                        AppointmentExternalId = Guid.NewGuid()
                    })
                    .ToList();

                    _logger.LogInformation($"Create appointments.");

                    if (appointments.Any())
                    {
                        _appointmentService.CreateAppointment(userId, appointments);
                        await _context.SaveChangesAsync(cancellationToken);

                        var numberOfCustomerSessions = appointments.Count;

                        if (numberOfCustomerSessions >= FIVE_SESSIONS && numberOfCustomerSessions < TEN_SESSIONS)
                            reservation.FiveSessionsDiscount = providerInfo.FiveSessionsDiscount;

                        else if (numberOfCustomerSessions >= TEN_SESSIONS)
                            reservation.TenSessionsDiscount = providerInfo.TenSessionsDiscount;

                        reservation.TotalPrice = appointments.Sum(x => x.PricePerSession);

                        await _context.SaveChangesAsync(cancellationToken);

                        transaction.Commit();
                    }

                    else
                        throw new Exception($"Appoinments list must not be empty.");

                    return Map(reservation);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError(ex, "Create reservation service exception: ");
                    throw;
                }
            }
        }

        public async Task ReservationRequestAsync(long reservationId, long userId, CancellationToken cancellationToken)
        {
            try
            {
                var reservation = await _context.Reservations.FirstOrDefaultAsync(x => x.Id == reservationId, cancellationToken);

                if (reservation is null)
                    throw new Exception($"Reservation with id {reservationId} not found.");

                if (reservation.Provider.User.UserType == UserType.ServiceProvider && reservation.Provider.User.NotificationSettings.NewBookings)
                {
                    var pushNotificationRequest = new PushNotificationRequestDto
                    {
                        DeviceId = reservation.Provider.User.FcmTokenDeviceId,
                        Title = "Booking request",
                        Body = $"{reservation.User.FirstName} {reservation.User.LastName} sent you a booking request for {reservation.Provider.Title}",
                        Data = Map(reservation),
                        SenderId = userId,
                        ReceiverId = reservation.ProviderId,
                        ReservationId = reservation.Id,
                        NotificationType = NotificationType.NewBookings
                    };

                    await _firebaseClient.SendPushNotificationAsync(pushNotificationRequest, cancellationToken);                    
                }

                reservation.ReservationStatus = ReservationStatus.SentRequest;
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Send booking request service exception: ");
                throw;
            }
        }

        public async Task ReservationRequestAnswerAsync(long reservationId, long userId, bool isAccepted, CancellationToken cancellationToken)
        {
            try
            {
                var reservation = await _context.Reservations.FirstOrDefaultAsync(x => x.Id == reservationId, cancellationToken);

                if (reservation is null)
                    throw new Exception($"Reservation with id {reservationId} not found.");

                if (isAccepted)
                    reservation.ReservationStatus = ReservationStatus.Accepted;

                else
                    reservation.ReservationStatus = ReservationStatus.Rejected;

                await _context.SaveChangesAsync(cancellationToken);

                if (reservation.User.UserType == UserType.Customer && reservation.User.NotificationSettings.BookingConfirmations)
                {
                    var pushNotificationRequest = new PushNotificationRequestDto
                    {
                        DeviceId = reservation.User.FcmTokenDeviceId,
                        Title = isAccepted ? "Booking accepted" : "Booking rejected",
                        Body = isAccepted ? $"{reservation.Provider.User.FirstName} {reservation.Provider.User.LastName} accepted your booking request for {reservation.Provider.Title}" :
                                            $"{reservation.Provider.User.FirstName} {reservation.Provider.User.LastName} rejected your booking request for {reservation.Provider.Title}",
                        Data = Map(reservation),
                        SenderId = userId,
                        ReceiverId = reservation.CustomerId,
                        ReservationId = reservation.Id,
                        NotificationType = NotificationType.BookingConfirmations
                    };

                    await _firebaseClient.SendPushNotificationAsync(pushNotificationRequest, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Reservation request answer service exception: ");
                throw;
            }
        }

        public async Task<IEnumerable<ReservationRequestDto>> GetReservationRequestsAsync(long providerId, CancellationToken cancellationToken)
        {
            try
            {
                var reservations = await _context.Reservations.Where(x => x.ProviderId == providerId &&
                                                                     x.ReservationStatus == ReservationStatus.SentRequest)
                                                                    .ToListAsync(cancellationToken);

                return reservations.Select(x => new ReservationRequestDto
                {
                    Title = x.Provider.Title,
                    Duration = x.Provider.ScheduleSettings.DurationOfSessionInMinutes.ToString(),
                    ScheduledAppointments = x.Appointments.Select(z => z.AppointmentTime.ToString("dd MMMM yyyy HH:mm")).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get booking request service exception: ");
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetProviderAvailabiltyAsync(long providerId, DateTime requestDate, CancellationToken cancellationToken)
        {
            try
            {
                var provider = await _context.Providers.Where(a => a.Id == providerId && a.ScheduleSettings.ScheduledDaysOfWeek.Contains(requestDate.DayOfWeek.ToString()))
                                                       .Select(x => x.ScheduleSettings.ScheduledTimeSlots)
                                                       .FirstOrDefaultAsync(cancellationToken);
                if (!string.IsNullOrEmpty(provider))
                    return JsonConvert.DeserializeObject<List<ScheduledTimeSlotsDto>>(provider).Where(x => x.Status == TimeSlotStatus.Available).Select(x => x.Time);

                else
                    return Array.Empty<string>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Check provider availabilty service exception: ");
                throw;
            }
        }

        private IEnumerable<ScheduledAppointmentDto> ScheduledAppointmentsTime(AddReservationDto addReservationDto)
        {
            switch (addReservationDto.ReccurenceType)
            {
                case Common.RecurrenceProcessor.Enums.ReccurenceType.NonReccuring:
                    return CalculateNonRecuringReservation(addReservationDto);
                case Common.RecurrenceProcessor.Enums.ReccurenceType.Weekly:
                    return CalculateWeeklyReservation(addReservationDto);
                case Common.RecurrenceProcessor.Enums.ReccurenceType.Custom:
                    return CalculateCustomReservation(addReservationDto);
                default:
                    return Array.Empty<ScheduledAppointmentDto>();
            }
        }

        private IEnumerable<ScheduledAppointmentDto> CalculateNonRecuringReservation(AddReservationDto addReservationDto)
        {
            try
            {
                var getHoursMinutes = addReservationDto.RequestTime.Split(':');
                var hours = Convert.ToInt32(getHoursMinutes[0]);
                var minutes = Convert.ToInt32(getHoursMinutes[1]);

                var selectedAppointment = addReservationDto.RequestDate.AddHours(hours)
                                                                       .AddMinutes(minutes);

                var scheduledAppointments = new List<ScheduledAppointmentDto>
                {
                    new ScheduledAppointmentDto { AppointmentTime = selectedAppointment }
                };

                return scheduledAppointments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Calculate non recuring time service exception: ");
                throw;
            }
        }

        private IEnumerable<ScheduledAppointmentDto> CalculateWeeklyReservation(AddReservationDto addReservationDto)
        {
            try
            {
                var scheduledAppointments = new List<ScheduledAppointmentDto>();

                if (addReservationDto.NumberOfWeeks >= 1)
                {
                    SelectedDayOfWeekValues SelectedDays = new SelectedDayOfWeekValues();

                    switch (addReservationDto.RequestDate.DayOfWeek)
                    {
                        case DayOfWeek.Monday:
                            SelectedDays.Monday = true;
                            break;
                        case DayOfWeek.Tuesday:
                            SelectedDays.Tuesday = true;
                            break;
                        case DayOfWeek.Wednesday:
                            SelectedDays.Wednesday = true;
                            break;
                        case DayOfWeek.Thursday:
                            SelectedDays.Thursday = true;
                            break;
                        case DayOfWeek.Friday:
                            SelectedDays.Friday = true;
                            break;
                        case DayOfWeek.Saturday:
                            SelectedDays.Saturday = true;
                            break;
                        case DayOfWeek.Sunday:
                            SelectedDays.Sunday = true;
                            break;
                        default:
                            break;
                    }

                    var weeklyRecurrenceSettings = new WeeklyRecurrenceSettings(addReservationDto.RequestDate, addReservationDto.NumberOfWeeks);

                    var recurrenceValues = weeklyRecurrenceSettings.GetValues(RECUR_EVERY_X_WEEEKS, SelectedDays);

                    var getHoursMinutes = addReservationDto.RequestTime.Split(':');
                    var hours = Convert.ToInt32(getHoursMinutes[0]);
                    var minutes = Convert.ToInt32(getHoursMinutes[1]);

                    scheduledAppointments = recurrenceValues.Values.Select(x => new ScheduledAppointmentDto { AppointmentTime = x.AddHours(hours).AddMinutes(minutes) }).ToList();
                }
                return scheduledAppointments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Calculate non recuring time service exception: ");
                throw;
            }
        }

        private IEnumerable<ScheduledAppointmentDto> CalculateCustomReservation(AddReservationDto addReservationDto)
        {
            try
            {
                var scheduledAppointments = new List<ScheduledAppointmentDto>();

                if (addReservationDto.RequestDates.Any())
                {
                    var getHoursMinutes = addReservationDto.RequestTime.Split(':');
                    var hours = Convert.ToInt32(getHoursMinutes[0]);
                    var minutes = Convert.ToInt32(getHoursMinutes[1]);

                    scheduledAppointments = addReservationDto.RequestDates.Select(x => new ScheduledAppointmentDto { AppointmentTime = x.AddHours(hours).AddMinutes(minutes) }).ToList();
                }
                return scheduledAppointments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Calculate non recuring time service exception: ");
                throw;
            }
        }

        private ReservationDto Map(Reservation reservation)
        {
            if (reservation is null)
                throw new Exception("Reservation is null");

            var providerInfo = _context.Providers.Include(x => x.ScheduleSettings)
                                                 .Include(x => x.User)
                                                 .ThenInclude(x => x.Address)
                                                 .FirstOrDefault(x => x.Id == reservation.ProviderId);

            return new ReservationDto
            {
                Title = providerInfo.Title,
                Duration = providerInfo.ScheduleSettings.DurationOfSessionInMinutes.ToString(),
                Address = providerInfo.User.Address.Street,
                City = providerInfo.User.Address.City,
                TotalPrice = reservation.TotalPrice.ToString("0.00"),
                ScheduledAppointments = reservation.Appointments.Select(x => x.AppointmentTime.ToString("dd MMMM yyyy HH:mm")).ToList()
            };
        }

        private Reservation Map(long userId, AddReservationDto addReservationDto)
        {
            return new Reservation
            {
                CreatedBy = userId.ToString(),
                CustomerId = userId,
                ProviderId = addReservationDto.ProviderId,
                ReservationStatus = ReservationStatus.Created,
                PayPerSession = false,
                PayTotal = false,
                ReservationPaymentId = Guid.NewGuid(),
            };
        }
    }
}
