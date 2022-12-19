using System;
using System.Linq;
using System.Net;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Nager.PublicSuffix;

namespace Digbyswift.Extensions.Http
{
    public static class HttpRequestExtensions
    {
        #region Uri/Url

        public static Uri GetAbsoluteUri(this HttpRequest request)
        {
            return new UriBuilder
            {
                Scheme = request.Scheme,
                Host = request.Host.Host,
                Path = request.Path.ToString(),
                Query = request.QueryString.ToString()
            }.Uri;
        }

        public static string GetAbsoluteUrl(this HttpRequest request)
        {
            return request.GetAbsoluteUri().ToString();
        }

        public static Uri GetAbsoluteBaseUri(this HttpRequest request)
        {
            return new UriBuilder
            {
                Scheme = request.Scheme,
                Host = request.Host.Host
            }.Uri;
        }

        public static string GetAbsoluteBaseUrl(this HttpRequest request)
        {
            return request.GetAbsoluteBaseUri().ToString();
        }
        
        #endregion

        #region Referrer

        /// <summary>
        /// Returns true only if the Referrer header is present and there is a value
        /// </summary>
        public static bool HasReferrer(this HttpRequest request)
        {
            if (!request.Headers.ContainsKey(HeaderNames.Referer))
                return false;

            return !String.IsNullOrWhiteSpace(request.Headers.ContainsKey(HeaderNames.Referer).ToString());
        }

        /// <summary>
        /// Returns null if referrer is not the same host
        /// </summary>
        public static Uri? GetSameHostReferrer(this HttpRequest request, bool allowSubDomains = false)
        {
            var referrerValue = request.Headers[HeaderNames.Referer];
            if (referrerValue.Count == 0)
                return null;

            var referrer = referrerValue[0];
            if (referrer == null || !Uri.TryCreate(referrer, UriKind.RelativeOrAbsolute, out Uri? referringUri))
                return null;

            if (request.GetAbsoluteBaseUri().IsBaseOf(referringUri))
                return referringUri;

            if (!allowSubDomains)
                return null;

            var currentDomainInfo = request.GetDomainInfo();
            var refererDomainInfo = referringUri.GetDomainInfo();

            return currentDomainInfo.RegistrableDomain.Equals(refererDomainInfo.RegistrableDomain,
                StringComparison.OrdinalIgnoreCase)
                ? referringUri
                : null;
        }
        
        /// <summary>
        /// Returns default if referrer is not the same host
        /// </summary>
        public static Uri? GetSameHostReferrerOrDefault(this HttpRequest request, bool allowSubDomains = false, string? defaultReferrer = null)
        {
            return request.GetSameHostReferrer(allowSubDomains) ?? (defaultReferrer != null ? new UriBuilder()
            {
                Path = defaultReferrer,
                Scheme = request.Scheme,
                Host = request.Host.Host
            }.Uri : null);
        }

        #endregion

        #region Request properties
        
        public static bool IsGetMethod(this HttpRequest request)
        {
            return request.Method.Equals(HttpMethods.Get, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsHeadMethod(this HttpRequest request)
        {
            return request.Method.Equals(HttpMethods.Head, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsPostMethod(this HttpRequest request)
        {
            return request.Method.Equals(HttpMethods.Post, StringComparison.OrdinalIgnoreCase);
        }
        
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            const string xRequestedWith = "X-Requested-With";
            const string xRequestedWithValue = "XMLHttpRequest";

            return request.Headers[xRequestedWith] == xRequestedWithValue;
        }
        
        /// <summary>
        /// See: https://learn.microsoft.com/en-us/azure/frontdoor/front-door-http-headers-protocol
        /// </summary>
        public static IPAddress GetClientIp(this HttpRequest request)
        {
            string? clientIp = null;
            
            var azureClientIpHeader = request.Headers["X-Azure-ClientIP"];
            if (azureClientIpHeader.Count > 0)
            {
                clientIp = azureClientIpHeader[0];
            }
            else
            {
                var forwardedIpHeader = request.Headers["X-Forwarded-For"];
                if (forwardedIpHeader.Count > 0)
                {
                    clientIp = forwardedIpHeader[0];
                }
            }
            
            if (clientIp != null && IPAddress.TryParse(clientIp, out IPAddress? ipAddress))
                return ipAddress;

            return request.HttpContext.Connection.RemoteIpAddress;
        }

        public static string? GetUserAgent(this HttpRequest request)
        {
            if (!request.HasUserAgent())
                return null;
            
            return request.Headers[HeaderNames.UserAgent];
        }

        public static bool HasUserAgent(this HttpRequest request, string? specificUserAgent = null)
        {
            if (!request.Headers.ContainsKey(HeaderNames.UserAgent))
                return false;

            if (String.IsNullOrWhiteSpace(specificUserAgent))
                return true;
            
            return request.Headers[HeaderNames.UserAgent].Contains(specificUserAgent, StringComparer.OrdinalIgnoreCase);
        }
        
        #endregion

        /// <summary>
        /// Request-caches Nager.PublicPrefix.DomainParser
        /// </summary>
        public static DomainInfo GetDomainInfo(this HttpRequest request)
        {
            const string domainInfoKey = "Digbyswift.Extensions.Http.DomainInfo";
            
            var domainParser = new DomainParser(new WebTldRuleProvider());

            if (request.HttpContext == null)
                return domainParser.Parse(request.GetAbsoluteBaseUri());
            
            if (request.HttpContext.Items[domainInfoKey] is DomainInfo domainInfo)
                return domainInfo;
            
            domainInfo = domainParser.Parse(request.GetAbsoluteBaseUri());
            request.HttpContext.Items[domainInfoKey] = domainInfo;

            return domainInfo;
        }

        #region Path

        public static bool PathHasExtension(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (String.IsNullOrWhiteSpace(request.Path))
                throw new ArgumentException("Request has no path");
            
            return Core.RegularExpressions.Regex.HasFileExtension.Value.IsMatch(request.Path);
        }
        
        private static readonly string[] ImageExtensions = new string[3]
        {
            ".png", ".jpg", ".jpeg"
        };

        public static bool IsPngOrJpeg(this HttpRequest request)
        {
            var path = request.Path.ToString();

            return ImageExtensions.Any(x => path.EndsWith(x));
        }

        public static bool IsSvg(this HttpRequest request)
        {
            return request.Path.Value.EndsWith(".svg");
        }

        public static string PathAndQueryReplaceKey(this HttpRequest request, string replaceKey, object value)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var uri = request.GetAbsoluteUri();

            // this gets all the query string key value pairs as a collection
            var newQueryString = HttpUtility.ParseQueryString(uri.Query);

            // this removes the key if exists
            newQueryString.Remove(replaceKey);

            // this gets the page path from root without QueryString
            string pagePathWithoutQueryString = uri.GetLeftPart(UriPartial.Path);

            return newQueryString.Count > 0
                ? $"{pagePathWithoutQueryString}?{replaceKey}={value}&{newQueryString}"
                : $"{pagePathWithoutQueryString}?{replaceKey}={value}";
        }

        public static string PathAndQueryWithoutKey(this HttpRequest request, string excludeKey)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var uri = request.GetAbsoluteUri();

            if (String.IsNullOrWhiteSpace(excludeKey) || !request.Query.ContainsKey(excludeKey))
                return uri.ToString();

            // this gets all the query string key value pairs as a collection
            var newQueryString = HttpUtility.ParseQueryString(uri.Query);

            // this removes the key if exists
            newQueryString.Remove(excludeKey);

            // this gets the page path from root without QueryString
            string pagePathWithoutQueryString = uri.GetLeftPart(UriPartial.Path);

            return newQueryString.Count > 0
                ? $"{pagePathWithoutQueryString}?{newQueryString}"
                : pagePathWithoutQueryString;
        }
        
        #endregion
    }
}