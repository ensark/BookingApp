using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Booking.Core.Domain.Entities;
using Booking.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers
{
    [Authorize]
    [Route("api/invites")]
    [ApiController]
    public class InviteController : ControllerBase
    {
        private readonly IInviteService _inviteService;

        public InviteController(IInviteService inviteService)
        {
            _inviteService = inviteService;
        }

        [Authorize(Roles = Role.CustomerAndServiceProvider)]
        [HttpPost("send-invite")]
        public async Task<IActionResult> SendSmsInvite([FromForm] IEnumerable<string> phoneNumbers, CancellationToken cancellationToken)
        {
            try
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userId = Convert.ToInt64(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                await _inviteService.SendInvitesAsync(userId, phoneNumbers, cancellationToken);
                return Ok(new { message = "The message have been successfully sent." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}