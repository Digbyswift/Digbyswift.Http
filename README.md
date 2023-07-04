# Digbyswift.Http

[![NuGet version (Digbyswift.Http)](https://img.shields.io/nuget/v/Digbyswift.Http.svg)](https://www.nuget.org/packages/Digbyswift.Http/)
![Build status](https://dev.azure.com/digbyswift/Digbyswift%20-%20OSS%20Packages/_apis/build/status/Build%20Digbyswift.Http)

A library of useful classes and HTTP-based extensions.

> Please note, as of v1.0.3 this package has been renamed from Digbyswift.Extensions.Http to Digbyswift.Http.

## Uri

- `ToBaseUri()`
- `ToBareUri()`
- `ToBareUrl()`
- `GetDomainInfo()`

## HttpRequest

#### ForwardedHttpRequest

- `AsForwarded()`


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
