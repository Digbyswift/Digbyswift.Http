using System.Collections.Generic;
using System.Linq;
using Digbyswift.Core.Constants;
using Digbyswift.Core.Extensions;
using Microsoft.AspNetCore.Http;

namespace Digbyswift.Http
{

    public static class PathStringExtensions
    {
       public static IEnumerable<string> Segments(this PathString pathString)
       {
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