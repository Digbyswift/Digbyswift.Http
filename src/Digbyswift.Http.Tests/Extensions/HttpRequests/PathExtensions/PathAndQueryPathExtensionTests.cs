using System;
using System.Collections.Generic;
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
        _sut.Path = new PathString("/testing/");
    }

    #region PathAndQueryReplaceValueOfKey

    [Test]
    public void PathAndQueryReplaceValueOfKey_ThrowsArgumentNullException_WhenRequestIsNull()
    {
        // Arrange
        HttpRequest request = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => request.PathAndQueryReplaceValueOfKey(String.Empty, String.Empty));
    }

    [Test]
    public void PathAndQueryReplaceValueOfKey_ReturnsEmptyQuerystring_WhenValueObjectIsNull()
    {
        // Arrange
        const string expectedResult = "/testing/";

        // Act
        var result = _sut.PathAndQueryReplaceValueOfKey(String.Empty, null);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [TestCase(100, "/testing/?test=100")]
    [TestCase(100.00d, "/testing/?test=100")]
    [TestCase(100.01d, "/testing/?test=100.01")]
    [TestCase(true, "/testing/?test=True")]
    public void PathAndQueryReplaceValueOfKey_ReturnsReplacedQuerystring_WhenValueIsStringified(object value, string expectedResult)
    {
        // Arrange
        const string key = "test";

        // Act
        var result = _sut.PathAndQueryReplaceValueOfKey(key, value);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [TestCaseSource(nameof(GuidTestCases))]
    public void PathAndQueryReplaceValueOfKey_ReturnsReplacedQuerystring_WhenValueIsStringified(GuidTestData testData)
    {
        // Arrange
        const string key = "test";

        // Act
        var result = _sut.PathAndQueryReplaceValueOfKey(key, testData.Source);

        // Assert
        Assert.That(result, Is.EqualTo(testData.ExpectedResult));
    }

    [TestCase("/testing")]
    [TestCase("/testing/")]
    [TestCase("/testing-again/")]
    public void PathAndQueryReplaceValueOfKey_ReturnsOriginalPath_WhenKeyIsEmpty(string path)
    {
        // Arrange & Act
        var result = _sut.PathAndQueryReplaceValueOfKey(String.Empty, String.Empty);

        // Assert
        Assert.That(result, Is.EqualTo(path));
    }

    [TestCase("/testing")]
    [TestCase("/testing/")]
    [TestCase("/testing-again/")]
    public void PathAndQueryReplaceValueOfKey_ReturnsOriginalPath_WhenKeyIsNotPresent(string path)
    {
        // Arrange
        const string nonMatchedKey = "mismatch";

        _sut.Path = new PathString(path);

        // Act
        var result = _sut.PathAndQueryReplaceValueOfKey(nonMatchedKey, String.Empty);

        // Assert
        Assert.That(result, Is.EqualTo(path));
    }

    [Test]
    public void PathAndQueryReplaceValueOfKey_ReturnsOriginalPathAndQuery_WhenKeyIsNotPresent()
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
        var result = _sut.PathAndQueryReplaceValueOfKey(nonMatchedKey, String.Empty);

        // Assert
        Assert.That(result, Is.EqualTo(expectedPath));
    }

    [Test]
    public void PathAndQueryReplaceValueOfKey_ReturnsReplacementValue_WhenKeyIsMatched()
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
    public void PathAndQueryReplaceValueOfKey_ReturnsReplacementValue_WhenMultipleKeyMatchesArePresent()
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

    #region PathAndQueryWithoutKey

    [Test]
    public void PathAndQueryWithoutKey_ThrowsArgumentNullException_WhenRequestIsNull()
    {
        // Arrange
        HttpRequest request = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => request.PathAndQueryWithoutKey(String.Empty));
    }

    [Test]
    public void PathAndQueryWithoutKey_ReturnsOriginalPath_WhenKeyIsNotPresent()
    {
        // Arrange
        const string originalPath = "/testing/";
        const string expectedPath = "/testing/?test=testquery";
        const string keyToReplaceValueFor = "test";
        const string mismatchedKey = "mismatch";

        _sut.Path = new PathString(originalPath);
        _sut.QueryString = _sut.QueryString.Add(QueryString.Create(keyToReplaceValueFor, "testquery"));

        // Act
        var result = _sut.PathAndQueryWithoutKey(mismatchedKey);

        // Assert
        Assert.That(result, Is.EqualTo(expectedPath));
    }

    [Test]
    public void PathAndQueryWithoutKey_ReturnsOriginalPath_WhenKeyAndQueryIsNotPresent()
    {
        // Arrange
        const string originalPath = "/testing/";
        const string expectedPath = "/testing/";
        const string mismatchedKey = "mismatch";

        _sut.Path = new PathString(originalPath);

        // Act
        var result = _sut.PathAndQueryWithoutKey(mismatchedKey);

        // Assert
        Assert.That(result, Is.EqualTo(expectedPath));
    }

    #endregion

    #region PathAndQueryWithoutKeys

    [Test]
    public void PathAndQueryWithoutKeys_ThrowsArgumentNullException_WhenRequestIsNull()
    {
        // Arrange
        HttpRequest request = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => request.PathAndQueryWithoutKeys(String.Empty));
    }

    [Test]
    public void PathAndQueryWithoutKeys_ReturnsOriginalPath_WhenKeysAreNotPresent()
    {
        // Arrange
        const string originalPath = "/testing/";
        const string expectedPath = "/testing/?test=testquery";
        const string keyToReplaceValueFor = "test";
        const string mismatchedKey = "mismatch";
        const string mismatchedKeyTwo = "mismatch2";

        _sut.Path = new PathString(originalPath);
        _sut.QueryString = _sut.QueryString.Add(QueryString.Create(keyToReplaceValueFor, "testquery"));

        // Act
        var result = _sut.PathAndQueryWithoutKeys(mismatchedKey, mismatchedKeyTwo);

        // Assert
        Assert.That(result, Is.EqualTo(expectedPath));
    }

    [Test]
    public void PathAndQueryWithoutKeys_ReturnsOriginalPath_WhenKeysAndQueryAreNotPresent()
    {
        // Arrange
        const string originalPath = "/testing/";
        const string expectedPath = "/testing/";
        const string mismatchedKey = "mismatch";

        _sut.Path = new PathString(originalPath);

        // Act
        var result = _sut.PathAndQueryWithoutKeys(mismatchedKey);

        // Assert
        Assert.That(result, Is.EqualTo(expectedPath));
    }

    #endregion

    #region PathAndQueryWithoutKeys

    [Test]
    public void QueryOrDefault_ThrowsArgumentNullException_WhenRequestIsNull()
    {
        // Arrange
        HttpRequest request = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => request.QueryOrDefault(String.Empty, String.Empty));
    }

    [Test]
    [Ignore("In progress")]
    public void QueryOrDefault_ReturnsQuerystring_WhenKeyIsMatched()
    {
        // Arrange
        const string originalPath = "/testing/";
        const string expectedResult = "?test=testquery";
        const string keyToFind = "test";

        _sut.Path = new PathString(originalPath);

        // Act
        _sut.QueryString = _sut.QueryString.Add(QueryString.Create(keyToFind, "testquery"));

        var result = _sut.QueryOrDefault(keyToFind, string.Empty);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    [Ignore("In progress")]
    public void QueryOrDefault_ReturnsFallBackQuerystring_WhenKeyIsNotMatched()
    {
        // Arrange
        const string keyToFind = "test";
        const string keyValue = "testquery";
        const string expectedResult = $"?{keyToFind}={keyValue}";

        _sut.Path = new PathString("/testing/");

        // Act
        _sut.QueryString = _sut.QueryString.Add(QueryString.Create(keyToFind, keyValue));

        var result = _sut.QueryOrDefault(keyToFind, String.Empty);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    #endregion

    private static IEnumerable<GuidTestData> GuidTestCases()
    {
        yield return new GuidTestData(Guid.NewGuid(), String.Empty);
        yield return new GuidTestData(new Guid("c73e70c0-8cc1-42f9-82d1-bef160557267"), String.Empty);
    }

    public class GuidTestData(Guid source, string expectedResult)
    {
        public Guid Source { get; } = source;
        public string ExpectedResult { get; } = expectedResult;
    }
}
