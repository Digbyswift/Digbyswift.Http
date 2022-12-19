using System;
using System.Collections.Generic;
using System.Linq;
using Digbyswift.Core.Constants;
using Microsoft.AspNetCore.Http;

namespace Digbyswift.Extensions.Http
{

    public static class PathStringExtensions
    {
       public static IEnumerable<string> Segments(this PathString pathString)
       {
           if (pathString == null)
                throw new ArgumentNullException(nameof(pathString));

           return pathString.Value?.SplitAndTrim(CharConstants.ForwardSlash) ?? Enumerable.Empty<string>();
       }

       public static string SegmentAt(this PathString pathString, int index)
       {
           return Segments(pathString).ElementAt(index);
       }

       public static string? SegmentAtOrDefault(this PathString pathString, int index, string? defaultSegment = null)
       {
           return Segments(pathString).ElementAtOrDefault(index) ?? defaultSegment;
       }
    }
}