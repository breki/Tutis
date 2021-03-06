using System.IO;
using System.Text;
using Brejc.Common.FileSystem;
using Brejc.Common.Tasks;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace SamsungTvChannelsTool
{
    public class Zipper2 : IZipper2
    {
        public Zipper2 (IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public void Zip (
            ITaskExecutionContext taskExecutionContext,
            string zipFileName,
            FileSet files,
            int? compressionLevel)
        {
            bool aborted = false;

            using (Stream zipFileStream = fileSystem.OpenFileToWrite (zipFileName))
            {
                using (ZipOutputStream zipStream = new ZipOutputStream (zipFileStream))
                {
                    if (compressionLevel.HasValue)
                        zipStream.SetLevel (compressionLevel.Value);

                    buffer = new byte[1024 * 1024];

                    foreach (string fileName in files.Files)
                    {
                        if (taskExecutionContext.ShouldAbort)
                        {
                            aborted = true;
                            break;
                        }

                        string basedFileName;
                        if (!string.IsNullOrEmpty(files.BaseDir))
                        {
                            char lastChar = files.BaseDir[files.BaseDir.Length - 1];
                            int skipCharIndex = 0;
                            if (lastChar != '\\' && lastChar != '/')
                                skipCharIndex++;

                            // cut off the leading part of the path (up to the root directory of the package)
                            basedFileName = fileName.Substring (files.BaseDir.Length + skipCharIndex);
                        }
                        else
                            basedFileName = fileName;

                        basedFileName = ZipEntry.CleanName (basedFileName);

                        //environment.LogMessage("Zipping file '{0}'", basedFileName);
                        AddFileToZip (fileName, basedFileName, zipStream);
                    }
                }
            }

            if (aborted)
                fileSystem.DeleteFile (zipFileName, false);
        }

        public FileSet Unzip (string zipFileName, string destinationDir)
        {
            this.destinationDir = destinationDir;
            FileSet unzippedFiles = new FileSet();

            using (Stream stream = fileSystem.OpenFileToRead (zipFileName))
            using (zipFile = new ZipFile (stream))
            {
                foreach (ZipEntry entry in zipFile)
                {
                    if (entry.IsFile)
                    {
                        ExtractEntry(entry);
                        unzippedFiles.AddFile(entry.Name);
                    }
                    else if (entry.IsDirectory)
                        ExtractEntry(entry);
                }
            }

            return unzippedFiles;
        }

        private void AddFileToZip (string fileName, string basedFileName, ZipOutputStream zipStream)
        {
            using (FileStream fileStream = File.Open (fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                string fileHeader = string.Empty;
                string fileFooter = string.Empty;

                ZipEntry entry = new ZipEntry (basedFileName);
                entry.DateTime = File.GetLastWriteTime (fileName);
                entry.Size = fileStream.Length + fileHeader.Length + fileFooter.Length;
                zipStream.PutNextEntry (entry);

                WriteTextToZipStream (fileHeader, zipStream);

                while (true)
                {
                    int sourceBytes = fileStream.Read (buffer, 0, buffer.Length);

                    if (sourceBytes == 0)
                        break;

                    zipStream.Write (buffer, 0, sourceBytes);
                }

                WriteTextToZipStream (fileFooter, zipStream);
            }
        }

        private void ExtractEntry (ZipEntry entry)
        {
            bool doExtraction = entry.IsCompressionMethodSupported ();
            string targetName = entry.Name;

            if (doExtraction)
            {
                targetName = Path.Combine (destinationDir, entry.Name);
                doExtraction = !string.IsNullOrEmpty (targetName);
            }

            string dirName = null;

            if (doExtraction)
            {
                if (entry.IsDirectory)
                    dirName = targetName;
                else
                    dirName = Path.GetDirectoryName (Path.GetFullPath (targetName));
            }

            if (doExtraction && !Directory.Exists (dirName))
                Directory.CreateDirectory (dirName);

            if (doExtraction && entry.IsFile)
                ExtractFileEntry (entry, targetName);
        }

        private static void WriteTextToZipStream (string text, ZipOutputStream zipStream)
        {
            if (text.Length > 0)
            {
                byte[] bytes = Encoding.ASCII.GetBytes (text);
                zipStream.Write (bytes, 0, bytes.Length);
            }
        }

        private void ExtractFileEntry (ZipEntry entry, string targetName)
        {
            using (FileStream outputStream = File.Create (targetName))
            {
                if (buffer == null)
                    buffer = new byte[4096];

                StreamUtils.Copy (zipFile.GetInputStream (entry), outputStream, buffer);
            }

            File.SetLastWriteTime (targetName, entry.DateTime);
        }

        private byte[] buffer;
        private string destinationDir;
        private readonly IFileSystem fileSystem;
        private ZipFile zipFile;
    }
}