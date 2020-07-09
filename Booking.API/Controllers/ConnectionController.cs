using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Booking.Common.Shared;
using Booking.Core.Domain.DTOs;
using Booking.Core.Domain.Entities;
using Booking.Core.Domain.Queries;
using Booking.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers
{
    [Authorize]
    [Route("api/connections")]
    [ApiController]
    public class ConnectionController : ControllerBase
    {
        private readonly IConnectionService _connectionService;

        public ConnectionController(IConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        [Authorize(Roles = Role.CustomerAndServiceProvider)]
        [HttpPost("create-connection-request")]
        public async Task<ActionResult<UserDto>> CreateConnectionRequest([FromForm] long connectionUserId, CancellationToken cancellationToken)
        {
            try
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userId = Convert.ToInt64(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                var connection = await _connectionService.CreateConnectionRequestAsync(userId, connectionUserId, cancellationToken);
                return Ok(connection);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.CustomerAndServiceProvider)]
        [HttpPost("connection-request-answer/{connectionId}")]
        public async Task<IActionResult> ConnectionRequestAnswer([FromRoute] long connectionId, [FromForm] bool isAccepted, CancellationToken cancellationToken)
        {
            try
            {               
                await _connectionService.ConnectionRequestAnswerAsync(connectionId, isAccepted, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.CustomerAndServiceProvider)]
        [HttpGet("get-connection-requests")]
        public async Task<ActionResult<IEnumerable<AddConnectionDto>>> GetConnectionRequests(CancellationToken cancellationToken)
        {
            try
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userId = Convert.ToInt64(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                var connectionRequests = await _connectionService.GetConnectionRequestsAsync(userId, cancellationToken);
                return Ok(connectionRequests);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.CustomerAndServiceProvider)]
        [HttpDelete("delete-connection/{connectionId}")]
        public async Task<IActionResult> DeleteProviderSkill([FromRoute] long connectionId, CancellationToken cancellationToken)
        {
            try
            {
                await _connectionService.DeleteConnectionAsync(connectionId, cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.CustomerAndServiceProvider)]        
        [HttpGet("get-connections")]
        public async Task<ActionResult<PagedResult<UserDto>>> GetConnections([FromQuery] SearchQuery searchQuery, CancellationToken cancellationToken)
        {
            try
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userId = Convert.ToInt64(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                var connections = await _connectionService.GetConnectonsAsync(userId, searchQuery, cancellationToken);
                return Ok(connections);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}