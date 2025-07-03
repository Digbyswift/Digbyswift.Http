using System;
using System.Collections.Generic;
using System.Net.Http;
using Digbyswift.Http.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Digbyswift.Extensions.Http.Tests.Extensions.HttpRequests;

[TestFixture]
public class RequestExtensionTests
{
    private HttpRequest _sut = null!;

    [SetUp]
    public void Setup()
    {
        var context = Substitute.For<HttpContext>();

        _sut = context.Request;
        _sut.Method = HttpMethod.Get.Method;
        _sut.Scheme = "http";
        _sut.Host = new HostString("localhost");
    }

    #region PathHasExtension

    [TestCase("/.x")]
    [TestCase("/.X")]
    [TestCase("/.txt")]
    [TestCase("/.TXT")]
    [TestCase("/0.jpg")]
    [TestCase("/0.JPEG")]
    [TestCase("/a/a.x")]
    [TestCase("/a/a.X")]
    [TestCase("/a/a.jpeg")]
    [TestCase("/a/a.JPEG")]
    public void PathHasExtension_ReturnsTrue_WhenPathHasAnExtension(string path)
    {
        // Arrange
        _sut.Path = new PathString(path);

        // Act
        var result = _sut.PathHasExtension();

        // Assert
        Assert.That(result, Is.True);
    }

    [TestCase("/")]
    [TestCase("/0")]
    [TestCase("/0/")]
    [TestCase("/a")]
    [TestCase("/a/")]
    [TestCase("/A")]
    [TestCase("/A/")]
    [TestCase("/a/b")]
    [TestCase("/a/b/")]
    [TestCase("/A/B")]
    [TestCase("/A/B/")]
    public void PathHasExtension_ReturnsFalse_WhenPathHasNoExtension(string path)
    {
        // Arrange
        _sut.Path = new PathString(path);

        // Act
        var result = _sut.PathHasExtension();

        // Assert
        Assert.That(result, Is.False);
    }

    [TestCase("A", "A", "5")]
    [TestCase("a", "a", "5")]
    [TestCase("ab", "ab", "5")]
    public void QueryOnly_ReturnsCorrectString_WhenQueryHasKey(string query, string key, string queryValue)
    {
        // Arrange
        var dictionary = new Dictionary<string, StringValues>()
        {
            { query, queryValue }
        };
        _sut.Query = new QueryCollection(dictionary);

        // Act
        var result = _sut.QueryOnly(key);

        // Assert
        var returnQuery = $"?{query}={queryValue}";
        ClassicAssert.AreEqual(result, returnQuery);
    }

    [TestCase("A", "B", "5")]
    [TestCase("a", "b", "5")]
    [TestCase("ab", "ba", "5")]
    public void QueryOnly_ReturnsEmptyString_WhenQueryHasNoKeyAndNoDefault(string query, string key, string queryValue)
    {
        // Arrange
        var dictionary = new Dictionary<string, StringValues>()
        {
            { query, queryValue }
        };
        _sut.Query = new QueryCollection(dictionary);

        // Act
        var result = _sut.QueryOnly(key);

        // Assert
        ClassicAssert.AreEqual(result, String.Empty);
    }

    #endregion
}
