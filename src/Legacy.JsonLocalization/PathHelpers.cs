using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Legacy.JsonLocalization
{
	public static class PathHelpers
    {
        // http://codebuckets.com/2017/10/19/getting-the-root-directory-path-for-net-core-applications/
        public static string GetApplicationRoot()
        {
            var exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            var directorySeparator = Path.Combine("x", "x").Replace("x", String.Empty);
            var pattern = @"(?<!fil)[A-Za-z]:\" + directorySeparator + @"+[\S\s]*?(?=\" + directorySeparator + @"+bin)";
            var appPathMatcher = new Regex(pattern);
            var appRoot = appPathMatcher.Match(exePath).Value;

            return appRoot;
        }
    }
}