# Digbyswift.Http

[![NuGet version (Digbyswift.Http)](https://img.shields.io/nuget/v/Digbyswift.Http.svg)](https://www.nuget.org/packages/Digbyswift.Http/)
[![Build and publish package](https://github.com/Digbyswift/Digbyswift.Http/actions/workflows/dotnet-build-publish.yml/badge.svg)](https://github.com/Digbyswift/Digbyswift.Http/actions/workflows/dotnet-build-publish.yml)

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
- `GetReferrer()`
- `GetRawReferrer()`
- `GetSameHostReferrer(bool allowSubDomains = false)`
- `GetSameHostReferrerOrDefault(bool allowSubDomains = false, string? defaultReferrer = null)`
- `TryGetReferrer()`
- `TryGetSameHostReferrer()`

#### Properties
- `IsGetMethod()`
- `IsHeadMethod()`
- `IsPostMethod()`
- `IsAjaxRequest()`
- `GetClientIp()`
- `GetUserAgent()`
- `HasUserAgent(string? specificUserAgent = null)`
- `GetDomainInfo()`
- `AcceptsWebP`

#### Paths
- `PathHasExtension()`
- `IsPngOrJpeg()`
- `IsSvg()`
- `PathAndQueryReplaceKey(string replaceKey, object value)`
- `PathAndQueryWithoutKey(string excludeKey)`
- `PathAndQueryWithoutKeys(string[] excludeKeys)`

## HttpResponse
- `SetNoCacheHeaders()`

## HttpResponseMessage
- `IsStatusCodeSuitableForRetry()`

## HttpContent
- `ReadAsJsonAsync<T>(JsonSerializerSettings? options = null)`

## PathString
- `Segments()`
- `SegmentAt(int index)`
- `SegmentAtOrDefault(int index, string? defaultSegment = null)`

## HttpContext

- `IsAuthenticated()`
