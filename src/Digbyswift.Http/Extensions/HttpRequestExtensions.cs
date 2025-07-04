using System;
using System.Linq;
using System.Net;
using System.Web;
using Digbyswift.Core.Constants;
using Digbyswift.Core.Extensions;
using Digbyswift.Http.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Nager.PublicSuffix;

namespace Digbyswift.Http.Extensions;

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
    /// Returns true only if the Referrer header is present and there is a value.
    /// </summary>
    public static bool HasReferrer(this HttpRequest request)
    {
        return request.Headers.TryGetValue(HeaderNames.Referer, out var headerValue) && !String.IsNullOrWhiteSpace(headerValue);
    }

    /// <summary>
    /// Returns the Referrer header value as a Uri if present, valid and not whitespace, otherwise null.
    /// </summary>
    public static Uri? GetReferrer(this HttpRequest request)
    {
        if (!request.Headers.TryGetValue(HeaderNames.Referer, out var headerValue) || String.IsNullOrWhiteSpace(headerValue))
            return null;

        if (!Uri.TryCreate(headerValue, UriKind.Absolute, out var referringUri))
            return null;

        return referringUri;
    }

    /// <summary>
    /// Returns the Referrer header value if present and not whitespace, otherwise null.
    /// </summary>
    public static string? GetRawReferrer(this HttpRequest request)
    {
        if (!request.Headers.TryGetValue(HeaderNames.Referer, out var headerValue) || String.IsNullOrWhiteSpace(headerValue))
            return null;

        return headerValue;
    }

    /// <summary>
    /// Returns true if the Referrer header value as a Uri if present, valid and not whitespace, otherwise false.
    /// </summary>
    public static bool TryGetReferrer(this HttpRequest request, out Uri? referringUri)
    {
        if (!request.Headers.TryGetValue(HeaderNames.Referer, out var headerValue) || String.IsNullOrWhiteSpace(headerValue))
        {
            referringUri = null;
            return false;
        }

        return Uri.TryCreate(headerValue, UriKind.RelativeOrAbsolute, out referringUri);
    }

    /// <summary>
    /// Returns null if referrer is not the same host.
    /// </summary>
    public static Uri? GetSameHostReferrer(this HttpRequest request, bool allowSubDomains = false)
    {
        var referrerValue = request.Headers[HeaderNames.Referer];
        if (referrerValue.Count == 0)
            return null;

        var referrer = referrerValue[0];
        if (referrer == null || !Uri.TryCreate(referrer, UriKind.RelativeOrAbsolute, out var referringUri))
            return null;

        if (request.GetAbsoluteBaseUri().IsBaseOf(referringUri))
            return referringUri;

        if (!allowSubDomains)
            return null;

        var currentDomainInfo = request.GetDomainInfo();
        var refererDomainInfo = referringUri.GetDomainInfo();

        return currentDomainInfo.RegistrableDomain?.Equals(refererDomainInfo.RegistrableDomain,
            StringComparison.OrdinalIgnoreCase) ?? false
            ? referringUri
            : null;
    }

    public static bool TryGetSameHostReferrer(this HttpRequest request, out Uri? referringUri)
    {
        referringUri = GetSameHostReferrer(request);
        return (referringUri != null);
    }

    /// <summary>
    /// Returns default if referrer is not the same host.
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
        return request.Method.EqualsIgnoreCase(HttpMethods.Get);
    }

    public static bool IsHeadMethod(this HttpRequest request)
    {
        return request.Method.EqualsIgnoreCase(HttpMethods.Head);
    }

    public static bool IsPostMethod(this HttpRequest request)
    {
        return request.Method.EqualsIgnoreCase(HttpMethods.Post);
    }

    /// <summary>
    /// Returns true if the request has the non-standard X-Requested-With header and the value, XMLHttpRequest.
    /// </summary>
    public static bool IsAjaxRequest(this HttpRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        const string xRequestedWithValue = "XMLHttpRequest";

        if (!request.Headers.TryGetValue(NonStandardHeaderNames.XRequestedWith, out var headerValue) || String.IsNullOrWhiteSpace(headerValue))
            return false;

        return headerValue.ToString().EqualsIgnoreCase(xRequestedWithValue);
    }

    /// <summary>
    /// See: https://learn.microsoft.com/en-us/azure/frontdoor/front-door-http-headers-protocol.
    /// </summary>
    public static IPAddress? GetClientIp(this HttpRequest request)
    {
        string? clientIp = null;

        var azureClientIpHeader = request.Headers[NonStandardHeaderNames.XAzureClientIp];
        if (azureClientIpHeader.Count > 0)
        {
            clientIp = azureClientIpHeader[0];
        }
        else
        {
            var forwardedIpHeader = request.Headers[NonStandardHeaderNames.XForwardedFor];
            if (forwardedIpHeader.Count > 0)
            {
                clientIp = forwardedIpHeader[0];
            }
        }

        if (clientIp != null && IPAddress.TryParse(clientIp, out var ipAddress))
            return ipAddress;

        return request.HttpContext.Connection.RemoteIpAddress;
    }

    public static string? GetUserAgent(this HttpRequest request)
    {
        if (!request.HasUserAgent())
            return null;

        return request.Headers[HeaderNames.UserAgent];
    }

    /// <summary>
    /// Returns true if the useragent either has a non-whitespace useragent value, or contains a specific useragent. When
    /// a specific useragent is supplied a match is true if the specific useragent is contained within the useragent value.
    /// </summary>
    public static bool HasUserAgent(this HttpRequest request, string? specificUserAgent = null)
    {
        if (!request.Headers.TryGetValue(HeaderNames.UserAgent, out var userAgentValue) || String.IsNullOrWhiteSpace(userAgentValue))
            return false;

        if (!String.IsNullOrWhiteSpace(specificUserAgent))
            return userAgentValue.Contains(specificUserAgent, StringComparer.OrdinalIgnoreCase);

        return true;
    }

    public static bool AcceptsWebP(this HttpRequest request)
    {
        var acceptHeaderKey = request.Headers.ContainsKey(NonStandardHeaderNames.XForwardedAccept)
            ? NonStandardHeaderNames.XForwardedAccept
            : HeaderNames.Accept;

        var acceptValues = request.Headers[acceptHeaderKey];
        return acceptValues.Count > 0 && (acceptValues[0]?.Contains(MimeTypeConstants.WebP) ?? false);
    }

    #endregion

    /// <summary>
    /// Returns the passed request decorated with any forwarded properties, specifically
    /// Request.Host and Request.Headers["Host"]. If the request has not been forwarded, the
    /// request will remain identical to the passed request.
    /// </summary>
    public static HttpRequest AsForwarded(this HttpRequest request)
    {
        return new ForwardedHttpRequest(request);
    }

    /// <summary>
    /// Request-caches Nager.PublicPrefix.DomainParser.
    /// </summary>
    public static DomainInfo GetDomainInfo(this HttpRequest request)
    {
        const string domainInfoKey = "Digbyswift.Http.DomainInfo";

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

    private static readonly string[] ImageExtensions = { ".png", ".jpg", ".jpeg" };

    public static bool IsPngOrJpeg(this HttpRequest request)
    {
        var path = request.Path.ToString();

        return ImageExtensions.Any(x => path.EndsWith(x, StringComparison.OrdinalIgnoreCase));
    }

    public static bool IsSvg(this HttpRequest request)
    {
        return request.Path.Value.EndsWith(".svg", StringComparison.OrdinalIgnoreCase);
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
        var pagePathWithoutQueryString = uri.GetLeftPart(UriPartial.Path);

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
        var pagePathWithoutQueryString = uri.GetLeftPart(UriPartial.Path);

        return newQueryString.Count > 0
            ? $"{pagePathWithoutQueryString}?{newQueryString}"
            : pagePathWithoutQueryString;
    }

    public static string PathAndQueryWithoutKeys(this HttpRequest request, params string[] excludeKeys)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var uri = request.GetAbsoluteUri();

        if (excludeKeys.Length == 0)
            return uri.ToString();

        // this gets all the query string key value pairs as a collection
        var newQueryString = HttpUtility.ParseQueryString(uri.Query);

        foreach (var key in excludeKeys)
        {
            newQueryString.Remove(key);
        }

        // this gets the page path from root without QueryString
        var pagePathWithoutQueryString = uri.GetLeftPart(UriPartial.Path);

        return newQueryString.Count > 0
            ? $"{pagePathWithoutQueryString}?{newQueryString}"
            : pagePathWithoutQueryString;
    }

    public static string PathAndQueryOnly(this HttpRequest request, string key, string? defaultValue)
    {
        return request.Query.TryGetValue(key, out var value)
        ? $"?{key}={value}"
        : !String.IsNullOrWhiteSpace(defaultValue) ? $"?{key}={defaultValue}" : String.Empty;
    }

    #endregion
}
