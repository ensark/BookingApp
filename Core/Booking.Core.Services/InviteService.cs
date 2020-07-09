using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Hangfire;
using Booking.Core.Domain.DTOs;
using Booking.Core.Domain.Entities;
using Booking.Core.Services.Interfaces;
using Booking.Infrastructure.Common.Interfaces;
using Booking.Infrastructure.Database;
using static Twilio.Rest.Api.V2010.Account.CallResource;

namespace Booking.Core.Services
{
    public class InviteService : IInviteService
    {
        private readonly BookingDBContext _context;
        private readonly ILogger<InviteService> _logger;
        private readonly ISmsService _smsService;

        private static readonly ThreadLocal<Random> _random = new ThreadLocal<Random>(() => new Random());

        public InviteService(BookingDBContext context, ILogger<InviteService> logger, ISmsService smsService)
        {
            _context = context;
            _logger = logger;
            _smsService = smsService;
        }

        public async Task SendInvitesAsync(long userId, IEnumerable<string> phoneNumbers, CancellationToken cancellationToken)
        {
            try
            {
                var inviter = await _context.Users.Where(x => x.Id == userId)
                                                  .Select(x => new UserDto { FirstName = x.FirstName, LastName = x.LastName })
                                                  .FirstOrDefaultAsync(cancellationToken);

                foreach (var phoneNumber in phoneNumbers)
                {
                    try
                    {
                        var sendSms = new SmsDto
                        {
                            Message = $"{inviter.FirstName} {inviter.LastName} invite you to join booking platfrom on www.bookingwith.com",
                            ReceiverNumber = phoneNumber
                        };

                        var sendSmsResponse = await _smsService.SendSmsAsync(sendSms);

                        if (sendSmsResponse.Status != StatusEnum.Failed || sendSmsResponse.Status != StatusEnum.Canceled)
                        {
                            var invite = new Invite
                            {
                                CreatedBy = userId.ToString(),
                                InviterId = userId,
                                FriendNumber = phoneNumber,
                                VoucherCodeSent = false
                            };

                            _context.Invites.Add(invite);
                            await _context.SaveChangesAsync(cancellationToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        continue;
                        throw new Exception($"SMS was not sent to {phoneNumber} " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Send invites service exception:");
                throw;
            }
        }

        public async Task CheckAcceptedInvitesAsync(IJobCancellationToken cancellationToken)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var joinedUser = await _context.Users.Where(y => _context.Invites.Any(z => z.FriendNumber == y.Phone && !z.VoucherCodeSent)).Select(x => x.Phone).ToListAsync();

                    var inviters = await _context.Invites.Where(x => joinedUser.Contains(x.FriendNumber))
                                                         .Select(x => new UserDto
                                                         {
                                                             FirstName = x.Inviter.FirstName,
                                                             LastName = x.Inviter.LastName,
                                                             Phone = x.Inviter.Phone,
                                                             UserId = x.InviterId
                                                         })
                                                         .Distinct()
                                                         .ToListAsync();

                    foreach (var inviterUser in inviters)
                    {
                        var code = GenerateCode();

                        var sendSms = new SmsDto
                        {
                            Message = $"Dear {inviterUser.FirstName} {inviterUser.LastName} you got vaucher code for discount because your friends joined to BookWith app. VAUCHER CODE: {code}",
                            ReceiverNumber = inviterUser.Phone
                        };

                        var sendSmsResponse = await _smsService.SendSmsAsync(sendSms);

                        if (sendSmsResponse.Status != StatusEnum.Failed || sendSmsResponse.Status != StatusEnum.Canceled)
                        {
                            var voucherCode = new VoucherCode
                            {
                                CreatedBy = inviterUser.UserId.ToString(),
                                UserId = inviterUser.UserId,
                                Code = code,
                                IsUsed = false
                            };

                            _context.VoucherCodes.Add(voucherCode);

                            var changeInviteStatus = await _context.Invites.Where(x => x.InviterId == inviterUser.UserId).FirstOrDefaultAsync();
                            changeInviteStatus.VoucherCodeSent = true;
                        }
                    }
                                                       
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError(ex, "Check accepted service exception:");
                    throw;
                }
            }
        }

        private static string GenerateCode()
        {
            char[] chars = "ACDEFGHJKLMNPQRTUVWXYZ234679".ToCharArray();

            var sb = new StringBuilder();
            for (int i = 0; i < 8; i++)
            {
                int num = _random.Value.Next(0, chars.Length);
                sb.Append(chars[num]);
            }
            return sb.ToString();
        }
    }
}

