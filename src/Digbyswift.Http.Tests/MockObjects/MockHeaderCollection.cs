using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Digbyswift.Extensions.Http.Tests.MockObjects;

#pragma warning disable CS8644 // Type does not implement interface member. Nullability of reference types in interface implemented by the base type doesn't match.
public class MockHeaderCollection : Dictionary<string, StringValues>, IHeaderDictionary
{
    public long? ContentLength { get; set; }
}
