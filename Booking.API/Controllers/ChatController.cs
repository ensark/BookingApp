using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Booking.Core.Domain.DTOs;
using Booking.Core.Domain.Entities;
using Booking.Core.Services.Interfaces;
using Booking.Core.Domain.Queries;
using Booking.Common.Shared;

namespace Booking.API.Controllers
{
    [Authorize]
    [Route("api/chats")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        
        public ChatController(IChatService chatService)
        {
            _chatService = chatService;           
        }
        
        [Authorize(Roles = Role.CustomerAndServiceProvider)]
        [HttpGet("get-chats")]
        public async Task<ActionResult<PagedResult<ChatsDto>>> GetChats([FromQuery] PagedQuery pagedQuery, CancellationToken cancellationToken)
        {
            try
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userId = Convert.ToInt64(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                var chats = await _chatService.GetChatsAsync(userId, pagedQuery, cancellationToken);
               
                return Ok(chats);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.CustomerAndServiceProvider)]
        [HttpGet("get-chat-messages/{chatId}")]
        public async Task<ActionResult<PagedResult<ChatMessagesDto>>> GetChatMessages([FromRoute] long chatId, [FromQuery] PagedQuery pagedQuery, CancellationToken cancellationToken)
        {
            try
            {
                var chatMessages = await _chatService.GetChatMessagesAsync(chatId, pagedQuery, cancellationToken);

                return Ok(chatMessages);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}