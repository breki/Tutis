using System.IO;

namespace Clavis
{
    public class ClavisRoot : IClavisRoot
    {
        public ClavisRoot(string directoryName, ITimeService timeService)
        {
            this.directoryName = directoryName;
            this.timeService = timeService;
        }

        public void DeleteIfExists()
        {
            if (Directory.Exists(directoryName))
                Directory.Delete(directoryName, true);
        }

        public IClavisFile OpenFile(string fileName, bool createNew)
        {
            string fullFileName = ConstructFullFileName(fileName);
            ClavisFile file = new ClavisFile(fullFileName, timeService);

            if (createNew || !HasFile(fileName))
                file.Create();

            return file;
        }

        public void DeleteFile(string fileName)
        {
            string fullFileName = ConstructFullFileName(fileName);
            ClavisFile file = new ClavisFile(fullFileName, timeService);
            file.Delete();
        }

        public bool HasFile(string fileName)
        {
            string fullFileName = ConstructFullFileName(fileName);
            return File.Exists(fullFileName);
        }

        private string ConstructFullFileName(string fileName)
        {
            return Path.Combine(directoryName, fileName) + ".s3db";
        }

        private readonly string directoryName;
        private readonly ITimeService timeService;
    }
}