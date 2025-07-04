using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Digbyswift.Extensions.Http.Tests.MockObjects;

public class MockHeaderCollection : Dictionary<string, StringValues>, IHeaderDictionary
{
    public long? ContentLength { get; set; }
}
