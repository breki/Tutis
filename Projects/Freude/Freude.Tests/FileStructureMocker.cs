using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using Brejc.Common.DataStructures;
using Brejc.Common.FileSystem;
using Rhino.Mocks;

namespace Freude.Tests
{
    /// <summary>
    /// Implements expectations to <see cref="IFileSystem"/> mock object methods to simulate a specified file structure.
    /// </summary>
    public class FileStructureMocker
    {
        /// <summary>
        /// Adds a file to the mocked file structure.
        /// </summary>
        /// <param name="fileName">The file name</param>
        /// <returns>This same object</returns>
        public FileStructureMocker AddFile(string fileName)
        {
            Contract.Requires(fileName != null);
            Contract.Ensures(Contract.Result<FileStructureMocker>() != null);

            string currentPath = fileName;

            bool isFileName = true;
            while (!string.IsNullOrEmpty(currentPath))
            {
                string directoryPath = Path.GetDirectoryName(currentPath);

                if (isFileName)
                {
                    string fileNameWithoutDir = Path.GetFileName(fileName);
                    filesByDirs.Add(directoryPath, fileNameWithoutDir);
                    isFileName = false;
                }
                else
                {
                    string lowestDirName = Path.GetFileName (currentPath);
                    dirsByDirs.Add (directoryPath, lowestDirName);
                }

                currentPath = directoryPath;
            }

            return this;
        }

        /// <summary>
        /// Sets up <see cref="IFileSystem"/> mock object expectations with the specified file structure.
        /// </summary>
        /// <param name="fileSystem">The mock object</param>
        public void Mock(IFileSystem fileSystem)
        {
            Contract.Requires(fileSystem != null);

            SetSubdirectoriesExpectations(fileSystem);
            SetFilesExpectations(fileSystem);
            SetExpectationsForDirsWithoutFiles(fileSystem);
        }

        private void SetFilesExpectations(IFileSystem fileSystem)
        {
            foreach (KeyValuePair<string, List<string>> filesInDirPair in filesByDirs)
            {
                string dirPath = filesInDirPair.Key;

                List<IFileInformation> fileInfos = new List<IFileInformation>();

                foreach (string fileInDir in filesInDirPair.Value)
                {
                    IFileInformation fileInfo = MockRepository.GenerateStub<IFileInformation>();
                    fileInfo.Stub(x => x.FullName).Return(Path.Combine(dirPath, fileInDir));
                    fileInfos.Add(fileInfo);
                }

                fileSystem.Stub(x => x.GetDirectoryFiles(dirPath)).Return(fileInfos.ToArray());
            }
        }

        private void SetSubdirectoriesExpectations(IFileSystem fileSystem)
        {
            foreach (KeyValuePair<string, List<string>> dirPair in dirsByDirs)
            {
                string dirPath = dirPair.Key;

                List<IDirectoryInformation> dirInfos = new List<IDirectoryInformation>();

                foreach (string dirInDir in dirPair.Value)
                {
                    IDirectoryInformation dirInfo = MockRepository.GenerateStub<IDirectoryInformation>();
                    dirInfo.Stub(x => x.FullName).Return(Path.Combine(dirPath, dirInDir));
                    dirInfos.Add(dirInfo);
                }

                fileSystem.Stub(x => x.GetDirectorySubdirectories(dirPath)).Return(dirInfos.ToArray());
            }
        }

        private void SetExpectationsForDirsWithoutFiles(IFileSystem fileSystem)
        {
            HashSet<string> dirsWithoutFiles = new HashSet<string> (filesByDirs.Keys);
            dirsWithoutFiles.ExceptWith(dirsByDirs.Keys);

            foreach (string dirWithoutFiles in dirsWithoutFiles)
            {
                string dir = dirWithoutFiles;
                fileSystem.Stub (x => x.GetDirectorySubdirectories(dir)).Return (new IDirectoryInformation[0]);
            }
        }

        private readonly MultiDictionary<string, string> dirsByDirs = new MultiDictionary<string, string>();
        private readonly MultiDictionary<string, string> filesByDirs = new MultiDictionary<string, string>();
    }
}