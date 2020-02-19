using System.IO;
using Blueshift.Core.Extensions;

namespace Blueshift.UI.Utility
{
    public static class DirectoryFinder
    {
        /// <summary>
        /// Find directories inside the specified directory or keep looking
        /// higher up in the hierarchy.
        /// <returns>Returns the directories found or an empty list.</returns>
        /// </summary>
        public static DirectoryInfo[] Find(string rootDirectory, string searchPattern)
            => Find(new DirectoryInfo(rootDirectory), searchPattern);

        /// <summary>
        /// Find directories inside the specified directory or keep looking
        /// higher up in the hierarchy.
        /// <returns>Returns the directories found or an empty list.</returns>
        /// </summary>
        public static DirectoryInfo[] Find(DirectoryInfo rootDirectory, string searchPattern)
        {
            DirectoryInfo currentDirectory = rootDirectory;
            DirectoryInfo[] results;

            do
            {
                // Only look in the current directory, and if not found keep looking at the parent.
                results = currentDirectory.GetDirectories(searchPattern, SearchOption.TopDirectoryOnly);
                currentDirectory = currentDirectory.Parent;
            } while (results.IsNullOrEmpty() && currentDirectory != null);

            return results;
        }
    }
}
