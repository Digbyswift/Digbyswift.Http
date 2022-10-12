using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace Digbyswift.Extensions.Http
{
	public static class HttpResponseMessageExtensions
	{
		private const string CacheControl = "Cache-Control";
		private const string CacheControlValue = "no-cache, no-store, must-revalidate";
		private const string Expires = "Expires";
		private const string ExpiresValue = "-1";
		private const string Pragma = "Pragma";
		private const string PragmaValue = "no-cache";

        internal static readonly HttpStatusCode[] RetryStatusCodes = {
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

		public static void SetNoCacheHeaders(this HttpResponse response)
		{
			response.Headers[CacheControl] = CacheControlValue;
			response.Headers[Expires] = ExpiresValue;
			response.Headers[Pragma] = PragmaValue;
		}
	}
}