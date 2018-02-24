// Copyright (c) Arjen Post. See LICENSE and NOTICE in the project root for license information.

namespace PartialResponse.Net.Http.Formatting
{
    internal static class PathUtilities
    {
        internal static string CombinePath(string path, string name)
        {
            if (string.IsNullOrEmpty(path))
            {
                return name;
            }

            return $"{path}/{name}";
        }
    }
}