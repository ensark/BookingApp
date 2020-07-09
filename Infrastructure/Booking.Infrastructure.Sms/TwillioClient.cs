using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Twilio.Clients;
using Twilio.Http;
using HttpClient = System.Net.Http.HttpClient;

namespace Booking.Infrastructure.Sms
{
    public class TwillioClient : ITwilioRestClient
    {
        private readonly ITwilioRestClient _innerClient;

        public TwillioClient(IConfiguration configuration, HttpClient httpClient)
        {
            _innerClient = new TwilioRestClient(
                configuration[TwilioConfig.TWILIO_ACCOUNT_SID],
                configuration[TwilioConfig.TWILIO_AUTH_TOKEN],
                httpClient: new SystemNetHttpClient(httpClient));
        }

        public Response Request(Request request) => _innerClient.Request(request);
        public Task<Response> RequestAsync(Request request) => _innerClient.RequestAsync(request);
        public string AccountSid => _innerClient.AccountSid;
        public string Region => _innerClient.Region;
        public Twilio.Http.HttpClient HttpClient => _innerClient.HttpClient;
    }
}
