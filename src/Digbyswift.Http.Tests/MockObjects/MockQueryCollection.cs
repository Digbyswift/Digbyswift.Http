using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Digbyswift.Extensions.Http.Tests.MockObjects
{
    internal class MockQueryCollection(string query) : IQueryCollection
    {
        public StringValues this[string key] => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public ICollection<string> Keys => throw new NotImplementedException();

        public bool ContainsKey(string key)
        {
            return query.Contains(key);
        }

        public IEnumerator<KeyValuePair<string, StringValues>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(string key, out StringValues value)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return query;
        }
    }
}
