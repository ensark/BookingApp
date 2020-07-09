using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Booking.Core.Domain.DTOs;
using Booking.Core.Domain.Entities;
using Booking.Core.Services.Interfaces;
using Booking.Infrastructure.Common.Interfaces;
using Booking.Common.Shared;
using Booking.Core.Domain.Queries;

namespace Booking.API.Controllers
{
    [Authorize]
    [Route("api/appointments")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
       
        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;           
        }

        [Authorize(Roles = Role.Customer)]
        [HttpPut("update-appointment-time/{id}")]
        public async Task<IActionResult> UpdateAppointmentTime(long id, [FromForm] UpdateAppointmentTimeDto updateAppointmentDto, CancellationToken cancellationToken)
        {
            try
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userId = Convert.ToInt64(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                await _appointmentService.UpdateAppointmentTimeAsync(id, userId, updateAppointmentDto, cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.Customer)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(long id, CancellationToken cancellationToken)
        {
            try
            {
                await _appointmentService.DeleteAppointmentAsync(id, cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.Customer)]
        [HttpGet("get-appointments-by-reservation-id")]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAappointmentsByReservationId(long reservationId, CancellationToken cancellationToken)
        {
            try
            {
                var appointments = await _appointmentService.GetAppointmentsByReservationIdAsync(reservationId, cancellationToken);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.Customer)]
        [HttpGet("get-appointments-by-customer")]
        public async Task<ActionResult<PagedResult<CustomerAppoinmtentsDto>>> GetAappointmentsByCustomer([FromQuery] PagedQuery pagedQuery, CancellationToken cancellationToken)
        {
            try
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userId = Convert.ToInt64(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                var appointments = await _appointmentService.GetAppointmensByCustomerAsync(userId, pagedQuery, cancellationToken);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
