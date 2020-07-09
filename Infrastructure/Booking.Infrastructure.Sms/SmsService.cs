using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Booking.Infrastructure.Common.Interfaces;
using Booking.Core.Domain.DTOs;

namespace Booking.Infrastructure.Sms
{
    public class SmsService : ISmsService
    {
        private readonly ILogger<SmsService> _logger;
        private readonly ITwilioRestClient _twilioClient;
        private readonly string _twilioPhoneNumber;

        public SmsService(ILogger<SmsService> logger, IConfiguration configuration, ITwilioRestClient twilioClient)
        {
            _logger = logger;
            _twilioClient = twilioClient;
            _twilioPhoneNumber = configuration[TwilioConfig.TWILIO_PHONE_NUMBER];
        }

        public async Task<MessageResource> SendSmsAsync(SmsDto smsDto)
        {
            try
            {
                smsDto.SenderNumber = _twilioPhoneNumber;

                var message = MessageResource.CreateAsync(
                to: new PhoneNumber(smsDto.ReceiverNumber),
                from: new PhoneNumber(smsDto.SenderNumber),
                body: smsDto.Message,
                client: _twilioClient);

                return await message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Send sms service exception:");
                throw;
            }
        }
    }
}
