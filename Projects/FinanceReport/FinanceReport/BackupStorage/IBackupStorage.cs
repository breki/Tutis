namespace FinanceReport.BackupStorage
{
    public interface IBackupStorage
    {
        string FindLatestBackupFile();
    }
}