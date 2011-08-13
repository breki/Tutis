namespace Clavis
{
    public interface IClavisRoot
    {
        /// <summary>
        /// Deletes the whole Clavis root if it exists.
        /// </summary>
        void DeleteIfExists();

        IClavisFile OpenFile(string fileName, bool createNew);
        void DeleteFile(string fileName);

        bool HasFile(string fileName);
    }
}