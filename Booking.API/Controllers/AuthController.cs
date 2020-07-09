using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Booking.Core.Domain.DTOs;
using Booking.Core.Services.Interfaces;

namespace Booking.API.Controllers
{
    [Authorize]
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> RegisterUser([FromForm]RegisterUserDto registerUserDto, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.RegisterUserAsync(registerUserDto, cancellationToken);

                if (user is null)
                    return BadRequest(new { message = "User is null" });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<ActionResult<LoggedUserDto>> AuthenticateUser([FromForm]AuthenticateUserDto authenticateUserDto, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.AuthenticateUserAsync(authenticateUserDto, cancellationToken);

                if (user is null)
                    return BadRequest(new { message = "Email or password is incorrect" });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword([FromForm] string phoneNumber, CancellationToken cancellationToken)
        {
            try
            {
                await _authService.ForgotPasswordAsync(phoneNumber, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken([FromForm] string token, CancellationToken cancellationToken)
        {
            try
            {
                var loginResponse = await _authService.RefreshAccessToken(token, cancellationToken);
                return Ok(loginResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}