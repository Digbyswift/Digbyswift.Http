using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Digbyswift.Http;

public class ForwardedHttpRequest : HttpRequest
{
    // ReSharper disable once InconsistentNaming
    private const string ForwardedHostHeaderName = "X-Forwarded-Host";

    private readonly HttpRequest _httpRequest;
    public override HostString Host
    {
        get => new(GetHost());
        set => _httpRequest.Host = value;
    }

    public override HttpContext HttpContext => _httpRequest.HttpContext;
    public override IHeaderDictionary Headers
    {
        get
        {
            _httpRequest.Headers[HeaderNames.Host] = GetHost();
            if (_httpRequest.Headers.ContainsKey(ForwardedHostHeaderName))
            {
                _httpRequest.Headers.Remove(ForwardedHostHeaderName);
            }

            return _httpRequest.Headers;
        }
    }

    #region Decorated members

    public override bool HasFormContentType => _httpRequest.HasFormContentType;

    public override string Method
    {
        get => _httpRequest.Method;
        set => _httpRequest.Method = value;
    }

    public override string Scheme
    {
        get => _httpRequest.Scheme;
        set => _httpRequest.Scheme = value;
    }

    public override bool IsHttps
    {
        get => _httpRequest.IsHttps;
        set => _httpRequest.IsHttps = value;
    }

    public override PathString PathBase
    {
        get => _httpRequest.PathBase;
        set => _httpRequest.PathBase = value;
    }

    public override PathString Path
    {
        get => _httpRequest.Path;
        set => _httpRequest.Path = value;
    }

    public override QueryString QueryString
    {
        get => _httpRequest.QueryString;
        set => _httpRequest.QueryString = value;
    }

    public override IQueryCollection Query
    {
        get => _httpRequest.Query;
        set => _httpRequest.Query = value;
    }

    public override string Protocol
    {
        get => _httpRequest.Protocol;
        set => _httpRequest.Protocol = value;
    }

    public override IRequestCookieCollection Cookies
    {
        get => _httpRequest.Cookies;
        set => _httpRequest.Cookies = value;
    }

    public override long? ContentLength
    {
        get => _httpRequest.ContentLength;
        set => _httpRequest.ContentLength = value;
    }

    public override string? ContentType
    {
        get => _httpRequest.ContentType;
        set => _httpRequest.ContentType = value;
    }

    public override Stream Body
    {
        get => _httpRequest.Body;
        set => _httpRequest.Body = value;
    }

    public override IFormCollection Form
    {
        get => _httpRequest.Form;
        set => _httpRequest.Form = value;
    }

    public override Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken = default) => _httpRequest.ReadFormAsync(cancellationToken);

    #endregion

    public ForwardedHttpRequest(HttpRequest httpRequest)
    {
        _httpRequest = httpRequest;
    }

    private string? GetHost()
    {
        if (_httpRequest.Headers.TryGetValue(ForwardedHostHeaderName, out var forwardedHost) && !String.IsNullOrWhiteSpace(forwardedHost))
            return forwardedHost;

        return _httpRequest.Host.HasValue
            ? _httpRequest.Host.Value
            : String.Empty;
    }
}
