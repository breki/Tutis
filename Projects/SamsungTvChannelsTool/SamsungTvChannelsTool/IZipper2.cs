using System.Collections.Generic;
using Brejc.Common.FileSystem;
using Brejc.Common.Tasks;

namespace SamsungTvChannelsTool
{
    public interface IZipper2
    {
        void Zip (
            ITaskExecutionContext taskExecutionContext,
            string zipFileName,
            FileSet files,
            int? compressionLevel);

        FileSet Unzip (string zipFileName, string destinationDir);
    }
}