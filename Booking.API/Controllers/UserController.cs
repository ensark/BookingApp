using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Booking.Core.Services.Interfaces;
using Booking.Core.Domain.DTOs;
using Booking.Core.Domain.Enums;
using Booking.Core.Domain.Entities;

namespace Booking.API.Controllers
{
    [Authorize]
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUploadService _uploadService;
        private readonly IAuthService _authService;

        public UserController(IUserService userService, IUploadService uploadService, IAuthService authService)
        {
            _userService = userService;
            _uploadService = uploadService;
            _authService = authService;
        }

        [Authorize(Roles = Role.CustomerAndServiceProvider)]
        [HttpPost("upload-profile-image")]
        public async Task<IActionResult> UploadProfileImage([FromForm] UploadFileDto uploadFileDto, CancellationToken cancellationToken)
        {
            try
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userId = Convert.ToInt64(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                await _uploadService.UploadFileAsync(userId, uploadFileDto, cancellationToken);
                return Ok(new { message = "The photo have been successfully uploaded." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.CustomerAndServiceProvider)]
        [HttpPut("update-profile-image")]
        public async Task<IActionResult> UpdateProfileImage(long attachmentId, [FromForm] UploadFileDto updateFileDto, CancellationToken cancellationToken)
        {
            try
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userId = Convert.ToInt64(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                await _uploadService.UpdateFileAsync(userId, attachmentId, updateFileDto, cancellationToken);
                return Ok(new { message = "The photo have been successfully replaced." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.CustomerAndServiceProvider)]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromForm] string newPassword, CancellationToken cancellationToken)
        {
            try
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userId = Convert.ToInt64(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                await _authService.ChangePasswordAsync(userId, newPassword, cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.CustomerAndServiceProvider)]
        [HttpPut("update-user-privacy-settings")]
        public async Task<IActionResult> UpdateUserPrivacySettings([FromForm] UpdateUserPrivacySettingsDto updateUserPrivacySettings, CancellationToken cancellationToken)
        {
            try
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userId = Convert.ToInt64(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                await _userService.UpdateUserPrivacySettingsAsync(userId, updateUserPrivacySettings, cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.CustomerAndServiceProvider)]
        [HttpPatch("update-notification-settings/{notificationSettingsId}")]
        public async Task<IActionResult> UpdateNotificationSettings(long notificationSettingsId, [FromBody] JsonPatchDocument<NotificationSettingsDto> patchDoc, CancellationToken cancellationToken)
        {
            try
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userId = Convert.ToInt64(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                await _userService.UpdateNotificationSettingsAsync(userId, notificationSettingsId, patchDoc, cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.CustomerAndServiceProvider)]
        [HttpPatch("update-reminders/{reminderId}")]
        public async Task<IActionResult> UpdateReminders(long reminderId, [FromBody] JsonPatchDocument<ReminderDto> patchDoc, CancellationToken cancellationToken)
        {
            try
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userId = Convert.ToInt64(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                await _userService.UpdateRemindersAsync(userId, reminderId, patchDoc, cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.Customer)]
        [HttpPut("become-service-provider")]
        public async Task<ActionResult<LoggedUserDto>> BecomeServiceProvider(CancellationToken cancellationToken)
        {
            try
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userId = Convert.ToInt64(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                var loginData = await _authService.BecomeServiceProviderAsync(userId, cancellationToken);
                return Ok(loginData);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id, CancellationToken cancellationToken)
        {
            try
            {
                await _userService.DeleteUserAsync(id, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.CustomerAndServiceProvider)]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(long id, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id, cancellationToken);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}