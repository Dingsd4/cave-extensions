namespace Cave
{
    /// <summary>Gets an interface for <see cref="FileFinder" /> file comparer.</summary>
    public interface IDirectoryFinderComparer
    {
        #region Members

        /// <summary>Determines whether a directory matches the wanted criterias or not.</summary>
        /// <param name="directory">The directory item to be matched.</param>
        /// <returns>True if the directory is a match at the comparer.</returns>
        bool DirectoryMatches(DirectoryItem directory);

        #endregion
    }
}
