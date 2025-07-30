using System;
using System.Net.Http;
using Digbyswift.Http.Extensions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;

namespace Digbyswift.Extensions.Http.Tests.Extensions.HttpRequests.PathExtensions;

[TestFixture]
public class PathAndQueryPathExtensionTests
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

    #region PathandQueryWithoutKey

    [Test]
    public void PathAndQueryReplaceKey_ThrowsArgumentNullException_IfRequestIsNull()
    {
        // Arrange
        HttpRequest request = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => request.PathAndQueryReplaceValueOfKey(string.Empty, string.Empty));
    }

    [Test]
    public void PathAndQueryReplaceKey_ReturnsEmptyQuerystring_IfValueObjectIsNull()
    {
        // Arrange
        const string originalPath = "/testing/";
        const string expectedResult = "/testing/";

        _sut.Path = new PathString(originalPath);

        // Act
        var result = _sut.PathAndQueryReplaceValueOfKey(string.Empty, null);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [TestCase(100, "/testing/?test=100")]
    [TestCase(100.00d, "/testing/?test=100")]
    [TestCase(100.01d, "/testing/?test=100.01")]
    [TestCase(true, "/testing/?test=True")]
    public void PathAndQueryReplaceKey_ReturnsQuerystring_IfValueObjectCanBeParsedToString(object value, string expectedResult)
    {
        // Arrange
        const string originalPath = "/testing/";
        const string key = "test";

        _sut.Path = new PathString(originalPath);
        _sut.QueryString = _sut.QueryString.Add(QueryString.Create(key, value.ToString()));

        // Act
        var result = _sut.PathAndQueryReplaceValueOfKey(key, value);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [TestCase("/testing")]
    [TestCase("/testing/")]
    [TestCase("/testing-again/")]
    public void PathAndQueryReplaceKey_ReturnsOriginalPath_WhenKeyIsEmpty_AndRequestHasNoQuerystring(string path)
    {
        // Arrange
        _sut.Path = new PathString(path);

        // Act
        var result = _sut.PathAndQueryReplaceValueOfKey(string.Empty, string.Empty);

        // Assert
        Assert.That(result, Is.EqualTo(path));
    }

    [TestCase("/testing")]
    [TestCase("/testing/")]
    [TestCase("/testing-again/")]
    public void PathAndQueryReplaceKey_ReturnsOriginalPath_WhenKeyIsNotPresent(string path)
    {
        // Arrange
        const string nonMatchedKey = "mismatch";

        _sut.Path = new PathString(path);

        // Act
        var result = _sut.PathAndQueryReplaceValueOfKey(nonMatchedKey, string.Empty);

        // Assert
        Assert.That(result, Is.EqualTo(path));
    }

    [Test]
    public void PathAndQueryReplaceKey_ReturnsOriginalPathAndQuery_WhenKeyIsNotPresent()
    {
        // Arrange
        const string originalPath = "/testing/";
        const string expectedPath = "/testing/?test=testquery";

        const string keyToReplaceValueFor = "test";
        const string replacementValue = "testquery";
        const string nonMatchedKey = "mismatch";

        _sut.Path = new PathString(originalPath);
        _sut.QueryString = _sut.QueryString.Add(QueryString.Create(keyToReplaceValueFor, replacementValue));

        // Act
        var result = _sut.PathAndQueryReplaceValueOfKey(nonMatchedKey, string.Empty);

        // Assert
        Assert.That(result, Is.EqualTo(expectedPath));
    }

    [Test]
    public void PathAndQueryReplaceKey_ReturnsPathAndQuery_WithValueOfKeyReplaced()
    {
        // Arrange
        const string originalPath = "/testing/";
        const string expectedPath = "/testing/?test=replaced";

        const string keyToReplaceValueFor = "test";
        const string replacementValue = "replaced";

        _sut.Path = PathString.FromUriComponent(originalPath);
        _sut.QueryString = _sut.QueryString.Add(QueryString.Create(keyToReplaceValueFor, "testquery"));

        // Act
        var result = _sut.PathAndQueryReplaceValueOfKey(keyToReplaceValueFor, replacementValue);

        // Assert
        Assert.That(result, Is.EqualTo(expectedPath));
    }

    [Test]
    public void PathAndQueryReplaceKey_ReturnsPathAndQuery_WithMultipleValuesOfSameKeyReplaced()
    {
        // Arrange
        const string originalPath = "/testing/";
        const string expectedPath = "/testing/?test=replaced";

        const string keyToReplaceValueFor = "test";
        const string replacementValue = "replaced";

        _sut.Path = new PathString(originalPath);
        _sut.QueryString = _sut.QueryString.Add(QueryString.Create(keyToReplaceValueFor, "testquery"));
        _sut.QueryString = _sut.QueryString.Add(QueryString.Create(keyToReplaceValueFor, "testagain"));

        // Act
        var result = _sut.PathAndQueryReplaceValueOfKey(keyToReplaceValueFor, replacementValue);

        // Assert
        Assert.That(result, Is.EqualTo(expectedPath));
    }

    #endregion
}
