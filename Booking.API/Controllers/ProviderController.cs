using System;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Booking.Core.Domain.DTOs;
using Booking.Core.Services.Interfaces;
using Booking.Core.Domain.Enums;
using Booking.Core.Domain.Entities;
using Booking.Core.Domain.Queries;
using Booking.Common.Shared;

namespace Booking.API.Controllers
{
    [Authorize]
    [Route("api/providers")]
    [ApiController]
    public class ProviderController : ControllerBase
    {
        private readonly IProviderService _providerService;

        public ProviderController(IProviderService providerService)
        {
            _providerService = providerService;
        }

        [Authorize(Roles = Role.ServiceProvider)]
        [HttpPost("create-provider")]
        public async Task<ActionResult<ProviderDto>> CreateProvider([FromForm]AddProviderDto providerDto, CancellationToken cancellationToken)
        {
            try
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userId = Convert.ToInt64(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                var provider = await _providerService.CreateProviderAsync(userId, providerDto, cancellationToken);
                return Ok(provider);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.ServiceProvider)]
        [HttpPut("update-provider/{id}")]
        public async Task<IActionResult> UpdateProvider(long id, [FromForm]AddProviderDto providerDto, CancellationToken cancellationToken)
        {
            try
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userId = Convert.ToInt64(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                await _providerService.UpdateProviderAsync(id, userId, providerDto, cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.ServiceProvider)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProvider(long id, CancellationToken cancellationToken)
        {
            try
            {
                await _providerService.DeleteProviderAsync(id, cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.ServiceProvider)]
        [HttpGet("get-providers-by-services")]
        public async Task<ActionResult<IEnumerable<ProviderDto>>> GetProvidersByServices(long serviceType, CancellationToken cancellationToken)
        {
            try
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userId = Convert.ToInt64(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                var providers = await _providerService.GetProvidersByServiceTypeAsync(userId, (ServiceType)serviceType, cancellationToken);
                return Ok(providers);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.Customer)]
        [HttpGet("get-all-providers")]
        public async Task<ActionResult<PagedResult<ProviderSearchListDto>>> GetAllProviders([FromQuery] SearchQuery searchQuery, CancellationToken cancellationToken)
        {
            try
            {
                var providers = await _providerService.GetAllProvidersAsync(searchQuery, cancellationToken);
                return Ok(providers);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.ServiceProvider)]
        [HttpGet("get-provider-by-id")]
        public async Task<ActionResult<ProviderDto>> GetProviderById(long id, CancellationToken cancellationToken)
        {
            try
            {
                var provider = await _providerService.GetProviderByIdAsync(id, cancellationToken);
                return Ok(provider);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.ServiceProvider)]
        [HttpGet("calculate-time-slots")]
        public ActionResult<IList<string>> CalculateTimeSlots([FromQuery] CalculateTimeSlotsValuesDto calculateTimeSlotsDtos)
        {
            try
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userId = Convert.ToInt64(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                var timeSlots = _providerService.CalculateTimeSlots(userId, calculateTimeSlotsDtos);
                return Ok(timeSlots);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}