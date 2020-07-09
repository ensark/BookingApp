using System.Threading;
using System.Threading.Tasks;
using Booking.Common.Shared;
using Booking.Core.Domain.DTOs;
using Booking.Core.Domain.Queries;

namespace Booking.Core.Services.Interfaces
{
    public interface IChatService
    {       
        Task SaveChatMessagesAsync(AddChatDto addChatMessageDto, CancellationToken cancellationToken);

        Task<PagedResult<ChatsDto>> GetChatsAsync(long userId, PagedQuery pagedQuery, CancellationToken cancellationToken);

        Task<PagedResult<ChatMessagesDto>> GetChatMessagesAsync(long chatId, PagedQuery pagedQuery, CancellationToken cancellationToken);
    }
}
