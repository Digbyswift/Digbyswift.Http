using System;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Digbyswift.Http.Extensions
{
    public static class HttpResponseMessageExtensions
    {
        public static readonly HttpStatusCode[] RetryStatusCodes = {
            HttpStatusCode.Unauthorized, // 401
            HttpStatusCode.Forbidden, // 403
            HttpStatusCode.RequestTimeout, // 408
#if NETSTANDARD2_0            
			(HttpStatusCode)429, // 429
#else
            HttpStatusCode.TooManyRequests, // 429
#endif
            HttpStatusCode.InternalServerError, // 500
            HttpStatusCode.BadGateway, // 502
            HttpStatusCode.ServiceUnavailable, // 503
            HttpStatusCode.GatewayTimeout, // 504
        };

        public static bool IsStatusCodeSuitableForRetry(this HttpResponseMessage message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return RetryStatusCodes.Contains(message.StatusCode);
        }

    }
}
