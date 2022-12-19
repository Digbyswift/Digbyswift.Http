# Digbyswift.Extensions.Http

A library of useful HTTP-based extensions.

## Uri

- `ToBaseUri()`
- `ToBareUri()`
- `ToBareUrl()`
- `GetDomainInfo()`

## HttpRequest

#### Uri/Url

- `GetAbsoluteUri()`
- `GetAbsoluteUrl()`
- `GetAbsoluteBaseUri()`
- `GetAbsoluteBaseUrl()`

#### Referrer
- `HasReferrer()`
- `GetSameHostReferrer(bool allowSubDomains = false)`
- `GetSameHostReferrerOrDefault(bool allowSubDomains = false, string? defaultReferrer = null)`

#### Properties
- `IsGetMethod()`
- `IsHeadMethod()`
- `IsPostMethod()`
- `IsAjaxRequest()`
- `GetClientIp()`
- `GetUserAgent()`
- `HasUserAgent(string? specificUserAgent = null)`
- `GetDomainInfo()`

#### Paths
- `PathHasExtension()`
- `IsPngOrJpeg()`
- `IsSvg()`
- `PathAndQueryReplaceKey(string replaceKey, object value)`
- `PathAndQueryWithoutKey(string excludeKey)`

## HttpResponse
- `SetNoCacheHeaders()`

## HttpResponseMessage
- `IsStatusCodeSuitableForRetry()`

## PathString
- `Segments()`
- `SegmentAt(int index)`
- `SegmentAtOrDefault(int index, string? defaultSegment = null)`

## HttpContext

- `IsAuthenticated()`
