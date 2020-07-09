using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;
using Booking.Core.Domain.DTOs;

namespace Booking.Infrastructure.Common.Interfaces
{
    public interface ISmsService
    {
        Task<MessageResource> SendSmsAsync(SmsDto smsDto);
    }
}
