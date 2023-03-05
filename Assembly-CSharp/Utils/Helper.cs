using System;
using System.IO;
using System.Linq;

namespace Modding.Utils;

public static class Helper
{
    public static string CombinePaths(string stem, params string[] paths)
    {
        if (stem == null || paths == null)
            throw new ArgumentNullException();
        return paths.Aggregate(stem, Path.Combine);
    }
}