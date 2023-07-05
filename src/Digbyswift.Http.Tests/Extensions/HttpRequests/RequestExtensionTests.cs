using System.Net.Http;
using Digbyswift.Http.Extensions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;

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
    
    #endregion
    
}