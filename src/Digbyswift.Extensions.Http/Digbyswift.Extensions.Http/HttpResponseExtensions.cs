using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Digbyswift.Extensions.Http
{
	public static class HttpResponseExtensions
	{
		private static readonly string CacheControlValue = "no-cache, no-store, must-revalidate";
		private static readonly string ExpiresValue = "-1";
		private static readonly string PragmaValue = "no-cache";

		public static void SetNoCacheHeaders(this HttpResponse response)
		{
			response.Headers[HeaderNames.CacheControl] = CacheControlValue;
			response.Headers[HeaderNames.Expires] = ExpiresValue;
			response.Headers[HeaderNames.Pragma] = PragmaValue;
		}
	}
}