using System;
using System.Net;
using System.Web;
using Digbyswift.Core.Constants;
using Microsoft.AspNetCore.Http;
using Nager.PublicSuffix;

namespace Digbyswift.Extensions.Http
{
    public static class HttpRequestExtensions
    {
        private const string XRequestedWith = "X-Requested-With";
        private const string Referer = "Referer";

        public static bool IsGetMethod(this HttpRequest request)
        {
            return request.Method.Equals(HttpMethod.Get, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsHeadMethod(this HttpRequest request)
        {
            return request.Method.Equals(HttpMethod.Head, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsPostMethod(this HttpRequest request)
        {
            return request.Method.Equals(HttpMethod.Post, StringComparison.OrdinalIgnoreCase);
        }

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

        public static Uri GetAbsoluteBaseUri(this HttpRequest request)
        {
            return new UriBuilder
            {
                Scheme = request.Scheme,
                Host = request.Host.Host
            }.Uri;
        }

        public static string GetAbsoluteUrl(this HttpRequest request)
        {
            return request.GetAbsoluteUri().ToString();
        }

        public static string GetAbsoluteBaseUrl(this HttpRequest request)
        {
            return request.GetAbsoluteBaseUri().ToString();
        }

        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return request.Headers[XRequestedWith] == "XMLHttpRequest";
        }

        public static Uri SafeUrlReferrer(this HttpRequest request, bool allowSubDomains = false)
        {
            var referrerValue = request.Headers[Referer];
            if (referrerValue.Count == 0)
                return null;

            var referrer = referrerValue[0];
            if (referrer == null || !Uri.TryCreate(referrer, UriKind.RelativeOrAbsolute, out Uri referringUri))
                return null;

            if (request.GetAbsoluteBaseUri().IsBaseOf(referringUri))
                return referringUri;

            if (!allowSubDomains)
                return null;

            var currentDomainInfo = request.DomainInfo();
            var refererDomainInfo = referringUri.DomainInfo();

            return currentDomainInfo.RegistrableDomain.Equals(refererDomainInfo.RegistrableDomain,
                StringComparison.OrdinalIgnoreCase)
                ? referringUri
                : null;
        }

        public static DomainInfo DomainInfo(this HttpRequest request)
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

        public static Uri SafeUrlReferrerOrDefault(this HttpRequest request, string defaultReferrer = StringConstants.ForwardSlash)
        {
            return request.SafeUrlReferrer() ?? new UriBuilder()
            {
                Path = defaultReferrer,
                Scheme = request.Scheme,
                Host = request.Host.Host
            }.Uri;
        }

        /// <summary>
        /// See: https://learn.microsoft.com/en-us/azure/frontdoor/front-door-http-headers-protocol
        /// </summary>
        public static IPAddress GetClientIp(this HttpRequest request)
        {
            string clientIp = null;
            
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
            
            if (clientIp != null && IPAddress.TryParse(clientIp, out IPAddress ipAddress))
                return ipAddress;

            return request.HttpContext.Connection.RemoteIpAddress;
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
    }
}