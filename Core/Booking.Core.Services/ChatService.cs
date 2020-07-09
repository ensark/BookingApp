using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Booking.Core.Domain.DTOs;
using Booking.Core.Domain.Entities;
using Booking.Core.Services.Interfaces;
using Booking.Infrastructure.Database;
using Booking.Core.Domain.Queries;
using Booking.Common.Shared;

namespace Booking.Core.Services
{
    public class ChatService : IChatService
    {
        private readonly BookingDBContext _context;
        private readonly ILogger<ChatService> _logger;
        private readonly IUserService _userService;

        public ChatService(BookingDBContext context, ILogger<ChatService> logger, IUserService userService)
        {
            _context = context;
            _logger = logger;
            _userService = userService;
        }

        public async Task SaveChatMessagesAsync(AddChatDto addChatMessageDto, CancellationToken cancellationToken)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var currrentChat = new Chat();
                    long chatId = 0;

                    if (!_context.Chats.ToList().Exists(x => (x.SenderId == addChatMessageDto.SenderId && x.ReceiverId == addChatMessageDto.ReceiverId) ||
                                                             (x.SenderId == addChatMessageDto.ReceiverId && x.ReceiverId == addChatMessageDto.SenderId)))
                    {
                        var chat = new Chat
                        {
                            CreatedBy = addChatMessageDto.SenderId.ToString(),
                            SenderId = addChatMessageDto.SenderId,
                            ReceiverId = addChatMessageDto.ReceiverId,
                            ModifiedDate = DateTime.Now
                        };

                        _context.Chats.Add(chat);
                        await _context.SaveChangesAsync(cancellationToken);

                        chatId = chat.Id;
                    }
                    else
                    {
                        currrentChat = await _context.Chats.FirstOrDefaultAsync(x => x.SenderId == addChatMessageDto.SenderId && x.ReceiverId == addChatMessageDto.ReceiverId ||
                                                                                x.SenderId == addChatMessageDto.ReceiverId && x.ReceiverId == addChatMessageDto.SenderId, cancellationToken);
                        chatId = currrentChat.Id;
                        currrentChat.ModifiedDate = DateTime.Now;
                    }

                    var senderUser = await _userService.GetUserByIdAsync(addChatMessageDto.SenderId, cancellationToken);
                    var receiverUser = await _userService.GetUserByIdAsync(addChatMessageDto.ReceiverId, cancellationToken);

                    var chatMessage = new ChatMessage
                    {
                        CreatedBy = addChatMessageDto.SenderId.ToString(),
                        ChatId = chatId,
                        SenderName = $"{senderUser.FirstName} {senderUser.LastName}",
                        ReceiverName = $"{receiverUser.FirstName} {receiverUser.LastName}",
                        Content = addChatMessageDto.ChatMessage.MessageContent,
                        MessageSentAt = DateTime.Now,
                        IsRead = true
                    };

                    _context.ChatMessages.Add(chatMessage);
                    await _context.SaveChangesAsync(cancellationToken);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError(ex, "Save chat message service exception: ");
                    throw;
                }
            }
        }

        public async Task<PagedResult<ChatsDto>> GetChatsAsync(long userId, PagedQuery pagedQuery, CancellationToken cancellationToken)
        {
            try
            {
                var userData = await _userService.GetUserByIdAsync(userId, cancellationToken);

                var chats =  await _context.Chats.Where(x => x.SenderId == userId || x.ReceiverId == userId)
                                           .OrderByDescending(x => x.ModifiedDate)
                                           .Select(x => new ChatsDto
                                           {
                                               ChatWith = x.Receiver.FirstName == userData.FirstName && x.Receiver.LastName == userData.LastName ?
                                               $"{x.Sender.FirstName} {x.Sender.LastName}" :
                                               $"{x.Receiver.FirstName} {x.Receiver.LastName}"
                                           })
                                           .ToListAsync(cancellationToken);

                var totalChats = chats.Count;

                var pagedItems = chats.Skip(pagedQuery.Skip)
                                      .Take(pagedQuery.Take);

                var pagedResult = new PagedResult<ChatsDto>
                {
                    CurrentPage = pagedQuery.Page,
                    TotalPages = pagedQuery.CalculatePages(totalChats),
                    TotalItems = totalChats,
                    Items = pagedItems
                };

                return pagedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get chats service exception: ");
                throw;
            }
        }

        public async Task<PagedResult<ChatMessagesDto>> GetChatMessagesAsync(long chatId, PagedQuery pagedQuery, CancellationToken cancellationToken)
        {
            try
            {
                var chatMessages = await _context.ChatMessages.Where(x => x.ChatId == chatId)
                                       .OrderByDescending(x => x.MessageSentAt)
                                       .Select(a => new ChatMessagesDto
                                       {
                                           SenderName = a.SenderName,
                                           ReceiverName = a.ReceiverName,
                                           MessageContent = a.Content,
                                           MessageSentAt = a.MessageSentAt.ToString("dd MMM yyyy HH:mm")
                                       })
                                       .ToListAsync(cancellationToken);

                var totalChatMessages = chatMessages.Count;

                var pagedItems = chatMessages.Skip(pagedQuery.Skip)
                                             .Take(pagedQuery.Take);

                var pagedResult = new PagedResult<ChatMessagesDto>
                {
                    CurrentPage = pagedQuery.Page,
                    TotalPages = pagedQuery.CalculatePages(totalChatMessages),
                    TotalItems = totalChatMessages,
                    Items = pagedItems
                };

                return pagedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get chats messages service exception: ");
                throw;
            }
        }
    }
}
