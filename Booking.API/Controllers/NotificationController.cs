using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Booking.Core.Domain.DTOs;
using Booking.Core.Services.Interfaces;
using Booking.Core.Domain.Entities;
using Booking.Common.Shared;
using Booking.Core.Domain.Queries;

namespace Booking.API.Controllers
{
    [Authorize]
    [Route("api/notifications")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IPushNotificationService _pushNotificationService;

        public NotificationController(IPushNotificationService pushNotificationService)
        {
            _pushNotificationService = pushNotificationService;
        }

        [Authorize(Roles = Role.CustomerAndServiceProvider)]
        [HttpGet("get-push-notifications")]
        public async Task<ActionResult<PagedResult<NotificationDto>>> GetPushNotifications([FromQuery] PagedQuery pagedQuery, CancellationToken cancellationToken)
        {
            try
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userId = Convert.ToInt64(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                var notifications = await _pushNotificationService.GetPushNotificationsAsync(userId, pagedQuery, cancellationToken);

                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
