using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using Booking.Core.Domain.DTOs;
using Booking.Core.Domain.Entities;
using Booking.Core.Services.Interfaces;
using Booking.Core.Domain.Enums;

namespace Booking.API.Controllers
{
    [Authorize]
    [Route("api/user-profile")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUploadService _uploadService;

        public UserProfileController(IUserService userService, IUploadService uploadService)
        {
            _userService = userService;
            _uploadService = uploadService;
        }

        [Authorize(Roles = Role.CustomerAndServiceProvider)]
        [HttpPost("upload-attachments")]
        public async Task<IActionResult> UploadAttachments([FromForm] IEnumerable<UploadFileDto> files, CancellationToken cancellationToken)
        {
            try
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userId = Convert.ToInt64(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                await _uploadService.UploadFilesAsync(userId, files, cancellationToken);
                return Ok(new { message = "The files have been successfully uploaded." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.CustomerAndServiceProvider)]
        [HttpPost("upload-attachment")]
        public async Task<IActionResult> UploadAttachment([FromForm] UploadFileDto uploadFileDto, CancellationToken cancellationToken)
        {
            try
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userId = Convert.ToInt64(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                await _uploadService.UploadFileAsync(userId, uploadFileDto, cancellationToken);
                return Ok(new { message = "The file have been successfully uploaded." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.CustomerAndServiceProvider)]
        [HttpPut("update-attachment")]
        public async Task<IActionResult> UpdateAttachment(long attachmentId, [FromForm] UploadFileDto updateFileDto, CancellationToken cancellationToken)
        {
            try
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userId = Convert.ToInt64(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                await _uploadService.UpdateFileAsync(userId, attachmentId, updateFileDto, cancellationToken);
                return Ok(new { message = "The file have been successfully replaced." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.ServiceProvider)]
        [HttpPatch("update-biography")]
        public async Task<IActionResult> UpdateBiography([FromBody] JsonPatchDocument<PatchUserDto> patchDoc, CancellationToken cancellationToken)
        {
            try
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userId = Convert.ToInt64(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                await _userService.UpdateBiographyAsync(userId, patchDoc, cancellationToken);
                return Ok(new { message = "The biography have been successfully updated." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.CustomerAndServiceProvider)]
        [HttpDelete("delete-attachment")]
        public async Task<IActionResult> DeleteAttachment(long id, CancellationToken cancellationToken)
        {
            try
            {
                await _uploadService.DeleteFileAsync(id, cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.ServiceProvider)]
        [HttpGet("provider")]
        public async Task<ActionResult<ProviderProfileDto>> GetProviderProfile(CancellationToken cancellationToken)
        {
            try
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userId = Convert.ToInt64(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                var providerProfile = await _userService.GetProviderProfileAsync(userId, cancellationToken);
                return Ok(providerProfile);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.Customer)]
        [HttpGet("customer")]
        public async Task<ActionResult<CustomerProfileDto>> GetCustomerProfile(CancellationToken cancellationToken)
        {
            try
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userId = Convert.ToInt64(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                var customerProfile = await _userService.GetCustomerProfileAsync(userId, cancellationToken);
                return Ok(customerProfile);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}