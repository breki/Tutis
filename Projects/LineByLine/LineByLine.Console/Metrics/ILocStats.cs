using System.IO;

namespace LineByLine.Console.Metrics
{
    /// <summary>
    /// The class is used to count lines of code and gather statistics
    /// about the no. of lines, no. of empty lines and no. of comments.
    /// </summary>
    public interface ILocStats
    {
        /// <summary>
        /// This method counts the loc statistics.
        /// </summary>
        /// <param name="stream">The stream representing a single
        /// file from which we count the loc statistics.</param>
        /// <returns>LocStatsData object with the results.</returns>
        LocStatsData CountLocStream(Stream stream);
    }
}
