using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Booking.Core.Services.Interfaces;
using Booking.Core.Domain.DTOs;
using Booking.Core.Domain.Entities;

namespace Booking.API.Controllers
{
    [Authorize]
    [Route("api/reservations")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [Authorize(Roles = Role.Customer)]
        [HttpPost("create-reservation")]
        public async Task<ActionResult<ReservationDto>> CreateReservation([FromForm] AddReservationDto addReservationDto, CancellationToken cancellationToken)
        {
            try
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userId = Convert.ToInt64(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                var reservation = await _reservationService.CreateReservationAsync(userId, addReservationDto, cancellationToken);
                return Ok(reservation);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.Customer)]
        [HttpPost("send-reservation-request/{reservationId}")]
        public async Task<IActionResult> ReservationRequest([FromRoute] long reservationId, CancellationToken cancellationToken)
        {
            try
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userId = Convert.ToInt64(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                await _reservationService.ReservationRequestAsync(reservationId, userId, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.ServiceProvider)]
        [HttpPost("reservation-request-answer/{reservationId}")]
        public async Task<ActionResult<bool>> ReservationRequestAnswer([FromRoute]  long reservationId, [FromForm] bool isAccepted, CancellationToken cancellationToken)
        {
            try
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userId = Convert.ToInt64(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                await _reservationService.ReservationRequestAnswerAsync(reservationId, userId, isAccepted, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.ServiceProvider)]
        [HttpGet("reservation-requests")]
        public async Task<ActionResult<IEnumerable<ReservationRequestDto>>> GetReservationRequests(CancellationToken cancellationToken)
        {
            try
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userId = Convert.ToInt64(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                var reservationRequests = await _reservationService.GetReservationRequestsAsync(userId, cancellationToken);
                return Ok(reservationRequests);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.Customer)]
        [HttpGet("check-provider-availabilty")]
        public async Task<ActionResult<IEnumerable<string>>> GetProviderAvailabilty(long providerId, DateTime requestDate, CancellationToken cancellationToken)
        {
            try
            {
                var schedules = await _reservationService.GetProviderAvailabiltyAsync(providerId, requestDate, cancellationToken);
                return Ok(schedules);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
