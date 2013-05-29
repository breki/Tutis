using FinanceReport.BackupStorage;
using NUnit.Framework;

namespace FinanceReport.Tests
{
    public class BackupStorageTests
    {
        [Test]
        public void Test()
        {
            IBackupStorage storage = new DropBoxBackupStorage ();
            string file = storage.FindLatestBackupFile();
            Assert.IsNotNull(file);
        } 
    }
}