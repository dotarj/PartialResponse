// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

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

            return string.Format("{0}/{1}", path, name);
        }
    }
}