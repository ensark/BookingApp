using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Booking.Core.Domain.DTOs;
using Booking.Core.Services.Interfaces;
using Booking.Core.Domain.Entities;

namespace Booking.API.Controllers
{
    [Authorize]
    [Route("api/provider-skills")]
    [ApiController]
    public class ProviderSkillController : ControllerBase
    {
        private readonly IProviderSkillService _providerSkillService;

        public ProviderSkillController(IProviderSkillService providerSkillService)
        {
            _providerSkillService = providerSkillService;
        }

        [Authorize(Roles = Role.ServiceProvider)]
        [HttpPost("add-provider-skill")]
        public async Task<ActionResult<ProviderSkillDto>> CreateProviderSkill([FromForm] AddProviderSkillDto addProviderSkillDto, CancellationToken cancellationToken)
        {
            try
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userId = Convert.ToInt64(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                var providerSkill = await _providerSkillService.CreateProviderSkillAsync(userId, addProviderSkillDto, cancellationToken);
                return Ok(providerSkill);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.ServiceProvider)]
        [HttpPut("update-provider-skill/{providerSkillId}")]
        public async Task<IActionResult> UpdateProviderSkill(long providerSkillId, [FromForm] AddProviderSkillDto addProviderSkillDto, CancellationToken cancellationToken)
        {
            try
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userId = Convert.ToInt64(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                await _providerSkillService.UpdateProviderSkillAsync(userId, providerSkillId, addProviderSkillDto, cancellationToken);
                return Ok(new { message = "The skill have been successfully updated." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.ServiceProvider)]
        [HttpDelete("delete-provider-skill/{providerSkillId}")]
        public async Task<IActionResult> DeleteProviderSkill(long providerSkillId, CancellationToken cancellationToken)
        {
            try
            {
                await _providerSkillService.DeleteProviderSkillAsync(providerSkillId, cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.ServiceProvider)]
        [HttpGet("get-provider-skills")]
        public async Task<ActionResult<IEnumerable<ProviderSkillDto>>> GetProviderSkills(CancellationToken cancellationToken)
        {
            try
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userId = Convert.ToInt64(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                var providerSkills = await _providerSkillService.GetProviderSkillsAsync(userId, cancellationToken);
                return Ok(providerSkills);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}